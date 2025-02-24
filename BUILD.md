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
# Releasing
There are bash scripts found in the `/scripts` folder that details out the commands to publish the release files for the project.
Primarily there is the `release.sh` script to publish the release builds, and the `macos.sh` to create the macOS packages. Both of these scripts are used in the 
github release hook.

## release.sh
`/scripts/release.sh` will perform a `dotnet publish` to create a release binary for multiple platforms (`win-x64`, `osx-arm64`, `osx-x64`, `linux-arm64`, `linux-x64`)
the releases are then placed into the `/build/` folder with each platform release under their own respective sub-directory. 

## macos.sh
`/scripts/macos.sh` is a separate script that is meant to be run after the release builds are generated from `/scripts/release.sh`. This script takes the generated release binaries and packages them into a `.app` for macOS
The script itself takes in one of two arguments: `--x64` and `--arm64` to determine which macos package to generate. The output is then placed inside the `/build/` folder as `/build/osx_{platform}_macapp/TableauMigration.app`


