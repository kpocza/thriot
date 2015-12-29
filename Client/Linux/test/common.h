#include "ManagementClient.h"

using namespace Thriot::Management;

#pragma once

// !!! ENSURE TO have thriothost resolved to the correct hostname by adding it to your /etc/hosts file !!!

#define APIURL "http://thriothost/api/v1"
#define PAPIURL "http://thriothost/papi/v1"
#define RAPIURL "http://thriothost/rapi/v1"
#define WSURL "ws://thriothost:8080"

//#define APIURL "https://thriothost/api/v1"
//#define PAPIURL "https://thriothost/papi/v1"
//#define RAPIURL "https://thriothost/rapi/v1"
//#define WSURL "wss://thriothost:8081"

//#define SINKDATA "localazuredata"
//#define SINKTIMESERIES "localazuretimeseries"

//#define PARAMSINKDATA "azuredata"
//#define SINKPARAMCS "UseDevelopmentStorage=true"
//#define SINKPARAMT "LinuxData"

#define SINKDATA "localsqldata"
#define SINKTIMESERIES "localsqltimeseries"

#define PARAMSINKDATA "sqldata"
#define SINKPARAMCS "Server=.\\SQLEXPRESS;Database=ThriotTelemetry;Trusted_Connection=true"
#define SINKPARAMT "LinuxData"

//#define SINKDATA "localpgsqldata"
//#define SINKTIMESERIES "localpgsqltimeseries"

//#define PARAMSINKDATA "pgsqldata"
//#define SINKPARAMCS "Server=127.0.0.1;Port=5432;Database=ThriotTelemetry;User Id=thriottelemery;Password=thriottelemetry"
//#define SINKPARAMT "LinuxData"


struct PlatformTestInput
{
	Device Dev;
};

struct ReportingTestInput
{
	Network Net;
	Device Dev1;
	Device Dev2;
};

string getRandomEmail();
ManagementClient* CreateManagementClient();
PlatformTestInput CreatePlatformTestInput();
ReportingTestInput CreateReportingTestInput();

