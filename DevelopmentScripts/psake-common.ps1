properties {
    #directories
    $root = hg root

    $source_directory = [System.IO.Path]::Combine($root, 'Source')
    $application_directory = [System.IO.Path]::Combine($root, 'Applications')
    $application_xml_directory = [System.IO.Path]::Combine($root, 'ApplicationsXML')
    $development_scripts_directory = [System.IO.Path]::Combine($root, 'DevelopmentScripts')
    $version_directory = [System.IO.Path]::Combine($root, 'Version')
    $vendor_directory = [System.IO.Path]::Combine($root, 'Vendor')
    $robot_directory = [System.IO.Path]::Combine($root, 'Robot')
    $testlog_directory = [System.IO.Path]::Combine($root, 'TestLogs')
    $buildlogs_directory = [System.IO.Path]::Combine($root, 'BuildLogs')
    $pdb_directory = [System.IO.Path]::Combine($root, 'PDBs')
    $doc_directory = [System.IO.Path]::Combine($root, 'Documentation')

    #build variables
    $version = '1.0.0.0'
    $server_type = 'teambranch'
    $build_type = 'DEV'
    $branch = hg branch
    $build_config = "Debug"
    $Injections = 'DisableInjections'

    #assembly info variables
    $company = 'kCura LLC'
    $product = [System.IO.Path]::GetFileName($root)
    $product_description =  [System.IO.Path]::GetFileName($root) + ' Description'

    #versioning database info
    $server = 'bld-mstr-01.kcura.corp'
    $database ='TCBuildVersion'
    $project = 'Development'
    $major_version = 1
    $minor_version = 0
    
    $buildid = 0

    #microsoft directories
    $microsoft_net_directory = [System.IO.Path]::Combine($env:windir,'Microsoft.NET','Framework','v4.0.30319')
    $microsoft_net64_directory = [System.IO.Path]::Combine($env:windir,'Microsoft.NET','Framework64','v4.0.30319')
    $microsoft_interop_directory = [System.IO.Path]::Combine(${env:ProgramFiles(x86)},'Microsoft.NET')
    $microsoft_vs_directory = [System.IO.Path]::Combine($env:VS110COMNTOOLS,'Common7','Tools')
    $windows_sdk_directory = [System.IO.Path]::Combine(${env:ProgramFiles(x86)}, 'Microsoft SDKs', 'Windows', 'v7.0A')
    
    $msbuild_exe = [System.IO.Path]::Combine(${env:ProgramFiles(x86)}, 'MSBuild', '14.0', 'Bin','MSBuild.exe')

    #nunit variables
    $NUnit = [System.IO.Path]::Combine($development_scripts_directory, 'NUnit.Runners', 'tools', 'nunit-console.exe')
    $NUnit_x86 = [System.IO.Path]::Combine($development_scripts_directory, 'NUnit.Runners', 'tools', 'nunit-console-x86.exe')
    $NUnit3 = [System.IO.Path]::Combine($development_scripts_directory, 'NUnit.Console', 'tools', 'nunit3-console.exe')

    #build variables
    $verbosity ="normal" 
    $inputfile = [System.IO.Path]::Combine($development_scripts_directory, 'build.xml')
    $targetsfile = [System.IO.Path]::Combine($development_scripts_directory, 'msbuild.targets')
    $dependencygraph = [System.IO.Path]::Combine($development_scripts_directory, 'DependencyGraph.xml')
    $internaldlls = [System.IO.Path]::Combine($development_scripts_directory, 'dlls.txt')
    $logfile = [System.IO.Path]::Combine($buildlogs_directory, 'build.log')
    $logfilewarn = [System.IO.Path]::Combine($buildlogs_directory, 'buildwarnings.log')
    $logfileerror = [System.IO.Path]::Combine($buildlogs_directory, 'builderrors.log')
    $diagnostic ="false"

    #signing variables
    $signscript = [System.IO.Path]::Combine($development_scripts_directory, 'sign.ps1')

    #nuget variables
    $nuspec_directory = [System.IO.Path]::Combine($development_scripts_directory,'NuGet')
    $nuget_exe_directory = [System.IO.Path]::Combine($vendor_directory,'NuGet')
    $nuget_exe = [System.IO.Path]::Combine($nuget_exe_directory,'NuGet.exe')
	$proget_server = 'https://proget.kcura.corp/nuget/NuGet'
    $nuget_version = $version

    #build tool variables    
    $buildhelper_exe = [System.IO.Path]::Combine($development_scripts_directory, 'kCura.BuildHelper.exe')
    $rapbuilder_exe = [System.IO.Path]::Combine($development_scripts_directory, 'kCura.RAPBuilder.exe')
    $testrunner_exe = [System.IO.Path]::Combine($development_scripts_directory, 'kCura.TestRunner.exe')
    $buildeditor_exe = [System.IO.Path]::Combine($development_scripts_directory, 'kCura.BuildToolsEditor.exe')

    #package variable
    $package_root_directory = [System.IO.Path]::Combine($root, 'Packages')
}