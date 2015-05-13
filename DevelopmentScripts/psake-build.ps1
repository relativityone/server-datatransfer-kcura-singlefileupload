. .\psake-common.ps1


task default -depends build


task build -depends build_initalize, build_projects {
 
}


task build_initalize {   
    ''
    ('='*25) + ' Build Parameters ' + ('='*25)
    'version      = ' + $version 
    'server type  = ' + $server_type 
    'build type   = ' + $build_type 
    'branch       = ' + $branch 
    'build config = ' + $build_config
    ''

    'Time: ' + (Get-Date -Format 'yyy-MM-dd HH:mm:ss')
    
    'Build Type and Server Type result in sign set to ' + ($build_type -ne 'DEV' -and $server_type -ne 'local')  

    if([System.IO.File]::Exists($logfile)) {Remove-Item $logfile}
}


task get_buildhelper {
    exec {
        & $nuget_exe @('install', 'kCura.BuildHelper', '-ExcludeVersion')
    }      
    Copy-Item ([System.IO.Path]::Combine($development_scripts_directory, 'kCura.BuildHelper', 'lib', 'kCura.BuildHelper.exe')) $development_scripts_directory
}

task create_build_script -depends get_buildhelper {   
    exec {
        & $buildhelper_exe @(('/source:' + $root), 
                             ('/input:' + $inputfile), 
                             ('/output:' + $targetsfile), 
                             ('/graph:' + $dependencygraph), 
                             ('/dllout:' + $internaldlls), 
                             ('/vs:11.0'), 
                             ('/sign:' + ($build_type -ne 'DEV' -and $server_type -ne 'local')), 
                             ('/signscript:' + $signScript ))
    }                                                                        
}                                                                               
                                                                                
task build_projects -depends create_build_script {  
    exec {                                                                                
        &  $msbuild_exe @(($targetsfile),   
                         ('/property:SourceRoot=' + $root),
                         ('/property:Configuration=' + $build_config),	
                         ('/property:BuildProjectReferences=false'),		
                         ('/target:BuildTiers'),
                         ('/verbosity:' + $verbosity),
                         ('/nologo'),
                         ('/maxcpucount'), 
                         ('/flp1:LogFile=' + $logfile))       
    } 
}



