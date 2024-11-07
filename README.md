# Distillery.RapidPay.Assestment

This project was created to support the requirements of the RapidPay assestment. 
Is an ASP.NET Core web application built using .NET 8 and C# that supports user registration, JWT authorization and has 3 Credit Card related endpoints.
The WebAPI uses Clean Architecture design as well as Mediator pattern for decoupling of the Web Service and the core logic.
It supports SQL Server for higher environments as well as SQLite for local testing if a SQL Server instance is not available.
A proper Docker configuration is enable but it will required Docker Desktop in order to be used, so running in HTTPS for local testing is recommended.

### Main Technologies:

- .NET 8
- ASP.NET Core
- SQL Server
- SQLite

### Dependencies

- FluentValidation
- MediatR
- Identity
- JWT Authentication
- Ardalis.Result
- Automapper
- Entity Frameworks

### Setup

To setup the application just download the code, restore the dependencies and build.

#### SQLite

- No additional setup required if the included Sqlite DB is used.

#### SQL Server

- Set the ConnectionString "RapidPaySqlServer" to a valid instance.
- Run the migrations using:
  - PM: "update-database -Context SqlServerDbContext".
  - CLI: "dotnet ef database update --context SqlServerDbContext".

### AppSettings

Most configuration values can be set as default for local development if using SQLite, but it is recommend to review the following ones:

- ConnectionStrings
- UseSqlite

### Notes

- Due to time constrains no UTs were implemented.
- The SQLite.DB included in the build is already setup and includes some test records.
