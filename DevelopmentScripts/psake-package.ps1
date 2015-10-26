. .\psake-common.ps1


task default -depends package, sign


task package_initalize {
    $script:package_directory = [System.IO.Path]::Combine($package_root_directory, $Product, $branch, $version)
    $script:package_bin_directory = [System.IO.Path]::Combine($package_directory, 'bin')
    $script:package_rap_directory = [System.IO.Path]::Combine($package_directory, 'RAP')
    $script:package_cp_directory = [System.IO.Path]::Combine($package_directory, 'CustomPages')
    $script:package_nuget_directory = [System.IO.Path]::Combine($package_directory, 'NuGet')
    $script:package_pdb_directory = [System.IO.Path]::Combine($package_directory, 'PDBs')

    [System.IO.Directory]::CreateDirectory($package_directory)
    [System.IO.Directory]::CreateDirectory($package_nuget_directory)
    [System.IO.Directory]::CreateDirectory($package_bin_directory) 
    [System.IO.Directory]::CreateDirectory($package_pdb_directory)
}

task package -depends package_initalize { 

    Copy-Item -Path ([System.IO.Path]::Combine($nuspec_directory, '*')) -Destination $package_nuget_directory -Include '*.nupkg'
    
    if ([System.IO.Directory]::GetFiles($application_directory, "*.rap", [System.IO.SearchOption]::TopDirectoryOnly).Count -gt 0) {
        [System.IO.Directory]::CreateDirectory($package_rap_directory)
        Copy-Item -Path ([System.IO.Path]::Combine($application_directory, '*')) -Destination $package_rap_directory -Include '*.rap'
    }

    if ([System.IO.Directory]::Exists([System.IO.Path]::Combine($source_directory, 'CustomPages'))) {
        [System.IO.Directory]::CreateDirectory($package_cp_directory)
        Copy-Item -Path ([System.IO.Path]::Combine($source_directory, 'CustomPages', '*')) -Destination $package_cp_directory -Include '**' -Recurse
    }

    foreach($o in Get-ChildItem $source_directory) {
        if($o.PSIsContainer -and !$o.FullName.Contains('NUnit') -and ([System.IO.Directory]::Exists([System.IO.Path]::Combine($o.FullName, 'bin')))) {
            Copy-Item -Path ([System.IO.Path]::Combine($o.FullName, 'bin', '*')) -Destination $package_bin_directory -Include '*.exe', '*.dll', '*.msi'
        }    
    }

    if ([System.IO.Directory]::Exists($pdb_directory) {
        Copy-Item -Path ($pdb_directory) -Destination $package_pdb_directory -Include '*.pdb' -Recurse
    }
    
}

task sign -precondition { ($build_type -ne 'DEV') -and ($server_type -ne 'local') } {
    foreach($o in Get-ChildItem -Path $package_directory -Recurse  -Include '*.exe', '*.dll', '*.msi') {
        exec {
            & $signscript @($o.FullName)
        }
    }
}
