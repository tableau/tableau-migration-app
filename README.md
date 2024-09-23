# Tableau ESMB Migration App
Tableau express migration application to migrate data sources from Tableau Server to Tableau Cloud.
Requirements doc can be found [here](https://docs.google.com/document/d/1DXrYdTbS5aGcZeicNVAdD1tvGRwtH1Yj/edit#heading=h.gjdgxs).

# Package installation
`dotnet add package {TBD}`
`nutget install {TBD}`

# Repo Structure
```
./src
├── EsmbMigration/
├── EsmbMigration.Console/
├── EsmbMigration.Core/
└── EsmbMigration.GUI/
```
* EsmbMigration - Main app definition
* EsmbMigration.Console - CLI Implementation using CommandLineParser.
* smbMigration.Core - Core library functionality using the Tableau Migration SDK.
* smbMigration.GUI - Gui implementation using Avalonia framework.

# Building
## From Container
The dockerfile is defined in the `Dockerfile` from the project root.
```
docker build .
```
## From Source Root
```
dotnet build EsmbMigrationApp.sln
```

## Documentation
```
cd docs
docfx # Build
docfx --serve # Host the docs locally
```

##  Sub Project Build
Individual sub projects can be built from their respective folders.
```
cd src/EsmbMigrationApp.Console/
dotnet build
dotnet run
```

# Testing
```
dotnet test EsmbMigrationApp.sln
```
