properties {
    #directories
    $root = hg root

    $source_directory = [System.IO.Path]::Combine($root, 'Source')
    $application_directory = [System.IO.Path]::Combine($source_directory, 'Applications')
    $application_xml_directory = [System.IO.Path]::Combine($source_directory, 'ApplicationsXML')
    $development_scripts_directory = [System.IO.Path]::Combine($root, 'DevelopmentScripts')
    $version_directory = [System.IO.Path]::Combine($root, 'Version')
    $vendor_directory = [System.IO.Path]::Combine($root, 'Vendor')
    $robot_directory = [System.IO.Path]::Combine($root, 'Robot')
    $testlog_directory = [System.IO.Path]::Combine($root, 'TestLogs')

    #build variables
    $version = '1.0.0.0'
    $server_type = 'teambranch'
    $build_type = 'DEV'
    $branch = 'default'
    $build_config = "Debug"
       

    #assembly info variables
    $Company = 'kCura LLC'
    $Product = 'Template'
    $ProductDescription = 'Template repo for kCura'

    #microsoft directories
    $microsoft_net_directory = [System.IO.Path]::Combine($env:windir,'Microsoft.NET','Framework','v4.0.30319')
    $microsoft_net64_directory = [System.IO.Path]::Combine($env:windir,'Microsoft.NET','Framework64','v4.0.30319')
    $microsoft_interop_directory = [System.IO.Path]::Combine(${env:ProgramFiles(x86)},'Microsoft.NET')
    $microsoft_vs_directory = [System.IO.Path]::Combine($env:VS110COMNTOOLS,'Common7','Tools')
    $msbuild_exe = [System.IO.Path]::Combine( $microsoft_net64_directory,'MSBuild.exe')

    #nunit variables
    $NUnit = [System.IO.Path]::Combine(${env:ProgramFiles(x86)},'NUnit 2.5.10','bin','net-2.0', 'nunit-console.exe')
    $NUnit_x86 = [System.IO.Path]::Combine(${env:ProgramFiles(x86)},'NUnit 2.5.10','bin','net-2.0', 'nunit-console-x86.exe')
    $testinputfile = [System.IO.Path]::Combine($development_scripts_directory, 'Tests.xml')

    #build variables
    $verbosity ="normal" 
    $inputfile = [System.IO.Path]::Combine($development_scripts_directory, 'Projects.xml')
    $targetsfile = [System.IO.Path]::Combine($development_scripts_directory, 'msbuild.targets')
    $dependencygraph = [System.IO.Path]::Combine($development_scripts_directory, 'DependencyGraph.xml')
    $internaldlls = [System.IO.Path]::Combine($development_scripts_directory, 'dlls.txt')
    $logfile = [System.IO.Path]::Combine($root, 'build.log')
    $diagnostic ="false"

    #signing variables
    $signscript = [System.IO.Path]::Combine($development_scripts_directory, 'sign.bat')
    $sign = ($build_type -ne 'DEV' -and $server_type -ne 'local')

    #nuget variables
    $nuspec_directory = [System.IO.Path]::Combine($development_scripts_directory,'NuGet')
    $nuget_exe_directory = [System.IO.Path]::Combine($vendor_directory,'NuGet')
    $nuget_exe = [System.IO.Path]::Combine($nuget_exe_directory,'NuGet.exe')
    $nuget_server = 'http://dv-scm-nuget.kcura.corp/NuGet/'
    $nuget_version = $version

    #build tool variables    
    $buildhelper_exe = [System.IO.Path]::Combine($development_scripts_directory, 'kCura.BuildHelper.exe')
    $rapbuilder_exe = [System.IO.Path]::Combine($development_scripts_directory, 'kCura.RAPBuilder.exe')
    $testrunner_exe = [System.IO.Path]::Combine($development_scripts_directory, 'kCura.TestRunner.exe')
}