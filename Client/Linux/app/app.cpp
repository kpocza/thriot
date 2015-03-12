#include "ManagementClient.h"
#include "PlatformClient.h"
#include <time.h>
#include <stdio.h>
#include <iostream>
#include <unistd.h>

using namespace std;

void messageReceived(const PushedMessage& pushedMessage)
{
	cout << "received: " + pushedMessage.Payload << endl;
}

// !!! ENSURE TO have thriothost resolved to the correct hostname by adding it to your /etc/hosts file !!!

int main()
{
	ManagementClient*  managementClient = new ManagementClient("http://thriothost/api/v1");

	UserManagementClient* userManagementClient = managementClient->User();

	char email[100];

	sprintf(email, "Linuxemail%d@gmail.com", (int)time(NULL));

	RegisterInfo reg;
	reg.Name = "Linux test user";
	reg.Email = email;
	reg.Password = "P@ssw0rd";
	int regRetcode = userManagementClient->Register(reg);

	cout << "Registration return code: " << regRetcode << endl;

	CompanyManagementClient* companyClient = managementClient->Company();
	ServiceManagementClient* serviceClient = managementClient->Service();
	NetworkManagementClient* networkClient = managementClient->Network();
	DeviceManagementClient* deviceClient = managementClient->Device();
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
	Device device1;
	device1.CompanyId = companyId;
	device1.ServiceId = serviceId;
	device1.NetworkId = networkId;
	device1.Name = "test device1";
	string device1Id = deviceClient->Create(device1);
	Device device2;
	device2.CompanyId = companyId;
	device2.ServiceId = serviceId;
	device2.NetworkId = networkId;
	device2.Name = "test device2";
	string device2Id = deviceClient->Create(device2);

	service = serviceClient->Get(serviceId);
	cout << "preparing to fork" << endl;
	pid_t pid = fork();

	if(pid == -1)
		return 1;

	cout << "pid" << pid <<  endl;
	if(pid == 0)
	{
		PersistentConnectionClient*  persistentConnectionClient = new PersistentConnectionClient();
		persistentConnectionClient->Login("ws://thriothost:8080", device1Id, service.ApiKey);

		while(true)
		{
			char data[128];
			sprintf(data,"message sent to the other device: %d", (int)time(NULL)); 
			persistentConnectionClient->SendMessageTo(device2Id, data);
			cout << "sent: " << data << endl;
			usleep(100*1000);
		}
	}
	else
	{
		PersistentConnectionClient*  persistentConnectionClient = new PersistentConnectionClient();
		persistentConnectionClient->Login("ws://thriothost:8080", device2Id, service.ApiKey);
		persistentConnectionClient->Subscribe(ReceiveAndForget, messageReceived);

		while(true)
		{
			persistentConnectionClient->Spin();
		}
	}
}
