function Get-Version {
    $latestTag = git describe --tags --abbrev=0 2>$null
    if ($null -eq $latestTag) { return "v0.0.0" }
    return $latestTag
}

function New-Release {
    param([string]$Type = "patch")
    
    $current = Get-Version
    $currentVersion = $current.TrimStart('v')
    $parts = $currentVersion.Split('.')
    
    [int]$major = $parts[0]
    [int]$minor = $parts[1]
    [int]$patch = $parts[2]
    
    switch ($Type) {
        "major" { $major++; $minor = 0; $patch = 0 }
        "minor" { $minor++; $patch = 0 }
        "patch" { $patch++ }
    }
    
    $newVersion = "v$major.$minor.$patch"
    Write-Host "Creating new release: $newVersion" -ForegroundColor Green
    
    git tag $newVersion
    git push origin $newVersion
}

# Export functions
Export-ModuleMember -Function Get-Version, New-Release
