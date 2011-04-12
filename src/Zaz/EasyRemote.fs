namespace Zaz.EasyRemote.Server

    open System
    open System.Net
    open System.ServiceModel
    open System.ServiceModel.Web
    open Microsoft.Http
    open Zaz.Utils    

    [<ServiceContract>]     
    type CommandBus(
                    resolver : string -> (Type option), 
                    bus : Zaz.ICommandBus) =

        let parseQueryString query = 
             let nvc = Web.HttpUtility.ParseQueryString(query)
             nvc.Keys
             |> Seq.cast<string>
             |> Seq.map(fun key -> (key, nvc.[key]))
             |> Map.ofSeq

        [<WebGet(UriTemplate = "")>]
        member this.Get(response : HttpResponseMessage) = 
            response.Content <- HttpContent.Create("Endpoint for commands.")

        [<WebInvoke(Method = "POST", UriTemplate = "")>]
        member this.Post(request : HttpRequestMessage, response : HttpResponseMessage) = 
            let body = request.Content.ReadAsString()
            let form = parseQueryString(body)
            if form.ContainsKey("Zaz-Command-Id") = false then
                response.StatusCode <- HttpStatusCode.BadRequest                
                response.Content <- HttpContent.Create("Required value 'Zaz-Command-Id' was not found.")
            else
                let cmdId = form.["Zaz-Command-Id"]
                match resolver cmdId with
                | Some cmdType -> 
                        match Zaz.Utils.buildCommand(cmdType, form) with
                        | Success cmd -> 
                            bus.Post cmd
                            response.StatusCode <- HttpStatusCode.Accepted                
                            response.Content <- HttpContent.Create("Command " + cmdId + " accepted")
                        | Failure error -> 
                            response.StatusCode <- HttpStatusCode.BadRequest                
                            response.Content <- HttpContent.Create(error)
                | None -> 
                    response.StatusCode <- HttpStatusCode.NotFound                
                    response.Content <- HttpContent.Create("Cannot find command for command ID " + cmdId)                
             
    open Microsoft.ServiceModel.Http
    open Microsoft.ServiceModel.Resource

    type Registration =        
        static member Register(resolver : string -> (Type option), 
                               bus : Zaz.ICommandBus) =
            let routes = System.Web.Routing.RouteTable.Routes
            let config =  { new ResourceConfiguration() with
                                override this.RegisterRequestProcessorsForOperation(operation, processors, node) = ()
                                override this.RegisterResponseProcessorsForOperation(operation, processors, node) = ()                                
                                override this.GetInstance(serviceType, instanceContext, message) =
                                        new CommandBus(resolver, bus) |> unbox }
            RouteCollectionExtensions.AddResourceRoute<CommandBus>(routes, "Commands", config)            

        static member Register(resolver : System.Func<string, Type>, 
                               bus : Zaz.ICommandBus) =

            let resolver2 = fun key -> match resolver.Invoke(key) with
                                        | null -> None
                                        | x -> Some x
            Registration.Register(resolver2, bus)


        