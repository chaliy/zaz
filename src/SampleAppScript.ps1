if (-not (get-module PsUrl -ListAvailable)){
	if (-not (get-module PsGet -ListAvailable)){
		throw "Script requires PsGet (https://github.com/chaliy/psget) installed"
	}
	import-module PsGet	
	install-module https://github.com/chaliy/psurl/raw/master/PsUrl/PsUrl.psm1
}
import-module PsUrl

Write-Url "http://localhost:9302/Commands/" -Data @{ "Zaz-Command-Id" = "SampleCommands.PrintMessage";
													 "Message" = "Hello world!" }