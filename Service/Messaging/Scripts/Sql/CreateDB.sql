-- Tables --

CREATE TABLE [dbo].[DeviceNumeric](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Uid] [varchar](32) NOT NULL,
 CONSTRAINT [PK_Device] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[DeviceData](
	[DeviceId] [bigint] NOT NULL,
	[Idx] [int] NOT NULL,
	[Payload] [varbinary](512) NOT NULL,
	[Timestamp] [datetime2] NOT NULL,
 CONSTRAINT [PK_DeviceData] PRIMARY KEY CLUSTERED ([DeviceId] ASC, [Idx] ASC) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[DeviceMeta](
	[DeviceId] [bigint] NOT NULL,
	[DequeueIndex] [int] NOT NULL,
	[EnqueueIndex] [int] NOT NULL,
	[Peek] [bit] NOT NULL,
	[QueueSize] [int] NOT NULL,
	[Version] [int] NOT NULL
 CONSTRAINT [PK_DeviceMeta] PRIMARY KEY CLUSTERED ([DeviceId] ASC) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Keys and Indices -- 
CREATE UNIQUE NONCLUSTERED INDEX [IX_Device] ON [dbo].[DeviceNumeric]
(
	[Uid] ASC
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DeviceData]  WITH CHECK ADD  CONSTRAINT [FK_DeviceData_DeviceNumeric] FOREIGN KEY([DeviceId])
REFERENCES [dbo].[DeviceNumeric] ([Id])
GO

ALTER TABLE [dbo].[DeviceMeta]  WITH CHECK ADD  CONSTRAINT [FK_DeviceMeta_DeviceNumeric] FOREIGN KEY([DeviceId])
REFERENCES [dbo].[DeviceNumeric] ([Id])
GO


-- Table types --

CREATE TYPE [dbo].[DeviceIdTable] AS TABLE(
	[DeviceId] [bigint] NOT NULL
)
GO

CREATE TYPE [dbo].[DeviceIdWithIndexTable] AS TABLE(
	[DeviceId] [bigint] NOT NULL,
	[Index] [int] NULL
)
GO

CREATE TYPE [dbo].[EnqueueItemTable] AS TABLE(
	[DeviceId] [bigint] NOT NULL,
	[Payload] [varbinary](512) NOT NULL,
	[Timestamp] [datetime2](7) NOT NULL
)
GO

CREATE TYPE [dbo].[ResultTable] AS TABLE(
	[DeviceId] [bigint] NOT NULL,
	[DequeueIndex] [int] NOT NULL,
	[EnqueueIndex] [int] NOT NULL,
	[Peek] [bit] NOT NULL,
	[Version] [int] NOT NULL,
	[QueueSize] [int] NOT NULL,
	[MessageId] [int] NOT NULL
)
GO

-- Stored Procedures --

CREATE PROCEDURE [dbo].[RegisterDevice]
	@Uid varchar(32),
	@QueueSize int = 100,
	@DeviceId bigint OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[DeviceNumeric](Uid) VALUES (@Uid)

	set @DeviceId = SCOPE_IDENTITY()
	
	INSERT INTO [dbo].[DeviceMeta](DeviceId, DequeueIndex, EnqueueIndex, Peek, QueueSize, [Version]) 
	VALUES(@DeviceId, 0, 0, 0, @QueueSize, 0)

	INSERT INTO [dbo].[DeviceData](DeviceId, Idx, [Payload], [Timestamp])
	SELECT TOP (@QueueSize) @DeviceId, Idx = (ROW_NUMBER() OVER (ORDER BY [object_id])) - 1, 0x0, GETUTCDATE() FROM sys.all_objects ORDER BY Idx;
END
GO

CREATE PROCEDURE [dbo].[Enqueue]
(
	@Messages EnqueueItemTable READONLY
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @MessageTmp ResultTable;

	BEGIN TRY
		BEGIN TRAN
	
		UPDATE [dbo].[DeviceMeta] with(rowlock)
		SET EnqueueIndex = EnqueueIndex + 1,
			DequeueIndex = CASE WHEN DequeueIndex <= EnqueueIndex - QueueSize + 1 
					         THEN DequeueIndex + 1 
					         ELSE DequeueIndex 
				         END,
			Peek = CASE WHEN DequeueIndex <= EnqueueIndex - QueueSize + 1 
					         THEN 0
					         ELSE Peek
				         END,
			[Version] = [Version] + 1
		OUTPUT
			inserted.DeviceId as DeviceId,
			inserted.DequeueIndex as DequeueIndex,
			inserted.EnqueueIndex as EnqueueIndex,
			inserted.Peek as Peek,
			inserted.[Version] as [Version],
			inserted.QueueSize as QueueSize,
			deleted.EnqueueIndex as MessageId
			INTO @MessageTmp
		FROM @Messages dmsg
		WHERE [DeviceMeta].DeviceId = dmsg.DeviceId

		UPDATE [dbo].[DeviceData] with(rowlock)
		SET [Payload] = dm.[Payload],
			[Timestamp] = dm.[Timestamp]
		FROM @MessageTmp m, @Messages dm
		WHERE
			DeviceData.DeviceId = m.DeviceId AND 
			DeviceData.Idx = (m.MessageId % m.QueueSize) AND
			DeviceData.DeviceId = dm.DeviceId

		COMMIT TRAN

		SELECT 
			DeviceId, 
			DequeueIndex,
			EnqueueIndex, 
			Peek, 
			[Version],
			MessageId
		FROM @MessageTmp

		RETURN 0
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 ROLLBACK TRAN

		RETURN 1
	END CATCH
END
GO

CREATE PROCEDURE [dbo].[Dequeue]
(
	@DequeueItems DeviceIdWithIndexTable READONLY
)
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @MessageTmp ResultTable;

	BEGIN TRY
		BEGIN TRAN

		UPDATE [dbo].[DeviceMeta] with (rowlock, readpast)
		SET	DequeueIndex = DequeueIndex + 1,
			Peek = 0,
			[Version] = [Version] + 1
		OUTPUT
			inserted.DeviceId as DeviceId,
			inserted.DequeueIndex as DequeueIndex,
			inserted.EnqueueIndex as EnqueueIndex,
			inserted.Peek as Peek,
			inserted.[Version] as [Version],
			inserted.QueueSize as QueueSize,
			deleted.DequeueIndex as MessageId
			INTO @MessageTmp
		FROM @DequeueItems di
		WHERE 
			DeviceMeta.DeviceId = di.DeviceId AND 
			DequeueIndex < EnqueueIndex
	
		SELECT
			1 as IsMessage, 
			mt.DeviceId,
			mt.DequeueIndex,
			mt.EnqueueIndex,
			mt.Peek,
			mt.[Version],
			mt.MessageId,
			NULL as [Payload],
			NULL as [Timestamp]
		FROM
			@MessageTmp mt
			INNER JOIN @DequeueItems di ON (mt.DeviceId = di.DeviceId)
		WHERE
			mt.MessageId = di.[Index]
		UNION
		SELECT 
			1 as IsMessage, 
			mt.DeviceId,
			mt.DequeueIndex,
			mt.EnqueueIndex,
			mt.Peek,
			mt.[Version],
			mt.MessageId,
			dd.[Payload],
			dd.[Timestamp]
		FROM
			@MessageTmp mt
			INNER JOIN @DequeueItems di ON (mt.DeviceId = di.DeviceId)
			INNER JOIN DeviceData dd with (nolock) ON (dd.DeviceId = di.DeviceId AND dd.Idx = (mt.MessageId % mt.QueueSize))
		WHERE
			mt.MessageId <> di.[Index]
		UNION
		SELECT
			0 as IsMessage, 
			di.DeviceId,
			dm.DequeueIndex,
			dm.EnqueueIndex,
			dm.Peek,
			dm.[Version],
			NULL as MessageId,
			NULL as [Payload],
			NULL as [Timestamp]
		FROM
			@DequeueItems di
			INNER JOIN DeviceMeta dm with(rowlock, readpast) ON (dm.DeviceId = di.DeviceId)
		WHERE
			di.DeviceId NOT IN (SELECT DeviceId from @MessageTmp)

		COMMIT TRAN
		RETURN 0
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 ROLLBACK TRAN
		RETURN 1
	END CATCH	
END
GO


CREATE PROCEDURE [dbo].[Peek]
(
	@DequeueItems DeviceIdWithIndexTable READONLY
)
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @MessageTmp ResultTable;

	BEGIN TRY
		BEGIN TRAN

		UPDATE [dbo].[DeviceMeta] with (rowlock, readpast)
		SET	Peek = 1,
			[Version] = [Version] + 1
		OUTPUT
			inserted.DeviceId as DeviceId,
			inserted.DequeueIndex as DequeueIndex,
			inserted.EnqueueIndex as EnqueueIndex,
			inserted.Peek as Peek,
			inserted.[Version] as [Version],
			inserted.QueueSize as QueueSize,
			deleted.DequeueIndex as MessageId
			INTO @MessageTmp
		FROM @DequeueItems di
		WHERE 
			DeviceMeta.DeviceId = di.DeviceId AND 
			DequeueIndex < EnqueueIndex

		SELECT 
			1 as IsMessage,
			mt.DeviceId,
			mt.DequeueIndex,
			mt.EnqueueIndex,
			mt.Peek,
			mt.[Version],
			mt.MessageId,
			NULL as [Payload],
			NULL as [Timestamp]
		FROM
			@MessageTmp mt
			INNER JOIN @DequeueItems di ON (mt.DeviceId = di.DeviceId)
		WHERE
			mt.MessageId = di.[Index]
		UNION
		SELECT
			1 as IsMessage,
			mt.DeviceId,
			mt.DequeueIndex,
			mt.EnqueueIndex,
			mt.Peek,
			mt.[Version],
			mt.MessageId,
			dd.[Payload],
			dd.[Timestamp]
		FROM
			@MessageTmp mt
			INNER JOIN @DequeueItems di ON (mt.DeviceId = di.DeviceId)
			INNER JOIN DeviceData dd with (nolock) ON (dd.DeviceId = di.DeviceId AND dd.Idx = (mt.MessageId % mt.QueueSize))
		WHERE
			mt.MessageId <> di.[Index]
		UNION
		SELECT
			0 as IsMessage, 
			di.DeviceId,
			dm.DequeueIndex,
			dm.EnqueueIndex,
			dm.Peek,
			dm.[Version],
			NULL as MessageId,
			NULL as [Payload],
			NULL as [Timestamp]
		FROM
			@DequeueItems di
			INNER JOIN DeviceMeta dm with(rowlock, readpast) ON (dm.DeviceId = di.DeviceId)
		WHERE
			di.DeviceId NOT IN (SELECT DeviceId from @MessageTmp)

		COMMIT TRAN
		RETURN 0
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 ROLLBACK TRAN
		RETURN 1
	END CATCH	
END
GO

CREATE PROCEDURE [dbo].[Commit]
(
	@CommitItems DeviceIdTable READONLY
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @MessageTmp ResultTable;
	
	BEGIN TRY

		UPDATE [dbo].[DeviceMeta] with (rowlock)
		SET DequeueIndex = DequeueIndex + 1, 
			Peek = 0
		OUTPUT
			inserted.DeviceId as DeviceId,
			inserted.DequeueIndex as DequeueIndex,
			inserted.EnqueueIndex as EnqueueIndex,
			inserted.Peek as [Peek],
			inserted.[Version] as [Version],
			inserted.QueueSize as [QueueSize],
			0 as [MessageId]
			INTO @MessageTmp
		FROM @CommitItems ci
		WHERE 
			DeviceMeta.DeviceId = ci.DeviceId AND 
			DequeueIndex < EnqueueIndex AND
			Peek = 1

		SELECT 
			DeviceId, 
			DequeueIndex,
			EnqueueIndex, 
			Peek, 
			[Version]
		FROM @MessageTmp

		RETURN 0
	END TRY
	BEGIN CATCH
		RETURN 1
	END CATCH	
END
GO
