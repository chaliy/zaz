namespace Zaz.Commander

open Zaz.Utils

type CommandLineCommandBuilder(resolver : string -> System.Type seq, ?argv : string list) =
    let argv = defaultArg argv (System.Environment.GetCommandLineArgs() |> Array.toList)
    let args =  argv
                |> List.map(fun arg -> 
                                let index = arg.IndexOf(":")
                                if index > 0 then
                                    ( arg.Substring(0, index), arg.Substring(index + 1) )
                                else  
                                    ( arg, "")
                            )
                |> Map.ofList
    
    let materializeCmd (t : System.Type) = Zaz.Utils.BuildCommand(t, args)           
           
    let buildCmd() =        
        if argv.Length <= 1 then                   
            Failure("Icorrect arguments:
        USAGE:

        Commander COMMAND_NAME Arg1:Val1 Arg2:Val2")
        else
            let commandKey = argv.[1]
            let commands = resolver(commandKey) |> Seq.toList
            match commands with
            | [x] -> materializeCmd x
            | [] -> Failure(sprintf "Command %s was not found" commandKey)        
            | _ -> Failure(failwithf "Too much commands was found for command key %s" commandKey)

    member x.BuildCommand = buildCmd