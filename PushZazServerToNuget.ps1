& .\LoadEnv.ps1

# NuGet SetApiKey Your-API-Key
NuGet pack .\src\ZazServer\ZazServer.csproj #-Symbols

gci *.nupkg | %{
	Write-Host Pushing $_
	NuGet push $_
	rm $_
}