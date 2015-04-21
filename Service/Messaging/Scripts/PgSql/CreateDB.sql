
-- Tables --

CREATE TABLE DeviceNumeric(
	Id bigserial primary key NOT NULL,
	Uid varchar(32) NOT NULL
);

CREATE TABLE DeviceData(
	DeviceId bigint NOT NULL,
	Idx int NOT NULL,
	Payload bytea NOT NULL,
	Timestamp timestamp NOT NULL,
	CONSTRAINT PK_DeviceData PRIMARY KEY (DeviceId, Idx)
);

CREATE TABLE DeviceMeta(
	DeviceId bigint primary key NOT NULL,
	DequeueIndex int NOT NULL,
	EnqueueIndex int NOT NULL,
	Peek boolean NOT NULL,
	QueueSize int NOT NULL,
	Version int NOT NULL
);


-- Keys and Indices -- 
CREATE UNIQUE INDEX IX_Device ON DeviceNumeric
(
	Uid ASC
);


ALTER TABLE DeviceData  ADD  CONSTRAINT FK_DeviceData_DeviceNumeric FOREIGN KEY(DeviceId)
REFERENCES DeviceNumeric (Id);

ALTER TABLE DeviceMeta ADD  CONSTRAINT FK_DeviceMeta_DeviceNumeric FOREIGN KEY(DeviceId)
REFERENCES DeviceNumeric (Id);


-- Table types --

CREATE TYPE DeviceIdTableRow AS (
	DeviceId bigint /*NOT NULL*/
);

CREATE TYPE DeviceIdWithIndexTableRow AS (
	DeviceId bigint /*NOT NULL*/,
	Index int /*NULL*/
);

CREATE TYPE EnqueueItemTableRow AS (
	DeviceId bigint /*NOT NULL*/,
	Payload bytea /*NOT NULL*/,
	Timestamp timestamp /*NOT NULL*/
);

CREATE TYPE ResultTableRow AS (
	DeviceId bigint /*NOT NULL*/,
	DequeueIndex int /*NOT NULL*/,
	EnqueueIndex int /*NOT NULL*/,
	Peek boolean /*NOT NULL*/,
	Version int /*NOT NULL*/,
	QueueSize int /*NOT NULL*/,
	MessageId int /*NOT NULL*/
);

-- Stored Procedures --

CREATE FUNCTION RegisterDevice (
	Uid varchar(32),
	QueueSize int = 100,
	OUT DeviceId bigint
)
RETURNS bigint AS $$
BEGIN
	
	INSERT INTO DeviceNumeric(Uid) VALUES (Uid) RETURNING Id INTO DeviceId;
	
	INSERT INTO DeviceMeta(DeviceId, DequeueIndex, EnqueueIndex, Peek, QueueSize, Version) 
	VALUES(@DeviceId, 0, 0, false, QueueSize, 0);

	INSERT INTO DeviceData(DeviceId, Idx, Payload, Timestamp)
	SELECT DeviceId, generate_series(0, QueueSize - 1), E'\\000', current_timestamp;
END;
$$ LANGUAGE plpgsql;


CREATE FUNCTION Enqueue
(
	MessagesJson json
)
RETURNS TABLE (DeviceId bigint /*NOT NULL*/, DequeueIndex int /*NOT NULL*/, EnqueueIndex int /*NOT NULL*/, Peek boolean /*NOT NULL*/, Version int /*NOT NULL*/, MessageId int /*NOT NULL*/) AS $$
BEGIN
	CREATE TEMP TABLE MessageTmp OF ResultTableRow ON COMMIT DROP;
	CREATE TEMP TABLE Messages OF EnqueueItemTableRow ON COMMIT DROP;

	INSERT INTO Messages
	SELECT * FROM json_populate_recordset(null::EnqueueItemTableRow, MessagesJson);

	WITH recalc as (
		UPDATE DeviceMeta dm /* with(rowlock)*/
		SET 	EnqueueIndex = dm.EnqueueIndex + 1,
			DequeueIndex = CASE WHEN dm.DequeueIndex <= dm.EnqueueIndex - dm.QueueSize + 1 
						 THEN dm.DequeueIndex + 1 
						 ELSE dm.DequeueIndex 
					 END,
			Peek = CASE WHEN dm.DequeueIndex <= dm.EnqueueIndex - dm.QueueSize + 1 
						 THEN false
						 ELSE dm.Peek
					 END,
			Version = dm.Version + 1
		FROM Messages dmsg
		WHERE dm.DeviceId = dmsg.DeviceId
		RETURNING
			dm.DeviceId,
			dm.DequeueIndex,
			dm.EnqueueIndex,
			dm.Peek,
			dm.Version,
			dm.QueueSize,
			dm.EnqueueIndex - 1 as MessageId
	)
	INSERT INTO MessageTmp SELECT * FROM recalc;

	UPDATE DeviceData /*with(rowlock)*/
	SET 	Payload = dm.Payload,
		Timestamp = dm.Timestamp
	FROM MessageTmp m, Messages dm
	WHERE
		DeviceData.DeviceId = m.DeviceId AND 
		DeviceData.Idx = (m.MessageId % m.QueueSize) AND
		DeviceData.DeviceId = dm.DeviceId;

	RETURN QUERY 
	SELECT 
		mt.DeviceId, 
		mt.DequeueIndex,
		mt.EnqueueIndex, 
		mt.Peek, 
		mt.Version,
		mt.MessageId
	FROM MessageTmp mt;

