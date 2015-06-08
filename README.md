# Thriot

---

## What is Thriot?

Thriot stands for *TH*ings that are *RIOT*ing which means that Thriot is an open source IoT Platform with Client libraries for such things that are rioting against being stupid and want to get connected to Thriot to become smart, record telemetry data and collaborate with other things.

## Thriot as an IoT Platform

There are several IoT platforms on the market. Most of them aims to handle several million devices with low latency of course on top of a huge infrastructure running in the cloud or on-premise DCs. The operators and developers of  these systems are so called service providers. Furthermore these solutions are generally proprietary pieces of software so they are mainly closed source.

## How Thriot is different from that?

The following facts make Thriot special:
- The main goal of Thriot is to satisfy the IoT needs of smart home users and individual companies so that they can install Thriot on their own and it’s not necessary to buy an IoT service from a service provider. Of course there is the possibility for service providers to provide Thriot as a SaaS solution (although it’s not yet recommended).
- Thriot achieves this flexibility with its loosely coupled plugable architecture so that besides cloud storage providers, on-premise storages can be also added to the system. (Currently Azure Table Storage, MS SQL (Express) and PostgreSql is supported)
- Thriot is completely open source. Everybody is welcome the contribute. 

## Basic concepts and architecture

You can read about basic concepts here: http://portal.thriot.io/basic-concepts/ while a high level architectural description can be found here: http://portal.thriot.io/architecture/

## Development environmnet - API, client library documentation

Currently Thriot Service is developed using .NET 4.5 and Visual Studio and runs on Windows (we have experimental support for Linux). However there is a C++ client for connecting Linux-based devices and microcomputers (Raspberry Pi 1/2, Banana Pi, etc) and of course we have a .NET-based client also.

Page http://portal.thriot.io/service-development-environment/ describes the steps creating Service development environment. http://portal.thriot.io/net-test-environment/ goes a step further and it describes how to create a fully functional test environment for the system. Page http://portal.thriot.io/linux-client-development-environment/ has some details on creating a Linux-based client development environment. 

Thriot has a REST-based and a Websocket-based API. http://portal.thriot.io/api-reference/ is the root page for all API documentation exposed by the Thriot service.

http://portal.thriot.io/net-client-library-reference/ describes the basic concepts of the .NET library. http://portal.thriot.io/linux-c-client-library-reference/ is for the C++ library that runs under Linux.

## Hosting environments

Currently Thirot Service is fully supported on Windows-based hosting environments and store data in Azure Table Storagem in Microsoft SQL (Express) or PostgreSQL. 

There is experimental level support for Linux (tested on Ubuntu 14.04.2 LTS) based hosting with PostgreSQL backend. Linux hosting currently employs mostly Mono and a little bit of ASP.NET 5.

Page http://portal.thriot.io/sql-backed-hosting-environment/ describes the steps to create a SQL-backed hosting environment while page http://portal.thriot.io/azure-environment/ describes the steps of how to create a hosting environment in Azure with ATS backend storage.
Page http://portal.thriot.io/postgresql-backed-hosting-environment/ described the PostgreSQL based hosting environment on Windows while http://portal.thriot.io/test-environment-on-linux/ defines how to install Thriot on Ubuntu 14.04.2 LTS.

The aim is to provide full hosting capabilities on Linux using completely ASP.NET 5 on CoreCLR.
