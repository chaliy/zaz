$here = (Split-Path -parent $MyInvocation.MyCommand.Definition)
set-location $here
import-module PowerSpec
import-module .\Zaz.psm1 -Force

test-spec {
    "When sending ping command"
    $resp = Send-ZazCommand SampleCommands.PrintMessage "{Message:'Hello!'}" -Destination "http://localhost.fiddler:9302/Commands"
    $resp | should be_equal "Command SampleCommands.PrintMessage accepted"
}