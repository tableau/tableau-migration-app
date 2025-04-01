$scriptsDir = $PSScriptRoot

. (Join-Path -Path $PSScriptRoot -ChildPath build-helpers.ps1)
Sign-File -Path (Resolve-Path ./build/win_x64/TableauMigrationApp.exe) -SignRemote $true
