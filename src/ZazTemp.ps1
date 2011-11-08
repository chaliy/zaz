
$cmd = @"
{ 
	'Key' : 'LongFoo1',
	'Command' : null
}
"@

$Destination = "http://localhost.fiddler:9302/Commands/"

function _temp_read($adb, $ii){    
    $readExpr = '"' + $adb + '":"(?<val>[^"]*)"'
    
    if ($ii -match $readExpr){
        return $matches["val"]
    }    
}

function _temp_readInt($adb, $ii){    
    $readExpr = '"' + $adb + '":(?<val>[\d]*)'
    
    if ($ii -match $readExpr){
        return $matches["val"]
    }    
}

function _temp_readId($ii){
    _temp_read "Id" $ii
}

function _temp_readStatus($ii){
    _temp_readInt "Status" $ii
}

Write-Verbose "Send command: $cmd"

$client = (New-Object Net.WebClient)
$client.Headers["Content-Type"] = "application/json"

# Sync version
# $client.UploadString($Destination, "POST", $cmd)

# Async
$scheduledResp = $client.UploadString($Destination + "/Scheduled", "POST", $cmd)
$execId = _temp_readId($scheduledResp)

Write-Host "Execution ID: $execId"


$Status_Pending = 0
$Status_InProgress = 1
$Status_Success = 2
$Status_Failure = 3


$read = $true
while($read){

    sleep -m 200

    $statsResp = $client.DownloadString($Destination + "/Scheduled/" + $execId)
    Write-Host "Status resp: $statsResp"

    $status = _temp_readStatus($statsResp)

    switch ($status)
    {
        $Status_Pending {
            Write-Host "Command is pending";
        }
        $Status_InProgress {
            Write-Host "Command in pregress";
        }                    
        $Status_Success {
            Write-Host "Command is complete";
            $read = $false
        }
        $Status_Failure {
            Write-Error "Command failed";
            $read = $false
        }        
    }          
}
