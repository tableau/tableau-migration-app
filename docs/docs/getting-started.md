# Getting Started
## Dependencies
The project is coded in C# using Dotnet.
To build and run this project, you will need to have .NET 8.0 installed.

## Building The Project
The project can be loaded into Visual Studio using the solution present in the repo.

Otherwise, it can also  be built using the following commands in a terminal instance:
```
# From the project root folder
$ dotnet restore # download needed dependencies for the project
$ dotnet build # this will build both the Core and GUI modules.
```

You can also go into each project folder in `src` and build a singular project.
The GUI module can also be run:
```
# From /src/Tableau.Migration.App.GUI/
$ dotnet run # Will build and then run the app.
```

## Running Unit Tests
Tests can be run either through Visual Studio, or by the following terminal command:
```
# From either the project root, or /src/Tableau.Migration.App.<Core | GUI>/
$ dotnet test
```

## Creating Packages
Packages can be created using `dotnet publish` commands. A script named `release.sh` can be found in the `scripts/` folder that holds the command to build a publishing target for
* Win x64
* OSX ARM64
* OSX x64
* Linux x64
* Linux ARM64

*note* For MacOs, there is an additional `/script/macos.sh` script to build the `.app` package by creating the appropriate fodler structure using the published files and the `Info.plist` found in `/src/Tableau.Migration.App.GUI/Assets/`.
