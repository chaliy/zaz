# Using HTTPIE

http        GET     http://localhost:9302/Commands
http --form POST    http://localhost:9302/Commands/Legacy         key=test Zaz-Command-Id=SampleCommands.PrintMessage Message="Hello World!"


# SampleAppScript2.ps1 works perfectly
http        POST    http://localhost:9302/Commands                "{ Key: SampleCommands.PrintMessage, Command={ Message: 'Hello World!' }"




