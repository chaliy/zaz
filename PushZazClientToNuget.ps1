& .\LoadEnv.ps1

# NuGet SetApiKey Your-API-Key
.\.nuget\NuGet pack .\src\ZazClient\ZazClient.csproj #-Symbols

gci *.nupkg | %{
	Write-Host Push $_
	.\.nuget\NuGet push $_
	rm $_
}