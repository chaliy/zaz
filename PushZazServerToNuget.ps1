# NuGet SetApiKey Your-API-Key
.\packages\NuGet.CommandLine.1.4.20615.182\tools\NuGet pack .\src\ZazServer\ZazServer.csproj #-Symbols

gci *.nupkg | %{
	Write-Host Pushing $_
	.\packages\NuGet.CommandLine.1.4.20615.182\tools\NuGet push $_
	rm $_
}