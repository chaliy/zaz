& .\LoadEnv.ps1

# NuGet SetApiKey Your-API-Key
NuGet pack .\src\ZazClient\ZazClient.csproj #-Symbols

gci *.nupkg | %{
	Write-Host Push $_
	NuGet push $_
	rm $_
}