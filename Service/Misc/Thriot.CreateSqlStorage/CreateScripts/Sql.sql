-- BEGIN: InitialDatabase
BEGIN TRAN
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Setting') 
	BEGIN
		create table [dbo].[Company] (
			[Id] [char](32) not null,
			[Name] [nvarchar](50) not null,
			[TelemetryDataSinkSettingsJson] [nvarchar](2048) null,
			primary key ([Id])
		);
		create table [dbo].[Device] (
			[Id] [char](32) not null,
			[Name] [nvarchar](50) not null,
			[DeviceKey] [varchar](32) not null,
			[NumericId] [bigint] not null,
			[CompanyId] [char](32) not null,
			[NetworkId] [char](32) not null,
			[ServiceId] [char](32) not null,
			primary key ([Id])
		);
		create table [dbo].[LoginUser] (
			[Email] [nvarchar](128) not null,
			[PasswordHash] [varchar](64) not null,
			[Salt] [varchar](32) not null,
			[UserId] [char](32) not null,
			primary key ([Email])
		);
		create table [dbo].[Network] (
			[Id] [char](32) not null,
			[Name] [nvarchar](50) not null,
			[NetworkKey] [varchar](32) not null,
			[TelemetryDataSinkSettingsJson] [nvarchar](2048) null,
			[ParentNetworkId] [char](32) null,
			[CompanyId] [char](32) not null,
			[ServiceId] [char](32) not null,
			primary key ([Id])
		);
		create table [dbo].[Service] (
			[Id] [char](32) not null,
			[Name] [nvarchar](50) not null,
			[ApiKey] [varchar](32) not null,
			[TelemetryDataSinkSettingsJson] [nvarchar](2048) null,
			[CompanyId] [char](32) not null,
			primary key ([Id])
		);
		create table [dbo].[Setting] (
			[Category] [varchar](32) not null,
			[Config] [varchar](32) not null,
			[Value] [nvarchar](2048) not null,
			primary key ([Category], [Config])
		);
		create table [dbo].[User] (
			[Id] [char](32) not null,
			[Name] [nvarchar](50) not null,
			[Email] [nvarchar](128) not null,
			[Activated] [bit] not null,
			[ActivationCode] [varchar](8000) null,
			primary key ([Id])
		);
		create table [dbo].[UserCompany] (
			[UserId] [char](32) not null,
			[CompanyId] [char](32) not null,
			primary key ([UserId], [CompanyId])
		);
		alter table [dbo].[Device] add constraint [Device_Company] foreign key ([CompanyId]) references [dbo].[Company]([Id]);
		alter table [dbo].[Device] add constraint [Device_Network] foreign key ([NetworkId]) references [dbo].[Network]([Id]);
		alter table [dbo].[Device] add constraint [Device_Service] foreign key ([ServiceId]) references [dbo].[Service]([Id]);
		alter table [dbo].[Network] add constraint [Network_ChildNetworks] foreign key ([ParentNetworkId]) references [dbo].[Network]([Id]);
		alter table [dbo].[Network] add constraint [Network_Company] foreign key ([CompanyId]) references [dbo].[Company]([Id]);
		alter table [dbo].[Network] add constraint [Network_Service] foreign key ([ServiceId]) references [dbo].[Service]([Id]);
		alter table [dbo].[Service] add constraint [Service_Company] foreign key ([CompanyId]) references [dbo].[Company]([Id]);
		alter table [dbo].[UserCompany] add constraint [User_Companies_Source] foreign key ([UserId]) references [dbo].[User]([Id]);
		alter table [dbo].[UserCompany] add constraint [User_Companies_Target] foreign key ([CompanyId]) references [dbo].[Company]([Id]);

		INSERT INTO Setting(Category, Config, Value) VALUES('Version', 'System', '0.1');
		INSERT INTO Setting(Category, Config, Value) VALUES('Version', 'Database', '1');
	END
COMMIT TRAN
-- END: InitialDatabase

