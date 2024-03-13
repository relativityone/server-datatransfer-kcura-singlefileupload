FormatTaskName "------- Executing Task: {0} -------"

properties {
    $SourceDir = Join-Path $PSScriptRoot "source"
    $Solution = ((Get-ChildItem -Path $SourceDir -Filter *.sln -File)[0].FullName)
    $ArtifactsDir = Join-Path $PSScriptRoot "Artifacts"
    $LogsDir = Join-Path $ArtifactsDir "Logs"
    $LogFilePath = Join-Path $LogsDir "buildsummary.log"
    $ErrorLogFilePath = Join-Path $LogsDir "builderrors.log"
}

Task default -Depends Analyze, Compile, Test, Package -Description "Build and run unit tests. All the steps for a local build.";

Task Analyze -Description "Run build analysis" {
    # Leaving this blank until we are ready to add in analyzers later
}

Task NugetRestore -Description "Restore the packages needed for this build" {
    exec { & $NugetExe @('restore', $Solution) }
}

Task Compile -Depends NugetRestore -Description "Compile code for this repo" {
    Initialize-Folder $ArtifactsDir -Safe
    Initialize-Folder $LogsDir -Safe

    exec { msbuild @($Solution,
        ("/target:Build"),
        ("/property:Configuration=$BuildConfig"),
        ("/consoleloggerparameters:Summary"),
        ("/property:PublishWebProjects=True"),
        ("/nodeReuse:False"),
        ("/maxcpucount"),
        ("/nologo"),
        ("/fileloggerparameters1:LogFile=$LogFilePath"),
        ("/fileloggerparameters2:errorsonly;LogFile=$ErrorLogFilePath"))
    }
}

Task Test -Description "Run tests that don't require a deployed environment." {
    $LogPath = Join-Path $LogsDir "UnitTestResults.xml"
    Invoke-Tests -WhereClause "namespace !~ FunctionalTests" -OutputFile $LogPath -WithCoverage
}

Task FunctionalTest -Description "Run functional tests that require a deployed environment." {
    $LogPath = Join-Path $LogsDir "FunctionalTestResults.xml"
    Invoke-Tests -WhereClause "namespace =~ FunctionalTests && TestExecutionCategory == CI" -OutputFile $LogPath -TestSettings (Join-Path $PSScriptRoot FunctionalTestSettings)
}

Task Sign -Description "Sign all files" {
    Get-ChildItem $SourceDir -recurse | Where-Object {$_.Directory.Name -eq "bin" -and @(".dll",".msi",".exe") -contains $_.Extension} | Select-Object -expand FullName | Set-DigitalSignature -ErrorAction Stop
}

