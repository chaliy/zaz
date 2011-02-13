namespace Zaz.Commander

type CommandBuilderResult =
| Success of obj
| Failure of string

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

    let rec convertArgVal (v : string) (t : System.Type) : obj = 
        if t = typeof<System.Guid> then
            new System.Guid(v) |> box
        else if t.IsEnum then
            System.Enum.Parse(t, v) |> box
        else if t = typeof<System.Boolean> then
            System.Boolean.Parse(v) |> box
        else if t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<System.Nullable<_>> then            
            convertArgVal v (System.Nullable.GetUnderlyingType(t))
        else v |> box
                    
    let getArgVal key (t : System.Type) : obj = 
        let v = getArg key
        convertArgVal v t

    let materializeCmd (t : System.Type) =
            
        let withoutValues = t.GetProperties() 
                            |> Array.filter(fun p -> not (args |> Seq.exists(fun arg -> fst(arg) = p.Name )))                            
                            |> Array.toList
                                                        
        match withoutValues with
        | [] -> 
            let cmd = System.Activator.CreateInstance(t)
            t.GetProperties()
            |> Seq.map(fun p -> (p, getArgVal p.Name p.PropertyType))
            |> Seq.iter(fun (p, v) -> p.SetValue(cmd, v, null))
            CommandBuilderResult.Success(cmd)
        | [x] -> CommandBuilderResult.Failure(sprintf "Value for required property %s was not found" (x.Name)) 
        | _ ->
            let msgBuilder = new System.Text.StringBuilder("Value for required properties: ")
            withoutValues
            |> List.iter(fun p -> msgBuilder.Append(p.Name + ";") |> ignore)
            msgBuilder.Append(" was not found") |> ignore

            CommandBuilderResult.Failure(msgBuilder.ToString()) 
            
            
           
    let buildCmd() =        
        if argv.Length <= 1 then                   
            CommandBuilderResult.Failure("Icorrect arguments:
                                            USAGE:

                                            Commander COMMAND_NAME Arg1:Val1 Arg2:Val2")
        else
            let commandKey = argv.[1]
            let commands = resolver(commandKey) |> Seq.toList
            match commands with
            | [x] -> materializeCmd x
            | [] -> CommandBuilderResult.Failure(sprintf "Command %s was not found" commandKey)        
            | _ -> CommandBuilderResult.Failure(failwithf "Too much commands was found for command key %s" commandKey)

    member x.BuildCommand = buildCmd