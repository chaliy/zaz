# NuGet SetApiKey Your-API-Key
# .\src\ZazClient\ZazClient.nuspec
.\packages\NuGet.CommandLine.1.4.20615.182\tools\NuGet pack .\src\ZazClient\ZazClient.csproj -Symbols

gci *.nupkg | %{
	Write-Host Push $_
	.\packages\NuGet.CommandLine.1.4.20615.182\tools\NuGet push $_
	rm $_
}