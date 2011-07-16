import-module PsGet	# From http://psget.net
install-module PsUrl

Write-Url "http://localhost:9302/Commands/Legacy/" -Data @{ "Zaz-Command-Id" = "SampleCommands.PrintMessage";
													 "Message" = "Hello world!" }