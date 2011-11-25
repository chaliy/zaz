$here = (Split-Path -parent $MyInvocation.MyCommand.Definition)
set-location $here
ipmo PsGet
install-module PowerSpec
import-module .\Zaz.psm1 -Force

test-spec {
    "When sending ping command"
    
    $res = Send-ZazCommand SampleCommands.PrintMessage @{"Message" = "Hello!"} -Destination "http://localhost.fiddler:9302/commands/"
    $res | should work
}


test-spec {
    "When setting user name"
    
    Set-ZazConfig User Chaliy
    $true | should work
}