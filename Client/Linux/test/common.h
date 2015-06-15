#include "ManagementClient.h"

using namespace Thriot::Management;

#pragma once

// !!! ENSURE TO have thriothost resolved to the correct hostname by adding it to your /etc/hosts file !!!

#define APIURL "http://thriothost/api/v1"
#define PAPIURL "http://thriothost/papi/v1"
#define RAPIURL "http://thriothost/rapi/v1"
#define WSURL "ws://thriothost:8080"

//#define SINKDATA "localazuredata"
//#define SINKTIMESERIES "localazuretimeseries"

//#define PARAMSINKDATA "azuredata"
//#define SINKPARAMCS "UseDevelopmentStorage=true"
//#define SINKPARAMT "LinuxData"

//#define SINKDATA "localsqldata"
//#define SINKTIMESERIES "localsqltimeseries"

//#define PARAMSINKDATA "sqldata"
//#define SINKPARAMCS "Server=.\\SQLEXPRESS;Database=IoTTelemetry;Trusted_Connection=true"
//#define SINKPARAMT "LinuxData"

#define SINKDATA "localpgsqldata"
#define SINKTIMESERIES "localpgsqltimeseries"

#define PARAMSINKDATA "pgsqldata"
#define SINKPARAMCS "Server=127.0.0.1;Port=5432;Database=Thriot;User Id=thriot;Password=thriot"
#define SINKPARAMT "LinuxData"


struct PlatformTestInput
{
	ManagementClient* ManagementCli;
	Device Dev;
};

struct ReportingTestInput
{
	ManagementClient* ManagementCli;
	Network Net;
	Device Dev1;
	Device Dev2;
};

string getRandomEmail();
ManagementClient* CreateManagementClient();
PlatformTestInput CreatePlatformTestInput();
ReportingTestInput CreateReportingTestInput();

