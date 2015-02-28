# Thriot

---

## What is Thriot?

Thriot stands for Rioting things which means that Thriot is an open source IoT Platform with 
Client libraries for such things that are rioting against being stupid and want to connect to Thriot 
to become smart, record telemetry data and collaborate with other things.

## How things are structured?

There is always a tree-like network structure. Devices (things) can be created under any network in the tree.
Devices can send message to any device in the same network and any device can record telemetry data.

## What are the 3 operating modes?

Besides networks and devices you can have services and companies. Here come the following 
3 operating modes (aka. service profile) into the picture (globally configurable):

1. Single service
2. Single company
3. Service provider

The default configutation is *Servie provider* to have all freedom while development.

*Single service* mode is for smart homes and small users with a single service who want to manage 
only a handful of devices. In this mode the sytem has only a single service, just networks 
and devices can be managed. In case of *single company* mode you can create different services for 
different businesses as a company but cannot register multiple companies 
(there is a single company in the system). 
In *service provider* mode any registered user is free to create several companies with any services.

Multiple users can be added to any company (service in single service mode) as coadmins.

## What are the main concepts for the platform?

* Things (devices) can be managed on a website
* The platform functionality can be reached through an API
* The server platform runs on Windows using Microsoft .NET 4.5 
  The aim is to support Linux with .NET Core CLR 
* There is a .NET client library and a C++ client library that currently support Linux on Intel and ARM.
  You can attach Raspberry Pi and Banana Pi based devices to the system 
* Currently incoming telemetry data can be recorded to Azure Table Storage and Microsoft SQL (Express also)
* The M2M messaging is implemented on top of Microsoft SQL 2012+ (Express also) 
* Thriot has an extensible architecture which means that further storage engines can be added 
  to the system to remove Windows dependency 

## What are the main platform components?

* *Management API* - REST API for managing users, companies, services, networks, devices
* *Platform API* - REST API for recording telemetry data and sending/receiving messages
* *Platform WebSocket* - WebSocket API with the same functionality as the Platform API but
  with persistent connection support
* *Reporting API* - REST API with reporting functionality to retrieve device and network level reports
  for telemetry data in JSON and CSV format
* *Management Website* - Web UI for device management and reporting - 
  employs Management API and Reporting API
* *Messaging services* - Background services with SQL storage for M2M messaging

## How do I prepare the development environment?

### Common steps

* Branch/clone the github repository (do not use c:\Thriot by default)
* Use Visual Studio 2013, Azure Emulator 2.5 and Microsoft SQL Express 2014
* Open *Service\IoT.Service.sln* using Visual Studio
* Do a full build in Debug configuration
* Create *IoTMessaging database* in the *.\SQLEXPRESS* instance
* Run *Service\Messaging\Scripts\CreateDB.sql* on the IoTMessaging database
* Create c:\Thriot\log folder. Ensure that the current user has write permission to this folder

#### Further steps to create local Azure development environment

* Do a full build in *DevAzure* configuration
* Run *Service\Misc\IoT.CreateAzureStorage\bin\Debug\IoT.CreateAzureStorage.exe* to create the 
  necessary tables in the Azure Table Storage emulator
* Run all unit tests. Correct any environment issues until every test turns green
* Set the IoT.Web project as the startup project. Hit F5. The website should appear
* Try registering a new user. Play with the website
* Start *Service\Platform\IoT.Platform.WebsocketService\bin\DevAzure\IoT.Platform.WebsocketService.exe*
* Open *Client\DotNet\IoT.Client.DotNet.sln* in an other Visual Studio instance
* Ensure that in app.config the settings under *IIS Express - Dev Azure* are uncommented
* Do a full Debug build
* Run all integration tests. Correct any environment issues until every test turns green.

#### Further steps to create local SQL Express-based development environment

* Do a full rebuild in *DevSql* configuration
* Create two databases in the *.\SQLEXPRESS* instance: IoT, IoTTelemetry
* Run *Service\Misc\IoT.CreateSqlStorage\bin\Debug\IoT.CreateSqlStorage.exe* to create the 
  necessary tables in *IoT* database in the local SQL Express 2014
* Run all unit tests. Correct any environment issues until every test turns green
