namespace Zaz.Commander

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

    let getArg key = 
        let arg = args |> Seq.tryFind(fun arg -> fst(arg) = key )
        match arg with
        | Some(x) -> snd x
        | None -> failwithf "Value for required property %s was not found" key

    let rec convenrtArgVal (v : string) (t : System.Type) : obj = 
        if t = typeof<System.Guid> then
            new System.Guid(v) |> box
        else if t.IsEnum then
            System.Enum.Parse(t, v) |> box
        else if t = typeof<System.Boolean> then
            System.Boolean.Parse(v) |> box
        else if t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<System.Nullable<_>> then            
            convenrtArgVal v (System.Nullable.GetUnderlyingType(t))
        else v |> box
                    
    let getArgVal key (t : System.Type) : obj = 
        let v = getArg key
        convenrtArgVal v t

    let materializeCmd (t : System.Type) =    
        let cmd = System.Activator.CreateInstance(t)
        t.GetProperties()    
        |> Seq.map(fun p -> (p, getArgVal p.Name p.PropertyType))
        |> Seq.iter(fun (p, v) -> p.SetValue(cmd, v, null))
        cmd

    let validateArgs() =
        if argv.Length = 0 then                   
            failwithf "Icorrect arguments:
                            USAGE:

                            Commander COMMAND_NAME Arg1:Val1 Arg2:Val2"
        
    let buildCmd() =
        validateArgs()
        let commandKey = argv.[1]
        let commands = resolver(commandKey) |> Seq.toList
        match commands with
        | [] -> failwithf "Command %s was not found" commandKey
        | [x] -> materializeCmd x
        | _ -> failwithf "Too much commands was found for command key %s" commandKey

    member x.BuildCommand = buildCmd