function Clean-DotNETProject
{
    Get-ChildItem .\ -include bin, obj -Recurse | foreach ($_) {
        Write-Output "Remove: $($_.FullName)"
        Remove-Item $_.FullName -Force -Recurse
    }
}

Clean-DotNETProject

dotnet pack --include-source -c Debug

if (!(Test-Path .\_published\))
{
    md .\_published\
}
rm -Force .\_published\*

Copy-Item .\bin\Debug\*.nupkg .\_published\ -ErrorAction SilentlyContinue
Copy-Item .\bin\Release\*.nupkg .\_published\ -ErrorAction SilentlyContinue

Remove-Item -ErrorAction Ignore -Force -Recurse $HOME\.nuget\packages\superset\
