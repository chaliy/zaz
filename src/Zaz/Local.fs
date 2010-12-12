namespace Zaz.Local

open Zaz

type LocalCommandBus(resolver : System.Type -> ((obj -> unit) seq)) =
   interface ICommandBus with
      member this.Post(cmd) = 
        resolver(cmd.GetType())
        |> Seq.iter(fun handler -> handler(cmd))

module Types =

    open System
    open System.Reflection

    let ofAssembly (asm : System.Reflection.Assembly) = 
        asm.GetTypes()
        |> Seq.filter( fun t -> t.IsAbstract = false 
                                && t.IsClass = true )    

    let filter (filter : Type -> bool) (types : Type seq) =
        types |> Seq.filter filter

    let chooseHandlerMethods (filter : MethodInfo -> bool) (cmd : Type) (types : Type seq) = 
        types                
        |> Seq.collect(fun t -> t.GetMethods())
        |> Seq.filter(filter)        
        |> Seq.filter( fun m -> m.GetParameters().Length = 1 
                                 && m.GetParameters().[0].ParameterType.IsAssignableFrom(cmd) )

    let toExecutable (act : Type -> obj) (mm : MethodInfo seq) =
        mm 
        |> Seq.map(fun m -> fun (c : obj) ->                         
                                let handler = act m.ReflectedType
                                let xpr = System.Linq.Expressions.Expression.Call(                            
                                                System.Linq.Expressions.Expression.Constant(handler, m.ReflectedType), m,
                                                System.Linq.Expressions.Expression.Constant(c, c.GetType())) 
                            
                                if m.ReturnType = typeof<System.Void> then
                                    System.Linq.Expressions.Expression.Lambda<System.Action>(xpr).Compile().Invoke()
                                else
                                    System.Linq.Expressions.Expression.Lambda<System.Func<System.Object>>(xpr).Compile().Invoke() |> ignore                            
                                () )
      
type DefaultBuses =        
    static member LocalBus(asm : System.Reflection.Assembly, ?activator : System.Type -> obj) =
        let handlersFromAssembly(asm : System.Reflection.Assembly) =
            fun cmd -> Types.ofAssembly(asm)
                        |> Types.filter( fun t -> t.Name.EndsWith("Handler") )
                        |> Types.chooseHandlerMethods (fun m -> m.Name = "Handle") cmd
                        |> Types.toExecutable (defaultArg activator System.Activator.CreateInstance)
        new LocalCommandBus(handlersFromAssembly(asm))

    static member LocalBus(asm : System.Reflection.Assembly, activator : System.Func<System.Type, obj>) =
        let handlersFromAssembly(asm : System.Reflection.Assembly) =
            fun cmd -> Types.ofAssembly(asm)
                        |> Types.filter( fun t -> t.Name.EndsWith("Handler") )
                        |> Types.chooseHandlerMethods (fun m -> m.Name = "Handle") cmd
                        |> Types.toExecutable (fun t -> activator.Invoke(t))
        new LocalCommandBus(handlersFromAssembly(asm))

    static member LocalBus(asm : System.Reflection.Assembly) =
        let handlersFromAssembly(asm : System.Reflection.Assembly) =
            fun cmd -> Types.ofAssembly(asm)
                        |> Types.filter( fun t -> t.Name.EndsWith("Handler") )
                        |> Types.chooseHandlerMethods (fun m -> m.Name = "Handle") cmd
                        |> Types.toExecutable (System.Activator.CreateInstance)
        new LocalCommandBus(handlersFromAssembly(asm))

   
