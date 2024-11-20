# Tableau Migration App
Tableau express migration application to migrate data sources from Tableau Server to Tableau Cloud.
Requirements doc can be found [here](https://docs.google.com/document/d/1DXrYdTbS5aGcZeicNVAdD1tvGRwtH1Yj/edit#heading=h.gjdgxs).

# Repo Structure
```
./src
├── Tableau.Migration.App.Core/
└── Tableau.Migration.App.GUI/
```
* Tableau.Migration.App.Core - Core library functionality using the Tableau Migration SDK.
* Tableau.Migration.App.GUI - Gui implementation using Avalonia framework.

# Building
```
## From Source Root
```
dotnet build Tableau.Migration.App.sln
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
cd src/Tableau.Migration.App.GUI/
dotnet build
dotnet run
```

# Testing
```
dotnet test Tableau.Migration.App.sln
```