--EXCEPTION
--	WHEN OTHERS THEN
--		RETURN QUERY 
--		SELECT NULL::bigint as deviceid, NULL::int as dequeueindex, NULL::int as enqueueindex, NULL::boolean as peek, NULL::int as version, NULL::int as messageid;
END;
$$ LANGUAGE plpgsql;

CREATE FUNCTION Dequeue
(
	DequeueItemsJson json
)
RETURNS TABLE (IsMessage boolean /*NOT NULL*/, DeviceId bigint /*NOT NULL*/, DequeueIndex int /*NOT NULL*/, EnqueueIndex int /*NOT NULL*/, Peek boolean /*NOT NULL*/, Version int /*NOT NULL*/, MessageId int /* NULL*/, Payload bytea /*NULL*/, "Timestamp" timestamp /*NULL*/) AS $$
BEGIN	
	CREATE TEMP TABLE MessageTmp OF ResultTableRow ON COMMIT DROP;
	CREATE TEMP TABLE DequeueItems OF DeviceIdWithIndexTableRow ON COMMIT DROP;
	
	INSERT INTO DequeueItems
	SELECT * FROM json_populate_recordset(null::DeviceIdWithIndexTableRow, DequeueItemsJson);

	WITH recalc AS (
		UPDATE DeviceMeta dm
		SET	DequeueIndex = dm.DequeueIndex + 1,
			Peek = false,
			Version = dm.Version + 1
		FROM DequeueItems di
		WHERE 
			dm.DeviceId = di.DeviceId AND 
			dm.DequeueIndex < dm.EnqueueIndex
		RETURNING
			dm.DeviceId,
			dm.DequeueIndex,
			dm.EnqueueIndex,
			dm.Peek,
			dm.Version,
			dm.QueueSize,
			dm.DequeueIndex -1
	)
	INSERT INTO MessageTmp SELECT * FROM recalc;
	
	RETURN QUERY
	SELECT
		true as IsMessage, 
		mt.DeviceId,
		mt.DequeueIndex,
		mt.EnqueueIndex,
		mt.Peek,
		mt.Version,
		mt.MessageId,
		NULL as Payload,
		NULL as Timestamp
	FROM
		MessageTmp mt
		INNER JOIN DequeueItems di ON (mt.DeviceId = di.DeviceId)
	WHERE
		mt.MessageId = di.Index
	UNION
	SELECT 
		true as IsMessage,
		mt.DeviceId,
		mt.DequeueIndex,
		mt.EnqueueIndex,
		mt.Peek,
		mt.Version,
		mt.MessageId,
		dd.Payload,
		dd.Timestamp
	FROM
		MessageTmp mt
		INNER JOIN DequeueItems di ON (mt.DeviceId = di.DeviceId)
		INNER JOIN DeviceData dd ON (dd.DeviceId = di.DeviceId AND dd.Idx = (mt.MessageId % mt.QueueSize))
	WHERE
		mt.MessageId <> di.Index
	UNION
	SELECT
		false as IsMessage,
		di.DeviceId,
		dm.DequeueIndex,
		dm.EnqueueIndex,
		dm.Peek,
		dm.Version,
		NULL as MessageId,
		NULL as Payload,
		NULL as Timestamp
	FROM
		DequeueItems di
		INNER JOIN DeviceMeta dm ON (dm.DeviceId = di.DeviceId)
	WHERE
		di.DeviceId NOT IN (SELECT m.DeviceId from MessageTmp m);

END;
$$ LANGUAGE plpgsql;

