-- Stored Procedures --

DROP FUNCTION RegisterDevice(varchar(32),int);
DROP FUNCTION Enqueue(json);
DROP FUNCTION Dequeue(json);
DROP FUNCTION Peek(json);
DROP FUNCTION Commit(json);

-- Tables --

DROP TABLE DeviceData;
DROP TABLE DeviceMeta;
DROP TABLE DeviceNumeric;


-- Table types --

DROP TYPE DeviceIdTableRow;
DROP TYPE DeviceIdWithIndexTableRow;
DROP TYPE EnqueueItemTableRow;
DROP TYPE ResultTableRow;
