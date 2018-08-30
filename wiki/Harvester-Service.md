# Basics

The **Harvester Service** is a windows service that **harvests Library usage and Demographic data** and publishes it to a database.

It's base Directory will located on the root folder of whichever base drive it was installed on, e.g. "C:\\Harvester\\".

## Log FileName Format

### "(OperationResult) OperationName (Date) GUID.txt"

### Examples

![LogFormat](LogFormat.png)

### Explanation

The directory "\\Harvester\\Logs\\" will store a log file for each operation run.

**Operation Result** will be either be "S" for Success, "F" for Failure, "C" for Cancel.

**Operation Name** will be the Name given to an operation in the configuration file.

**Date** will be a Run Date in the format "MM-DD-YYYY".

**GUID** or Globally Unique Identifiers are generated at the start of the operation.

## Configuration

Operations are scheduled in a Configuration File named "Configuration.xml" in the "\\Harvester\\" directory.

If the configuration file contains errors the Service will stop functioning and email either the Admin, Developer, or User for specific reasons. See example at [Configuration File Specifications](models/Configuration-File-Specifications.md)

## Databases

The Harvester uses two databases: **Harvester Database** and **Statistics Database**.

The **Statistics Database** contains the Demographic and Library Usage data.

The **Harvester Database** contains data regarding successful operations and files ingested as well as which repositories have been used.