namespace Zaz

module Utils =

    type CommandBuilderResult =
    | Success of obj
    | Failure of string

    let BuildArgs(cmd) =
        let cmdType = cmd.GetType()   
        cmdType.GetProperties()
        |> Array.map(fun pi -> (pi.Name, pi.GetValue(cmd, null).ToString()))
        |> Map.ofArray

    let BuildCommand(cmdType : System.Type, args : Map<string, string>) = 
    
        let getArg key =    
                 
            if args.ContainsKey(key) then
                args.[key]
            else
                failwithf "Value for required property %s was not found" key                

        let rec convertArgVal (v : string) (t : System.Type) : obj = 
            if t = typeof<System.Guid> then
                new System.Guid(v) |> box
            else if t.IsEnum then
                System.Enum.Parse(t, v) |> box
            else if t = typeof<System.Boolean> then
                System.Boolean.Parse(v) |> box
            else if t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<System.Nullable<_>> then            
                convertArgVal v (System.Nullable.GetUnderlyingType(t))
            else 
                let converter = System.ComponentModel.TypeDescriptor.GetConverter(t)                
                if converter <> null && converter.CanConvertFrom(typeof<string>) then
                    converter.ConvertFromString(v)
                else 
                    v |> box
                    
        let getArgVal key (t : System.Type) : obj = 
            let v = getArg key
            convertArgVal v t

        let materializeCmd (t : System.Type) =
            
            let withoutValues = t.GetProperties() 
                                |> Array.filter(fun p -> not (args.ContainsKey(p.Name)))                            
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
            
           
        materializeCmd cmdType
        