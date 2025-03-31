#Requires -Version 5
<#
.SYNOPSIS
    Common build script helper functions.
    Intended to be dot sourced into the build script.

.DESCRIPTION
    Author: Blake Anderton, Joshua Poehls
#>

param(
    [switch] $Trace
)

$ErrorActionPreference = 'Stop'
# https://stackoverflow.com/questions/28682642/powershell-why-is-using-invoke-webrequest-much-slower-than-a-browser-download
$ProgressPreference = 'SilentlyContinue'
Set-StrictMode -Version Latest
if ($Trace) {
    Set-PSDebug -Strict -Trace 1
}

function Sign-File {
    [OutputType([void])]
    param(
        [string] $Path,
        [bool] $SignRemote
    )

    Write-Host "Sign File $path SignRemote $SignRemote"
    if ($SignRemote) {
        signRemote $Path
    }
    else {
        signLocal $Path
    }
}

function getCommandPath {
    [OutputType([string])]
    param(
        [Parameter(Mandatory = $true)]
        [string] $ExecutableName,
        [Parameter(Mandatory = $true)]
        [string[]] $OtherDirs 
    )

    foreach ($dir in , '' + $OtherDirs) {
        $path = if ($dir.Length -eq 0) { $ExecutableName } else { "$dir\$ExecutableName" }
        $fullPath = Get-Command -ErrorAction Ignore -Name $path
        if ($null -ne $fullPath) {
            return $fullPath.Source
        }
    }

    throw "Could not get path to executable $ExecutableName"
}

function getSignToolPath {
    [OutputType([string])]
    param()

    getCommandPath -ExecutableName signtool.exe -OtherDirs @(
        "${env:ProgramFiles(x86)}\Windows Kits\8.1\bin\x86"
        "${env:ProgramFiles(x86)}\Windows Kits\10\bin\10.0.22621.0\x86"
    )
}

function signLocal {
    [OutputType([void])]
    param(
        [string] $Path
    )

    $signToolPath = getSignToolPath

    # A rfc3161 timestamp server is needed if we are to sign the file with a timestamp.
    $timestampServer = "http://timestamp.entrust.net/TSS/RFC3161sha2TS"

    # CERTIFICATE_ALIAS must be set to a valid alias in GitLab CI/CD environment.
    # The corresponding certificate must be located at c:\cert\<CERTIFICATE_ALIAS>_cert.pfx in Docker container.
    # If this environment variable is not set then sign using Windows certificate store with current user's certificate.
    $certAlias = $env:CERTIFICATE_ALIAS
    if ($certAlias -eq $null) {
        & $signToolPath sign /fd SHA256 /td SHA256 /tr $timestampServer $Path
    }
    else {
        & $signToolPath sign /f $certPath /p $certPassword /fd SHA256 /td SHA256 /tr $timestampServer $Path
    }

    $exitStatus = $LastExitCode
    if ($exitStatus -ne 0) {
        throw "SignTool failed with status $exitStatus"
    }
}

function signRemote {
    [OutputType([void])]
    param(
        [string] $Path,
        [string] $SessionId = (& { if ($env:CI_JOB_ID -eq $null) { [Guid]::NewGuid().ToString('D') } else { $env:CI_JOB_ID } }),
        [string] $TeamcityApiKey = $env:TEAMCITY_APIKEY,
        [string] $ArtifactoryApiKey = $env:ARTIFACTORY_APIKEY,
        [string] $ArtifactoryUrl = (& { if ($env:ARTIFACTORY_URL -eq $null) { "https://artifactory.prod.tableautools.com/artifactory" } else { $env:ARTIFACTORY_URL } }),
        [string] $TeamcityUrl = (& { if ($env:TeamcityUrl -eq $null) { "https://teamcity.prod.tableautools.com" } else { $env:TeamcityUrl } })
    )

    $fileName = Split-Path -Path $Path -Leaf -Resolve
    $baseUri = "$ArtifactoryUrl/content-migration/teamcity-sign-service/$SessionId"
    Write-Host "Upload $baseUri"

    $artifactHeaders = @{
        "X-JFrog-Art-Api" = $ArtifactoryApiKey
        "X-Checksum-Md5"  = (Get-FileHash $Path -Algorithm MD5).hash.ToLower()
        "X-Checksum-Sha1" = (Get-FileHash $Path -Algorithm SHA1).hash.ToLower()
        "Content-Type"    = "application/octet-stream"
    }

    try {
        Write-Host "Upload $Path"
        Invoke-RestMethod -inFile $Path -Method PUT -Uri "$baseUri/input/$fileName" -Headers $artifactHeaders
    }
    catch {
        $_.Exception | Format-List -Force
        throw "Remoted sign failed with $_."
    }

    $tcHeaders = @{
        "Authorization" = "Bearer $TeamcityApiKey"
        "Content-Type"  = "application/xml"
    }

    $dataBody = "<build><buildType id='PowerTools_TeamcitySignService_SignBinaries'/><properties><property name='Session' value='$SessionId'/></properties></build>"
    $r = Invoke-RestMethod -Uri "$TeamcityUrl/app/rest/buildQueue" -Method POST -Headers $tcHeaders -Body $dataBody
    $jobData = ([xml]$r)
    $buildId = $JobData.build.id
    Write-Host "Started signing - Teamcity Build: Id $buildId"

    while ($true) {
        try {
            $r = Invoke-RestMethod  -Uri "$TeamcityUrl/app/rest/builds?locator=id:$BuildId" -Method GET -Headers $tcHeaders
            $stateData = ([xml]$r)
            $state = $stateData.builds.build.state
            if ($state -eq "finished") {
                $status = $stateData.builds.build.status
                break
            }
        } 
        catch {
            # Dig into the exception to get the Response details.
            # Note that value__ is not a typo.
            Write-Host "StatusCode:" $_.Exception.Response.StatusCode.value__
            Write-Host "StatusDescription:" $_.Exception.Response.StatusDescription
        }
        Start-Sleep -s 5
    }

    Write-Host "Final status $status"

    if ($status -eq "SUCCESS") {
        Invoke-RestMethod -outFile $Path -Method GET -Uri "$baseUri/output/$fileName" -Headers $artifactHeaders
    }
    else {
        throw "Remoted sign failed with $status"
    }
}