CREATE FUNCTION Peek
(
	DequeueItemsJson json
)
RETURNS TABLE (IsMessage boolean /*NOT NULL*/, DeviceId bigint /*NOT NULL*/, DequeueIndex int /*NOT NULL*/, EnqueueIndex int /*NOT NULL*/, Peek boolean /*NOT NULL*/, Version int /*NOT NULL*/, MessageId int /* NULL*/, Payload bytea /*NULL*/, "Timestamp" timestamp /*NULL*/) AS $$
BEGIN	
	CREATE TEMP TABLE MessageTmp OF ResultTableRow ON COMMIT DROP;
	CREATE TEMP TABLE DequeueItems OF DeviceIdWithIndexTableRow ON COMMIT DROP;
	
	INSERT INTO DequeueItems
	SELECT * FROM json_populate_recordset(null::DeviceIdWithIndexTableRow, DequeueItemsJson);

	WITH recalc AS (
		UPDATE DeviceMeta dm
		SET	Peek = true,
			Version = dm.Version + 1
		FROM DequeueItems di
		WHERE 
			dm.DeviceId = di.DeviceId AND 
			dm.DequeueIndex < dm.EnqueueIndex
		RETURNING
			dm.DeviceId,
			dm.DequeueIndex,
			dm.EnqueueIndex,
			dm.Peek,
			dm.Version,
			dm.QueueSize,
			dm.DequeueIndex
	)
	INSERT INTO MessageTmp SELECT * FROM recalc;
	
	RETURN QUERY
	SELECT
		true as IsMessage, 
		mt.DeviceId,
		mt.DequeueIndex,
		mt.EnqueueIndex,
		mt.Peek,
		mt.Version,
		mt.MessageId,
		NULL as Payload,
		NULL as Timestamp
	FROM
		MessageTmp mt
		INNER JOIN DequeueItems di ON (mt.DeviceId = di.DeviceId)
	WHERE
		mt.MessageId = di.Index
	UNION
	SELECT 
		true as IsMessage,
		mt.DeviceId,
		mt.DequeueIndex,
		mt.EnqueueIndex,
		mt.Peek,
		mt.Version,
		mt.MessageId,
		dd.Payload,
		dd.Timestamp
	FROM
		MessageTmp mt
		INNER JOIN DequeueItems di ON (mt.DeviceId = di.DeviceId)
		INNER JOIN DeviceData dd ON (dd.DeviceId = di.DeviceId AND dd.Idx = (mt.MessageId % mt.QueueSize))
	WHERE
		mt.MessageId <> di.Index
	UNION
	SELECT
		false as IsMessage,
		di.DeviceId,
		dm.DequeueIndex,
		dm.EnqueueIndex,
		dm.Peek,
		dm.Version,
		NULL as MessageId,
		NULL as Payload,
		NULL as Timestamp
	FROM
		DequeueItems di
		INNER JOIN DeviceMeta dm ON (dm.DeviceId = di.DeviceId)
	WHERE
		di.DeviceId NOT IN (SELECT m.DeviceId from MessageTmp m);

END;
$$ LANGUAGE plpgsql;

CREATE FUNCTION Commit
(
	CommitItemsJson json
)
RETURNS TABLE (DeviceId bigint /*NOT NULL*/, DequeueIndex int /*NOT NULL*/, EnqueueIndex int /*NOT NULL*/, Peek boolean /*NOT NULL*/, Version int /*NOT NULL*/) AS $$
BEGIN
	CREATE TEMP TABLE MessageTmp OF ResultTableRow ON COMMIT DROP;
	CREATE TEMP TABLE CommitItems OF DeviceIdTableRow ON COMMIT DROP;
	
	INSERT INTO CommitItems
	SELECT * FROM json_populate_recordset(null::DeviceIdTableRow, CommitItemsJson);

	with recalc AS (
		UPDATE DeviceMeta dm
		SET 	DequeueIndex = dm.DequeueIndex + 1, 
			Peek = false
		FROM CommitItems ci
		WHERE 
			dm.DeviceId = ci.DeviceId AND 
			dm.DequeueIndex < dm.EnqueueIndex AND
			dm.Peek = true
		RETURNING
			dm.DeviceId,
			dm.DequeueIndex,
			dm.EnqueueIndex,
			dm.Peek,
			dm.Version
	)
	INSERT INTO MessageTmp SELECT * FROM recalc;

	RETURN QUERY
	SELECT 
		m.DeviceId, 
		m.DequeueIndex,
		m.EnqueueIndex, 
		m.Peek, 
		m.Version
	from MessageTmp m;
END;
$$ LANGUAGE plpgsql;
