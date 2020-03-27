function Clean-DotNETProject
{
    Get-ChildItem .\ -include bin, obj -Recurse | foreach ($_) {
        Write-Output "Remove: $($_.FullName)"
        Remove-Item $_.FullName -Force -Recurse
    }
}

Clean-DotNETProject

cd .\Superset\
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd ..

if (!(Test-Path .\_published\))
{
    md .\_published\
}

Copy-Item .\Superset\bin\Debug\*.nupkg .\_published\ -ErrorAction SilentlyContinue
Copy-Item .\Superset\bin\Debug\*.snupkg .\_published\ -ErrorAction SilentlyContinue
Copy-Item .\Superset\bin\Release\*.nupkg .\_published\ -ErrorAction SilentlyContinue
Copy-Item .\Superset\bin\Release\*.snupkg .\_published\ -ErrorAction SilentlyContinue

Remove-Item -ErrorAction Ignore -Force -Recurse $HOME\.nuget\packages\superset\
