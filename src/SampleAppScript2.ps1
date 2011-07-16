import-module PsGet	# From http://psget.net
install-module PsUrl

$cmd = @"
{ 
	'Key' : 'SampleCommands.PrintMessage',			
	'Command' : {
		'Message' : 'Hello world!'
	}
}
"@

$cmd = @"
{ 
	'Key' : 'SampleCommands.PrintMessage'
}
"@

Write-Url "http://localhost.fiddler:9302/Commands/" -Content $cmd