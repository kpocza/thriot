-- Stored Procedures --

DROP FUNCTION CreateDatabase();
DROP FUNCTION RegisterDevice(varchar(32),int);
DROP FUNCTION Enqueue(json);
DROP FUNCTION Dequeue(json);
DROP FUNCTION Peek(json);
DROP FUNCTION Commit(json);

-- Tables --

DROP TABLE Message;
DROP TABLE DeviceMeta;
DROP TABLE Device;
DROP TABLE Setting;


-- Table types --

DROP TYPE DeviceIdTableRow;
DROP TYPE DeviceIdWithIndexTableRow;
DROP TYPE EnqueueItemTableRow;
DROP TYPE ResultTableRow;
