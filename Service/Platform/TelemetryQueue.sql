IF EXISTS (select 1 from sys.procedures WHERE name = 'Enqueue')
	DROP PROCEDURE [dbo].[Enqueue]
GO
IF EXISTS (select 1 from sys.procedures WHERE name = 'Dequeue')
	DROP PROCEDURE [dbo].[Dequeue]
GO
IF EXISTS (select 1 from sys.procedures WHERE name = 'Commit')
	DROP PROCEDURE [dbo].[Commit]
GO
IF EXISTS (select 1 from sys.procedures WHERE name = 'Clear')
	DROP PROCEDURE [dbo].[Clear]
GO

IF EXISTS (select 1 from sys.types WHERE name = 'EnqueueItemType')
	DROP TYPE [dbo].[EnqueueItemType]
GO
IF EXISTS (select 1 from sys.types WHERE name = 'CommitItemType')
	DROP TYPE [dbo].[CommitItemType]
GO


IF NOT EXISTS (select 1 from sys.tables WHERE name = 'Queue')
	CREATE TABLE [dbo].[Queue] (
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
		[DequeueAt] [datetime] NULL,
		[DeviceId] [char](32) NOT NULL,
		[Data] [varbinary](8000) NOT NULL,
		[RecordedAt] [datetime] NOT NULL,
		CONSTRAINT [PK_Queue] PRIMARY KEY CLUSTERED ([Id] ASC)
	)
GO

CREATE TYPE [dbo].[EnqueueItemType] AS TABLE (
	[DeviceId] [bigint] NOT NULL,
	Data varbinary(8000) NOT NULL,
	RecordedAt datetime NOT NULL
)
GO

CREATE TYPE [dbo].[CommitItemType] AS TABLE (
	[Id] [bigint] NOT NULL
)
GO

CREATE PROCEDURE [dbo].[Enqueue]
(
	@EnqueueItems EnqueueItemType READONLY
)
AS
	INSERT INTO [dbo].[Queue](DeviceId, Data, RecordedAt)
	SELECT DeviceId, Data, RecordedAt
	FROM @EnqueueItems
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
		inserted.Data,
		inserted.RecordedAt
GO

CREATE PROCEDURE [dbo].[Commit]
(
	@CommitItems CommitItemType READONLY
)
AS
	DELETE FROM [dbo].[Queue] WITH (rowlock)
	WHERE Id IN (SELECT Id FROM @CommitItems)
GO

CREATE PROCEDURE [dbo].[Clear]
AS
	TRUNCATE TABLE [dbo].[Queue]
GO
