# Ensure dotnet --version is at least 6
$dotnetVersion = dotnet --version
$versionParts = $version.Split(' ')
$majorVersion = $versionParts[0].Split('.')[0]
if ($majorVersion -lt 6) {
    Write-Error "please install dotnet 6 or higher from https://dotnet.microsoft.com/en-us/download"
    exit 1
}

dotnet build

dotnet run --project .\Elevator\Elevator.csproj