#!/usr/bin/env bash

SCRIPT_DIR=$(dirname "$(realpath "$0")")
PROJECT_DIR=$(dirname "$SCRIPT_DIR")

dotnet publish ./src/Tableau.Migration.App.GUI -c Release -r win-x64 --self-contained /p:PublishSingleFile=true /p:DebugType=None /p:GenerateDocumentationFile=false -o ${PROJECT_DIR}/build/win_x64

dotnet publish ./src/Tableau.Migration.App.GUI -c Release -r osx-arm64 --self-contained -p:PublishSingleFile=true /p:DebugType=None /p:GenerateDocumentationFile=false -o ${PROJECT_DIR}/build/osx_arm64

dotnet publish ./src/Tableau.Migration.App.GUI -c Release -r osx-x64 --self-contained -p:PublishSingleFile=true /p:DebugType=None /p:GenerateDocumentationFile=false -o ${PROJECT_DIR}/build/osx_x64

dotnet publish ./src/Tableau.Migration.App.GUI -c Release -r linux-arm64 --self-contained -p:PublishSingleFile=true /p:DebugType=None /p:GenerateDocumentationFile=false -o ${PROJECT_DIR}/build/linux_arm64

dotnet publish ./src/Tableau.Migration.App.GUI -c Release -r linux-arm64 --self-contained -p:PublishSingleFile=true /p:DebugType=None /p:GenerateDocumentationFile=false -o ${PROJECT_DIR}/build/linux_x64
