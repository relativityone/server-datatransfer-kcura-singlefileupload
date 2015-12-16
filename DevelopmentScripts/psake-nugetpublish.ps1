. .\psake-common.ps1


task default -depends nuget_publish

task nuget_publish -precondition { ($build_type -eq 'GOLD') -and ($branch -eq 'default' -or $branch.startsWith('release')) } {
    $dictRef = @{}
    $dictPath = @{}
    $order = @()

    foreach($o in Get-ChildItem $nuspec_directory){
        
        if($o.Extension -ne '.nupkg') {continue}

        Add-Type -AssemblyName System.IO.Compression.FileSystem
        $zip = [System.IO.Compression.ZipFile]::OpenRead($o.FullName) 
        
        foreach($item in $zip.Entries){
            if(!$item.FullName.EndsWith('.nuspec')) {continue}

            $file = ([System.IO.StreamReader]$item.Open()).ReadToEnd()                

            $xdoc = New-Object System.Xml.XmlDocument

            $xdoc.LoadXml($file)

            $id = $xdoc.package.metadata.id
            
            $dictRef.Add($id, @())
            $dictPath.Add($id, $o.FullName)

            foreach ($node in $xdoc.package.metadata.dependencies.dependency) {
                $dictRef.Set_Item($id, ($dictRef.$id += $node.id))
            }            
        }    

        $zip.Dispose()           
    }   


    while ($dictRef.Count -gt 0){
        $key = ""

        foreach($ref in $dictRef.GetEnumerator()) {
        
            $inOrder = $true

            foreach($dep in $ref.Value) {
                if (!$order.Contains($dep)) {
                    $inOrder = $false
                    break
                }
            }

            if ($inOrder) {
                $key = $ref.Key
                $order += $key
                break
            }        
        }

        $dictRef.Remove($key)
    }


    foreach ($item in $order) {
        Write-Host "publishing" $dictPath.$item "..."
        
        exec {
            & $nuget_exe @('push', $dictPath.$item, '-Source', $nuget_server) 2>&1
        }

    }  
}
