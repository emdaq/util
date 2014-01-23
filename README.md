Emdaq Utilities
==========

Emdaq.Util has basic utilties.

Emdaq.DataAccess has data access utilties for a service/repository pattern, wrapping Dapper.NET. The full benefit of this setup can be seen at the service level (which is not here), with cross-repo and cross-service transactions.

## Getting Started

Requirements:
* Windows machine
* .NET 4.0
* Visual Studio
* MySQL 5.6.x ([newest version](http://dev.mysql.com/downloads/mysql/) is fine)

Optional:
* Nuget (used for dependencies, which are all in /packages right now)

## Example Repository

For an example repository using the pattern, see https://github.com/emdaq/util/blob/master/Emdaq.ExampleDataAccess/Repository/EmailRepo.cs
