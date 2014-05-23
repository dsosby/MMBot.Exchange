# PSake tasks

properties {
    $nuget = "nuget"
}

function Get-NugetUrl($nuget) {
    switch ($nuget) {
        "staging" {
            return "https://staging.nuget.org"
        }
        default {
            return "https://www.nuget.org"
        }
    }
}

function Get-LatestNupkg {
    # Better to sort by version
    $packages = gci *.nupkg | sort -Property LastWriteTime -Descending
    if (!$packages) {
        return $null
    }
    return $packages[0]
}

task default -depends package

task package {
    Exec { nuget pack .\MMBot.Exchange\MMBot.Exchange.csproj }
}

task clean {
    rm *.nupkg
}

task publish {
    $url = Get-NugetUrl $nuget
    $pack = Get-LatestNupkg

    Assert $pack "No nupkg found"
    Exec { nuget push $pack -Source $url }
}