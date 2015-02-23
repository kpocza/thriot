#include "gtest/gtest.h"
#include "ManagementClient.h"
#include "common.h"

TEST(TelemetryMetadataTest, get)
{
	ManagementClient*  managementClient = CreateManagementClient();

	TelemetryDataSinksMetadataClient* telemetryDataSinksMetadataClient = managementClient->TelemetryDataSinksMetadata();
	TelemetryDataSinksMetadata telemetryDataSinksMetadata = telemetryDataSinksMetadataClient->Get();

	ASSERT_TRUE(telemetryDataSinksMetadata.Incoming.size() > 0);
}

TEST(TelemetryMetadataTest, updateIncomingMetadata)
{
	ManagementClient*  managementClient = CreateManagementClient();
	CompanyManagementClient* companyClient = managementClient->Company();
	ServiceManagementClient* serviceClient = managementClient->Service();
	NetworkManagementClient* networkClient = managementClient->Network();
	Company company;
	company.Name = "test company";
	string companyId = companyClient->Create(company);
	Service service;
	service.CompanyId = companyId;
	service.Name = "test service";
	string serviceId = serviceClient->Create(service);
	Network network;
	network.CompanyId = companyId;
	network.ServiceId = serviceId;
	network.Name = "test network";
	string networkId = networkClient->Create(network);

	vector<TelemetryDataSinkParameters> telemetryDataSinkParameters;
	TelemetryDataSinkParameters tdsp1;
	tdsp1.SinkName = SINKDATA;
	TelemetryDataSinkParameters tdsp2;
	tdsp2.SinkName = SINKTIMESERIES;
	telemetryDataSinkParameters.push_back(tdsp1);
	telemetryDataSinkParameters.push_back(tdsp2);

	int retCompany = companyClient->UpdateIncomingTelemetryDataSinks(companyId, telemetryDataSinkParameters);
	int retService = serviceClient->UpdateIncomingTelemetryDataSinks(serviceId, telemetryDataSinkParameters);
	int retNetwork = networkClient->UpdateIncomingTelemetryDataSinks(networkId, telemetryDataSinkParameters);
	ASSERT_EQ(0, retCompany);
	ASSERT_EQ(0, retService);
	ASSERT_EQ(0, retNetwork);

	company = companyClient->Get(companyId);
	service = serviceClient->Get(serviceId);
	network = networkClient->Get(networkId);

	ASSERT_EQ(2, company.TelemetryDataSinkSettings.Incoming.size());
	ASSERT_EQ(SINKDATA, company.TelemetryDataSinkSettings.Incoming[0].SinkName);

	ASSERT_EQ(2, service.TelemetryDataSinkSettings.Incoming.size());
	ASSERT_EQ(SINKDATA, service.TelemetryDataSinkSettings.Incoming[0].SinkName);

	ASSERT_EQ(2, network.TelemetryDataSinkSettings.Incoming.size());
	ASSERT_EQ(SINKDATA, network.TelemetryDataSinkSettings.Incoming[0].SinkName);
}

TEST(TelemetryMetadataTest, updateIncomingMetadataWithParams)
{
	ManagementClient*  managementClient = CreateManagementClient();
	CompanyManagementClient* companyClient = managementClient->Company();
	Company company;
	company.Name = "test company";
	string companyId = companyClient->Create(company);

	vector<TelemetryDataSinkParameters> telemetryDataSinkParameters;
	TelemetryDataSinkParameters tdsp;
	tdsp.SinkName = PARAMSINKDATA;
	tdsp.Parameters["ConnectionString"] = SINKPARAMCS;
	tdsp.Parameters["Table"] = SINKPARAMT;
	telemetryDataSinkParameters.push_back(tdsp);

	int retCompany = companyClient->UpdateIncomingTelemetryDataSinks(companyId, telemetryDataSinkParameters);
	ASSERT_EQ(0, retCompany);

	company = companyClient->Get(companyId);

	ASSERT_EQ(1, company.TelemetryDataSinkSettings.Incoming.size());
	ASSERT_EQ(PARAMSINKDATA, company.TelemetryDataSinkSettings.Incoming[0].SinkName);
	ASSERT_EQ(2, company.TelemetryDataSinkSettings.Incoming[0].Parameters.size());
	ASSERT_EQ(SINKPARAMCS, company.TelemetryDataSinkSettings.Incoming[0].Parameters["ConnectionString"]);
	ASSERT_EQ(SINKPARAMT, company.TelemetryDataSinkSettings.Incoming[0].Parameters["Table"]);
}

