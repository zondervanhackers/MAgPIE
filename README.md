# MAgPIE

## Getting Started

MAgPIE (Metrics Aggregation Processing & Intake Engine) is an ETL system developed by Zondervan Library of Taylor University with the help of librarians and student workers. The main entry point for the system is the windows service (Harvester) that handles ingesting data and operation scheduling. When installed the Harvester will generate a configuration file and wait until a complete version is present to begin operations.

## Prerequisites

    Windows OS (Tested on Windows 10,8,7, Server 2012)
    SQL SERVER 2012
    MS Build Tools 2017
    .Net Framework 4.5 & 4.6

## Installing

We use the Cake (C# Make) to handle the building of the project, installation of the windows service, set up of the database it will be using to store the data. A powershell (.ps1) file is located in the root directory "build.ps1" and running it will completely install and deploy MAgPIE though this can be changed by modifying the file or passing different command line arguments.

## High Level Explanations

[Harvester Service](wiki/Harvester-Service.md)

[Configuration File Specifications](wiki/models/Configuration-File-Specifications.md)

[Operation Implementations](wiki/models/Operation-Logic.md)

### Statistics Database

[Circulation](wiki/models/Statistics-Database/Circulation-Database.md)

[Digital](wiki/models/Statistics-Database/Digital-Database.md)

[EZProxy](wiki/models/Statistics-Database/EZProxy-Database.md)

[Patron](wiki/models/Statistics-Database/Patron-Database.md)

[Statista](wiki/models/Statistics-Database/Statista-Database.md)

### Harvester Database

[Directory Records](wiki/models/Harvester-Database/Directory-Records.md)

[Operation Records](wiki/models/Harvester-Database/Operation-Records.md)

## Info for Developers

[Development Configuration](wiki/Development-Configuration.md)