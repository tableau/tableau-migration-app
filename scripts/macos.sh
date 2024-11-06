#!/usr/bin/env bash

# Default architecture
ARCH=""
MAC_APP_SUFFIX=""

# Parse command line arguments
while [[ "$1" != "" ]]; do
    case $1 in
	--x64 )   ARCH="osx_x64"
		  MAC_APP_SUFFIX="osx_x64_macapp"
		  ;;
	--arm64 ) ARCH="osx_arm64"
		  MAC_APP_SUFFIX="osx_arm64_macapp"
		  ;;
	* )       echo "Usage: $0 [--x64 | --arm64]"
		  exit 1
    esac
    shift
done

# If no architecture is specified, exit with a message
if [ -z "$ARCH" ]; then
    echo "Error: No architecture specified. Use --x64 or --arm64."
    exit 1
fi

APP_NAME="./build/${MAC_APP_SUFFIX}/TableauMigration.app"
SCRIPT_DIR=$(dirname "$(realpath "$0")")
PROJECT_DIR=$(dirname "$SCRIPT_DIR")

# Use the chosen architecture
PUBLISH_OUTPUT_DIRECTORY="${PROJECT_DIR}/build/${ARCH}/."

INFO_PLIST="${PROJECT_DIR}/src/Tableau.Migration.App.GUI/Assets/Info.plist"
ICON_FILE="tableau-migration-app.icns"
ICON_PATH="${PROJECT_DIR}/src/Tableau.Migration.App.GUI/Assets/${ICON_FILE}"

if [ -d "$APP_NAME" ]
then
    rm -rf "$APP_NAME"
fi

mkdir -p "$APP_NAME"

mkdir -p "$APP_NAME/Contents"
mkdir -p "$APP_NAME/Contents/MacOS"
mkdir -p "$APP_NAME/Contents/Resources"

cp "$INFO_PLIST" "$APP_NAME/Contents/Info.plist"
cp "$ICON_PATH" "$APP_NAME/Contents/Resources/${ICON_FILE}"
cp -a "$PUBLISH_OUTPUT_DIRECTORY" "$APP_NAME/Contents/MacOS"
