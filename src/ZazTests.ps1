$here = (Split-Path -parent $MyInvocation.MyCommand.Definition)
set-location $here
import-module PowerSpec -Force
import-module .\Zaz.psm1 -Force

test-spec {
    "When sending ping command"
    
    $res = Send-ZazCommand SampleCommands.PrintMessage "{'Message':'Hello!'}" -Destination "http://localhost.fiddler:9302/Commands/"    
    $res | should work
}