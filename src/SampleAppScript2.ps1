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

$cmd1 = @"
{ 
	'Key' : 'SampleCommands.PrintMessage'
}
"@


$client = (New-Object Net.WebClient)
$client.Headers.Add("Content-Type", "application/json")
$client.UploadString("http://localhost.fiddler:9302/Commands/", "POST", $cmd)

#Write-Url "http://localhost.fiddler:9302/Commands/" -Content $cmd -Verbose