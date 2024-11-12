# hangfire-demo

Hangfire is a job scheduling framework for .NET. It provides a simple way to schedule and execute background tasks. It is a free and open source project. You can find the source code on GitHub: [hangfire](https://github.com/HangfireIO/Hangfire).

This project is a demo of how I used Hangfire, including some intermediate additions.

## Storage

Hangfire can use different kinds of permanent storage. If you're using the prepared devcontainer, an MS SQL Server pod is already running and configured to be used.

Otherwise you need to supply your own SQL storage connection string via configuration.

## Configuration

The configurations are loaded in the following order:
1. appsettings.json
1. appsettings.{DOTNET_ENVIRONMENT}.json
1. Environment variables

