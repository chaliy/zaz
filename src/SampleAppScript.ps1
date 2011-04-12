import-module PsGet # Requires PsGet installed

install-module https://github.com/chaliy/psurl/raw/master/PsUrl/PsUrl.psm1
import-module PsUrl

Write-Url "http://localhost:9302/Commands/" -Data @{ "Zaz-Command-Id" = "SampleCommands.PrintMessage";
													 "Message" = "Hello world!" }