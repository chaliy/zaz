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
    [String]$Destination = "http://localhost:9302/Commands/"    
)

ipmo psget
install-module psjson
# PsJson and Net.WebClient are used to ensure script works under Powershell v2.0


$cmd = @"
{ 
	'Key' : '$Command',
	'Command' : $Data
}
"@

$Status_Pending = 0
$Status_InProgress = 1
$Status_Success = 2
$Status_Failure = 3

$Severity_Info = 0
$Severity_Warning = 1
$Severity_Error = 2

function convertToToken($d){
    $d.ToString("o")
}

function convertFromJsonDate($ii){
    if ($ii -match 'Date\((?<d>\d*)\)'){
        $d = new-object DateTime(1970, 1, 1)
        $d.AddMilliseconds([double]$matches["d"] + 1)
    }    
}

function writeTrace($t, $sev, $msg){
    switch($sev){
        $Severity_Info {
            Write-Host $msg
        }
        
        $Severity_Warning {
            Write-Host $msg -ForegroundColor:Yellow
        }
        
        $Severity_Error {
            Write-Host $msg -ForegroundColor:Red
        }
    }
}

Write-Verbose "Send command: $cmd"

$client = (New-Object Net.WebClient)
$client.Headers["Content-Type"] = "application/json"

# Sync version
# $client.UploadString($Destination, "POST", $cmd)

# Async
$scheduledResp = $client.UploadString($Destination + "Scheduled", "POST", $cmd)
$scheduled = convertfrom-json $scheduledResp
$execId = $scheduled.Id

Write-Verbose "Execution ID: $execId"

$read = $true
$token = [DateTime]::MinValue
while($read){

    sleep -m 500

    $url = $Destination + "Scheduled/" + $execId + "/?token=" + (convertToToken($token))
    Write-Verbose $url
    $statsResp = $client.DownloadString($url)
    $stats = convertfrom-json $statsResp
    
    $status = $stats.Status
    $trace = $stats.Trace    

    $trace | % {
                   
        $timestamp = convertFromJsonDate($_.Timestamp)

        writeTrace $timestamp ($_.Severity) ($_.Message)

        if ($timestamp -gt $token){
            $token = $timestamp
        }
    }

    switch ($status){               

        $Status_Pending {            
            Write-Verbose "Command is pending"
        }
        $Status_InProgress {
            Write-Verbose "Command is in progress"
        }                    
        $Status_Success {
            Write-Host "Command is complete" -ForegroundColor:Green
            $read = $false
        }
        $Status_Failure {
            Write-Error "Command failed"
            $read = $false
        }        
    }
}
    
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