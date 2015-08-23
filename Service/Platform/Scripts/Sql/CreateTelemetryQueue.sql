IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Setting') 
BEGIN
	CREATE TABLE [dbo].[Queue] (
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
		[DequeueAt] [datetime] NULL,
		[DeviceId] [char](32) NOT NULL,
		[Payload] [varbinary](8000) NOT NULL,
		[RecordedAt] [datetime] NOT NULL,
		CONSTRAINT [PK_Queue] PRIMARY KEY CLUSTERED ([Id] ASC)
	)

	CREATE TABLE [dbo].[Setting] (
		[Key] [varchar](32) NOT NULL,
		[Value] [nvarchar](2048) NOT NULL,
		CONSTRAINT [PK_Setting] PRIMARY KEY CLUSTERED ([Key] ASC)
	)

	CREATE TYPE [dbo].[EnqueueItemType] AS TABLE (
		[DeviceId] char(32) NOT NULL,
		[Payload] varbinary(8000) NOT NULL,
		[RecordedAt] datetime NOT NULL
	)

	CREATE TYPE [dbo].[CommitItemType] AS TABLE (
		[Id] [bigint] NOT NULL
	)

	INSERT INTO Setting([Key], Value) VALUES('Version', '1');
END
GO

IF EXISTS (select 1 from sys.procedures WHERE name = 'Enqueue')
	DROP PROCEDURE [dbo].[Enqueue]
GO
CREATE PROCEDURE [dbo].[Enqueue]
(
	@EnqueueItems EnqueueItemType READONLY
)
AS
	INSERT INTO [dbo].[Queue](DeviceId, Payload, RecordedAt)
	SELECT DeviceId, Payload, RecordedAt
	FROM @EnqueueItems
GO

IF EXISTS (select 1 from sys.procedures WHERE name = 'Dequeue')
	DROP PROCEDURE [dbo].[Dequeue]
GO
CREATE PROCEDURE [dbo].[Dequeue]
(
	@Count int,
	@ExpiredMins int
)
AS
	DECLARE @dequeueAt AS DATETIME = GETUTCDATE();
	DECLARE @expirationDate AS DATETIME = DATEADD(minute, -@ExpiredMins, @dequeueAt)
	;
	WITH q AS
	(
		SELECT TOP (@Count) * 
		FROM [dbo].[Queue] WITH (rowlock, readpast, updlock)
		WHERE
			DequeueAt IS NULL OR DequeueAt < @expirationDate
		ORDER BY Id
	)
	UPDATE q
	SET 
		DequeueAt = @dequeueAt
	OUTPUT 
		inserted.Id,
		inserted.DeviceId,
		inserted.Payload,
		inserted.RecordedAt
GO

IF EXISTS (select 1 from sys.procedures WHERE name = 'Commit')
	DROP PROCEDURE [dbo].[Commit]
GO
CREATE PROCEDURE [dbo].[Commit]
(
	@CommitItems CommitItemType READONLY
)
AS
	DELETE FROM [dbo].[Queue] WITH (rowlock)
	WHERE Id IN (SELECT Id FROM @CommitItems)
GO

IF EXISTS (select 1 from sys.procedures WHERE name = 'Clear')
	DROP PROCEDURE [dbo].[Clear]
GO
CREATE PROCEDURE [dbo].[Clear]
AS
	TRUNCATE TABLE [dbo].[Queue]
GO
