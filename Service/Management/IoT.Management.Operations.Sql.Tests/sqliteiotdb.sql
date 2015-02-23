create table [Company] (
    [Id] [varchar](32) not null,
    [Name] [nvarchar](50) not null,
    [TelemetryDataSinkSettingsJson] [nvarchar](2048) null,
    primary key ([Id])
);
GO

create table [Device] (
    [Id] [varchar](32) not null,
    [Name] [nvarchar](50) not null,
    [DeviceKey] [varchar](32) not null,
    [NumericId] [bigint] not null,
    [CompanyId] [varchar](32) not null,
    [NetworkId] [varchar](32) not null,
    [ServiceId] [varchar](32) not null,
    primary key ([Id]),
	foreign key ([CompanyId]) references [Company]([Id]),
	foreign key ([NetworkId]) references [Network]([Id]),
	foreign key ([ServiceId]) references [Service]([Id])
);
GO

create table [LoginUser] (
    [Email] [nvarchar](128) not null,
    [PasswordHash] [varchar](64) not null,
    [Salt] [varchar](32) not null,
    [UserId] [nvarchar](32) not null,
    primary key ([Email])
);
GO

create table [Network] (
    [Id] [varchar](32) not null,
    [Name] [nvarchar](50) not null,
    [ParentNetworkId] [varchar](32) null,
    [CompanyId] [varchar](32) not null,
    [ServiceId] [varchar](32) not null,
    [NetworkKey] [varchar](32) not null,
    [TelemetryDataSinkSettingsJson] [nvarchar](2048) null,
    primary key ([Id]),
	foreign key ([ParentNetworkId]) references [Network]([Id]),
	foreign key ([CompanyId]) references [Company]([Id]),
	foreign key ([ServiceId]) references [Service]([Id])
);
GO

create table [Service] (
    [Id] [varchar](32) not null,
    [Name] [nvarchar](50) not null,
    [ApiKey] [varchar](32) not null,
    [TelemetryDataSinkSettingsJson] [nvarchar](2048) null,
    [CompanyId] [varchar](32) not null,
    primary key ([Id]),
	foreign key ([CompanyId]) references [Company]([Id])
);
GO

create table [User] (
    [Id] [varchar](32) not null,
    [Name] [nvarchar](50) not null,
    [Email] [nvarchar](128) not null,
    [Activated] [bit] not null,
    [ActivationCode] [varchar](8000) null,
    primary key ([Id])
);
GO

create table [UserCompany] (
    [UserId] [varchar](32) not null,
    [CompanyId] [varchar](32) not null,
    primary key ([UserId], [CompanyId]),
	foreign key ([UserId]) references [User]([Id]),
	foreign key ([CompanyId]) references [Company]([Id])
);
GO

create table [Setting] (
    [Category] [varchar](32) not null,
    [Config] [varchar](32) not null,
    [Value] [nvarchar](2048) not null,
    primary key ([Category], [Config])
);
GO
