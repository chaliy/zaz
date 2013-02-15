& .\LoadEnv.ps1

# NuGet SetApiKey Your-API-Key
.\.nuget\NuGet pack .\src\ZazServer\ZazServer.csproj #-Symbols

gci *.nupkg | %{
	Write-Host Pushing $_
	.\.nuget\NuGet push $_
	rm $_
}