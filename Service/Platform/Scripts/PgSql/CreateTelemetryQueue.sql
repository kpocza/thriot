DO $$
BEGIN
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND lower(TABLE_NAME)=lower('Setting')) THEN
		CREATE TABLE Queue (
			Id bigserial PRIMARY KEY  NOT NULL,
			DequeueAt timestamp NULL,
			DeviceId char(32) NOT NULL,
			Payload bytea NOT NULL,
			RecordedAt timestamp NOT NULL
		);

		CREATE TABLE Setting (
			Key varchar(32) PRIMARY KEY NOT NULL,
			Value varchar(2048) NOT NULL
		);

		CREATE TYPE EnqueueItemType AS (
			DeviceId char(32) /*NOT NULL*/,
			Payload bytea /*NOT NULL*/,
			RecordedAt timestamp /*NOT NULL*/
		);

		CREATE TYPE DequeueItemType AS (
			Id bigint /*NOT NULL*/,
			DeviceId char(32) /*NOT NULL*/,
			Payload bytea /*NOT NULL*/,
			RecordedAt timestamp /*NOT NULL*/
		);

		CREATE TYPE CommitItemType AS (
			Id bigint /*NOT NULL*/
		);

		INSERT INTO Setting(Key, Value) VALUES('Version', '1');
	END IF;
END
$$;

CREATE OR REPLACE FUNCTION Enqueue
(
	EnqueueItemsJson json
)
RETURNS void AS $$
BEGIN
	INSERT INTO Queue(DeviceId, Payload, RecordedAt)
	SELECT DeviceId, Payload, RecordedAt
	FROM json_populate_recordset(null::EnqueueItemType, EnqueueItemsJson);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION Dequeue
(
	Count int,
	ExpiredMins int
)
RETURNS TABLE (Id bigint, DeviceId char(32), Payload bytea, RecordedAt timestamp) AS $$
DECLARE
	dequeueAtNow timestamp := now() at time zone 'utc';
	expirationDate timestamp := dequeueAtNow - ExpiredMins * INTERVAL '1 minute';
BEGIN
	CREATE TEMP TABLE QueueItemTmp OF DequeueItemType ON COMMIT DROP;

	WITH dq AS (
		UPDATE Queue q
		SET DequeueAt = dequeueAtNow
		FROM 
		(
			SELECT qq.Id, qq.DeviceId, qq.Payload, qq.RecordedAt
			FROM Queue qq
			WHERE
				qq.DequeueAt IS NULL OR qq.DequeueAt < expirationDate
			ORDER BY qq.Id
			LIMIT Count
		) s
		WHERE s.Id = q.Id
		RETURNING
			q.Id,
			q.DeviceId,
			q.Payload,
			q.RecordedAt
	)
	INSERT INTO QueueItemTmp SELECT * FROM dq;
	
	RETURN QUERY
	SELECT quit.Id, quit.DeviceId, quit.Payload, quit.RecordedAt 
	FROM QueueItemTmp quit;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION Commit
(
	CommitItemsJson json
)
RETURNS void AS $$
BEGIN
	DELETE FROM Queue
	WHERE Id IN (SELECT Id FROM json_populate_recordset(null::CommitItemType, CommitItemsJson));
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION Clear()
RETURNS void AS $$
BEGIN
	TRUNCATE TABLE Queue;
END;
$$ LANGUAGE plpgsql;
