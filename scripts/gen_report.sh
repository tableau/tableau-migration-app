#!/usr/bin/bash
sudo dotnet test tests/Tableau.Migration.App.Core.Tests/ /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=../../CoreCoverage.xml
sudo dotnet test tests/Tableau.Migration.App.GUI.Tests /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=../../GUICoverage.xml
reportgenerator -reports:"./*Coverage.xml" -targetdir:./coverage-report
