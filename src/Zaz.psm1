##
##    PowerShell Client for Zaz Command Bus
##

function Send-ZazCommand {
[CmdletBinding()]
Param(
    [Parameter(ValueFromPipeline=$true, ValueFromPipelineByPropertyName=$true, Mandatory=$true, Position=0)]
    [String]$Command,
    [Parameter(ValueFromPipeline=$true, ValueFromPipelineByPropertyName=$true, Mandatory=$false, Position=1)]
    $Data = "null",
    [String]$Destination = "http://localhost:9302/Commands"    
)

$cmd = @"
{ 
	'Key' : '$Command',
	'Command' : $Data
}
"@

function _temp_loadIdJson($input){
    $idregex = [regex] '"Id":"(?<id>[^"]*)"'
    $matches = $idregex.match($input)
    write-host $matches
}

Write-Verbose "Send command: $cmd"

$client = (New-Object Net.WebClient)
$client.Headers["Content-Type"] = "application/json"

# Sync version
# $client.UploadString($Destination, "POST", $cmd)

# Async
$scheduledResp = $client.UploadString($Destination + "/Scheduled", "POST", $cmd)
_temp_loadIdJson($scheduledResp)

    
<#
.Synopsis
    Sends command to ZAZ Command Bus
.Description    
.Parameter Command
    Key of the command
.Parameter Data
    Optional JSON payload of the command
.Parameter Destination
    Full URL to the command endpoint
.Example
    Send-ZazCommand FooCommand

    Description
    -----------
    Sends command FooCommand
	
.Example
    Send-ZazCommand Ping "{ Message : 'Hello word!' }" -Destination "http://localhost:9302/Commands"

    Description
    -----------
    Sends command Ping to command bus at "http://localhost:9302/Commands" with some data.

#>
}