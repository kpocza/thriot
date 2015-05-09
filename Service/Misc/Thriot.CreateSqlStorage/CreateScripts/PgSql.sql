CREATE OR REPLACE FUNCTION CreateDatabase() RETURNS int AS $$
-- BEGIN: InitialDatabase
BEGIN
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND lower(TABLE_NAME)=lower('Setting')) THEN
		create table "Company" (
			"Id" char(32) primary key not null,
			"Name" varchar(50) not null,
			"TelemetryDataSinkSettingsJson" varchar(2048) null
		);
		create table "Device" (
			"Id" char(32) primary key not null,
			"Name" varchar(50) not null,
			"DeviceKey" varchar(32) not null,
			"NumericId" bigint not null,
			"CompanyId" char(32) not null,
			"NetworkId" char(32) not null,
			"ServiceId" char(32) not null
		);
		create table "LoginUser" (
			"Email" varchar(128) primary key not null,
			"PasswordHash" varchar(64) not null,
			"Salt" varchar(32) not null,
			"UserId" char(32) not null
		);
		create table "Network" (
			"Id" char(32) primary key not null,
			"Name" varchar(50) not null,
			"NetworkKey" varchar(32) not null,
			"TelemetryDataSinkSettingsJson" varchar(2048) null,
			"ParentNetworkId" char(32) null,
			"CompanyId" char(32) not null,
			"ServiceId" char(32) not null
		);
		create table "Service" (
			"Id" char(32) primary key not null,
			"Name" varchar(50) not null,
			"ApiKey" varchar(32) not null,
			"TelemetryDataSinkSettingsJson" varchar(2048) null,
			"CompanyId" char(32) not null
		);
		create table "Setting" (
			"Category" varchar(32) not null,
			"Config" varchar(32) not null,
			"Value" varchar(2048) not null,
			primary key ("Category", "Config")
		);
		create table "User" (
			"Id" char(32) primary key not null,
			"Name" varchar(50) not null,
			"Email" varchar(128) not null,
			"Activated" boolean not null,
			"ActivationCode" varchar(8000) null
		);
		create table "UserCompany" (
			"UserId" char(32) not null,
			"CompanyId" char(32) not null,
			primary key ("UserId", "CompanyId")
		);
		alter table "Device" add constraint Device_Company foreign key ("CompanyId") references "Company"("Id");
		alter table "Device" add constraint Device_Network foreign key ("NetworkId") references "Network"("Id");
		alter table "Device" add constraint Device_Service foreign key ("ServiceId") references "Service"("Id");
		alter table "Network" add constraint Network_ChildNetworks foreign key ("ParentNetworkId") references "Network"("Id");
		alter table "Network" add constraint Network_Company foreign key ("CompanyId") references "Company"("Id");
		alter table "Network" add constraint Network_Service foreign key ("ServiceId") references "Service"("Id");
		alter table "Service" add constraint Service_Company foreign key ("CompanyId") references "Company"("Id");
		alter table "UserCompany" add constraint User_Companies_Source foreign key ("UserId") references "User"("Id");
		alter table "UserCompany" add constraint User_Companies_Target foreign key ("CompanyId") references "Company"("Id");

		INSERT INTO "Setting"("Category", "Config", "Value") VALUES('Version', 'Database', '1');
	END IF;
RETURN 0;
END
$$ LANGUAGE plpgsql;
-- END: InitialDatabase

SELECT * FROM CreateDatabase()