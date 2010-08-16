namespace CommandRouter.Local

open CommandRouter

type LocalCommandBus(resolver : System.Type -> obj -> unit) =
   interface ICommandBus with
      member this.Post(cmd) = 
        resolver(cmd.GetType())(cmd)
      
type CommandHandlerResolvers =
    static member FromAssembly(asm : System.Reflection.Assembly) =                 

        fun cmd -> 
                asm.GetTypes()
                   |> Seq.filter( fun t -> t.Name.EndsWith("Handler")
                                            && t.IsAbstract = false )
                   |> Seq.choose( fun t -> match t.GetMethod("Handle") with
                                           | null -> None
                                           | x -> Some(x) )
                   |> Seq.filter( fun m -> m.GetParameters().Length = 1
                                            && m.GetParameters().[0].ParameterType.IsAssignableFrom(cmd) )       
                   |> Seq.map( fun m -> fun (c : obj) ->                         
                            let handler = System.Activator.CreateInstance(m.ReflectedType)
                            let xpr = System.Linq.Expressions.Expression.Call(                            
                                            System.Linq.Expressions.Expression.Constant(handler, m.ReflectedType), m,
                                            System.Linq.Expressions.Expression.Constant(c, c.GetType())) 
                            
                            if m.ReturnType = typeof<System.Void> then
                                System.Linq.Expressions.Expression.Lambda<System.Action>(xpr).Compile().Invoke()
                            else
                                System.Linq.Expressions.Expression.Lambda<System.Func<System.Object>>(xpr).Compile().Invoke() |> ignore                                         
                            
                            () )                    
                  |> Seq.find (fun x -> true)