Task Package -Description "Package up the build artifacts" {
    $buildTools = Join-Path $PSScriptRoot "buildtools"
    $developmentScripts = Join-Path $PSScriptRoot "DevelopmentScripts"
    $RAPBuilder = Join-Path $buildTools "Relativity.RAPBuilder\tools\Relativity.RAPBuilder.exe"
    $BuildXML = Join-Path $developmentScripts "build.xml"

    exec { & $NuGetEXE install "Relativity.RAPBuilder" "-ExcludeVersion" -o $buildTools }

    exec { & $RAPBuilder `
        "--source" "$PSScriptRoot" `
        "--input" "$BuildXML" `
        "--version" "$RAPVersion"
    }

    Get-ChildItem -Path $ArtifactsDir -Filter *.nuspec |
    ForEach-Object {
        exec { & $NugetExe pack $_.FullName -OutputDirectory (Join-Path $ArtifactsDir "NuGet") -Version $PackageVersion }
    }

    Save-PDBs -SourceDir $SourceDir -ArtifactsDir $ArtifactsDir
}

Task Clean -Description "Delete build artifacts" {
    Initialize-Folder $ArtifactsDir

    Write-Host "Running Clean target on $Solution"
    exec { msbuild @($Solution,
        ("/target:Clean"),
        ("/property:Configuration=$BuildConfig"),
        ("/consoleloggerparameters:Summary"),
        ("/nodeReuse:False"),
        ("/maxcpucount"),
        ("/nologo"))
    }
}

Task Rebuild -Description "Do a rebuild" {
    Initialize-Folder $ArtifactsDir

    Write-Host "Running Rebuild target on $Solution"
    exec { msbuild @($Solution,
        ("/target:Rebuild"),
        ("/property:Configuration=$BuildConfig"),
        ("/consoleloggerparameters:Summary"),
        ("/property:PublishWebProjects=True"),
        ("/nodeReuse:False"),
        ("/maxcpucount"),
        ("/nologo"),
        ("/fileloggerparameters1:LogFile=$LogFilePath"),
        ("/fileloggerparameters2:errorsonly;LogFile=$ErrorLogFilePath"))
    }
}

Task Help -Alias ? -Description "Display task information" {
    WriteDocumentation
}

Task NightlyTest -Alias Nightly -Description "Run nightly functional tests that require a deployed environment." {
    $LogPath = Join-Path $LogsDir "NighlyTestResults.xml"
    $ChromeBinaryDirectory = (Get-ChildItem -Recurse -Directory -Path $ToolsDir -Filter "Relativity.Chromium.Portable*").FullName
    $ChromeBinaryLocation = (Get-ChildItem -Recurse -File -Include chrome.exe -Path $ChromeBinaryDirectory).DirectoryName
    $ENV:ChromeBinaryLocation = $ChromeBinaryLocation
    Invoke-Tests -WhereClause "namespace =~ FunctionalTests && cat == 'TestType.Nightly'" -OutputFile $LogPath -TestSettings (Join-Path $PSScriptRoot FunctionalTestSettings)
}
function Invoke-Tests
{
    param (
        [Parameter(Mandatory=$true, Position=0)]
        [String] $WhereClause,
        [Parameter()]
        [String] $OutputFile,
        [Parameter()]
        [String] $TestSettings,
        [Parameter()]
        [Switch]$WithCoverage
    )

    $NUnit = Resolve-Path (Join-Path $BuildToolsDir "NUnit.ConsoleRunner\tools\nunit3-console.exe")
    $settings = if($TestSettings) { "@$TestSettings" }
    Initialize-Folder $ArtifactsDir -Safe
    Initialize-Folder $LogsDir -Safe
    if($WithCoverage)
    {
        $OpenCover = Join-Path $BuildToolsDir "opencover\tools\OpenCover.Console.exe"
        $ReportGenerator = Join-Path $BuildToolsDir "reportgenerator\tools\net47\ReportGenerator.exe"
        $CoveragePath = Join-Path $LogsDir "Coverage.xml"
 
        exec { & $OpenCover -target:$NUnit -targetargs:"$Solution --where=`"$WhereClause`" --noheader --labels=On --skipnontestassemblies --result=$OutputFile $settings" -register:path64 -filter:"+[kCura*]* +[Relativity*]* -[*Tests*]*" -hideskipped:All -output:"$LogsDir\OpenCover.xml" -returntargetcode }
        exec { & $ReportGenerator -reports:"$LogsDir\OpenCover.xml" -targetdir:$LogsDir -reporttypes:Cobertura }
        Move-Item (Join-Path $LogsDir Cobertura.xml) $CoveragePath -Force
        $q = '"'
        $openCoverFile = Join-Path $LogsDir 'OpenCover.xml'
        $openCoverTargetArgs = "$Solution $q--where=$WhereClause$q --noheader --labels=On --skipnontestassemblies $q--result=$OutputFile$q $settings"
        Write-host "OpenCoverTargetArgs: $openCoverTargetArgs"
        exec { & $OpenCover -target:$NUnit -targetargs:"$openCoverTargetArgs" -register:user -filter:"+[RelativityServices*]* -[*Tests*]* -[*NUnit*]*" -hideskipped:All -output:"$openCoverFile" -returntargetcode }
        write-host "Executing report generator"
        exec { & $ReportGenerator -reports:"$openCoverFile" -targetdir:$LogsDir -reporttypes:Cobertura }
        Move-Item $openCoverFile $CoveragePath -Force
    }
    else
    {
        exec { & $NUnit $Solution `
            "--where=`"$WhereClause`"" `
            "--noheader" `
            "--labels=On" `
            "--skipnontestassemblies" `
            "--result=$OutputFile" `
            $settings
        }
    }
}