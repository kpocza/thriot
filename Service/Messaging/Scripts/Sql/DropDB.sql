-- Stored Procedures --

DROP PROCEDURE [dbo].[RegisterDevice]
DROP PROCEDURE [dbo].[Enqueue]
DROP PROCEDURE [dbo].[Dequeue]
DROP PROCEDURE [dbo].[Peek]
DROP PROCEDURE [dbo].[Commit]

-- Tables --

DROP TABLE [dbo].[Message]
DROP TABLE [dbo].[DeviceMeta]
DROP TABLE [dbo].[Device]
DROP TABLE [dbo].[Setting]

-- Table types --

DROP TYPE [dbo].[DeviceIdTable]
DROP TYPE [dbo].[DeviceIdWithIndexTable]
DROP TYPE [dbo].[EnqueueItemTable]
DROP TYPE [dbo].[ResultTable]
