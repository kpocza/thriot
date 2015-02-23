#pragma once

#define APIURL "http://avantasia/api/v1"
#define PAPIURL "http://avantasia/papi/v1"
#define RAPIURL "http://avantasia/rapi/v1"
#define WSURL "ws://avantasia:8080"

#define SINKDATA "localazuredata"
#define SINKTIMESERIES "localazuretimeseries"

#define PARAMSINKDATA "azuredata"
#define SINKPARAMCS "UseDevelopmentStorage=true"
#define SINKPARAMT "LinuxData"

class ManagementClient;
class OcassionalConnectionClient;

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

