# Entity Framework Core: Getting Started

This is based on the Pluralsight course, [Entity Framework Core: Getting Started](https://app.pluralsight.com/library/courses/entity-framework-core-get-started), but modified slightly to work more nicely with .NET 6 and in IDE/editors other than Visual Studio, in particular VS Code.

Rather than attaching a database to the project directly, which would be relatively normal in Visual Studio, this connects to a local MS SQL server which is handled by Docker via Docker Compose.

## Requirements

* .NET 6.0 SDK
* Docker (configured to be able to run Linux containers)

## Getting Started

For managing the database, there are a few commands which are useful to have to hand:

* To start up the database server the first time and watch the logs:
    ```
    docker compose up
    ```
* To start up the database server the first time and not watch the logs:
    ```
    docker compose up -d
    ```
* After the database server has been started once, but is currently stopped and you want to restart it:
    ```
    docker compose start
    ```
* If the database server is currently running and you want to stop it:
    ```
    docker compose stop
    ```
* If you want to tear down the database server (i.e. stop it if it's running and remove it entirely):
    ```
    docker compose down
    ```

## Notes

* There is a `.env` file in the root directory which contains the environment variables for the MS SQL Server, this may not normally be under source control as it contains secrets (or it may be one of many environment variables files, of which one or more may not be under source control for similar reasons)
* Currently, the connection string is hardcoded in the `SamuraiApp.Data` project, in the `SamuraiContext.cs` file, as per the course. This contains the password from the `.env` file again, so **if you want to change the password for your MS SQL Server, you will need to change this in both places**. This is also the case for the port, if you want to change that. Ideally, this would be generated based on the environment variables, but as this is a training course this is not necessary at this point.