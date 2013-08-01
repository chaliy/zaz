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

$cmd2 = @"
{ 
	'Key' : 'SampleCommands.PrintMessage',			
	'Command' : {
		'Message' : 'Hello world #2'
	}
}
"@

$cmd1 = @"
{ 
	'Key' : 'SampleCommands.PrintMessage'
}
"@

$legacy = "Message=Hello+world!&Key=test&Zaz-Command-Id=SampleCommands.PrintMessage";

$client = (New-Object Net.WebClient)

$client.Headers.Add("Accept", "application/json")
$client.Headers.Add("Content-Type", "application/json")

$client.Headers.Add("Accept", "application/json")
$client.Headers.Add("Content-Type", "application/json")
$client.UploadString("http://localhost.fiddler:9302/Commands/Legacy", "POST", $legacy)

$client.Headers.Add("Accept", "application/json")
$client.Headers.Add("Content-Type", "application/json")
$client.UploadString("http://localhost.fiddler:9302/Commands/Legacy", "POST", $legacy)

$client.Headers.Add("Accept", "application/json")
$client.Headers.Add("Content-Type", "application/json")
$client.UploadString("http://localhost.fiddler:9302/Commands/", "POST", $cmd)

$client.Headers.Add("Accept", "application/json")
$client.Headers.Add("Content-Type", "application/json")
$client.UploadString("http://localhost.fiddler:9302/Commands/", "POST", $cmd)

$client.Headers.Add("Accept", "application/json")
$client.Headers.Add("Content-Type", "application/json")
$client.UploadString("http://localhost.fiddler:9302/Commands/Scheduled/", "POST", $cmd)

$client.Headers.Add("Accept", "application/json")
$client.Headers.Add("Content-Type", "application/json")
$client.UploadString("http://localhost.fiddler:9302/Commands/Scheduled/", "POST", $cmd)

# Write-Url "http://localhost:9302/Commands/" -Content $cmd -Verbose