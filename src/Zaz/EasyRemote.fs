namespace Zaz.EasyRemote.Server

    open System
    open System.Net
    open System.ServiceModel
    open System.ServiceModel.Web
    open System.Net.Http
    open Microsoft.ApplicationServer.Http
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
            response.Content <- new StringContent("Endpoint for commands.")

        [<WebInvoke(Method = "POST", UriTemplate = "")>]
        member this.Post(request : HttpRequestMessage, response : HttpResponseMessage) = 
            let body = request.Content.ReadAsString()
            let form = parseQueryString(body)
            if form.ContainsKey("Zaz-Command-Id") = false then
                response.StatusCode <- HttpStatusCode.BadRequest                
                response.Content <- new StringContent("Required value 'Zaz-Command-Id' was not found.")
            else
                let cmdId = form.["Zaz-Command-Id"]
                match resolver cmdId with
                | Some cmdType -> 
                        match Zaz.Utils.BuildCommand(cmdType, form) with
                        | Success cmd ->
                            try 
                                bus.Post cmd
                                response.StatusCode <- HttpStatusCode.Accepted                
                                response.Content <- new StringContent("Command " + cmdId + " accepted")
                            with
                                | x -> 
                                    response.StatusCode <- HttpStatusCode.BadRequest                
                                    response.Content <- new StringContent(x.ToString())
                        | Failure error -> 
                            response.StatusCode <- HttpStatusCode.BadRequest                
                            response.Content <- new StringContent(error)
                | None -> 
                    response.StatusCode <- HttpStatusCode.NotFound                
                    response.Content <- new StringContent("Cannot find command for command ID " + cmdId)                
                 
    open Microsoft.ApplicationServer.Http.Activation
    open Microsoft.ApplicationServer.Http.Description

    type Registration =        
        static member Register(resolver : string -> (Type option), 
                               bus : Zaz.ICommandBus) =
            let routes = System.Web.Routing.RouteTable.Routes
            let commandBusFactory = { new IResourceFactory with
                                        member this.GetInstance(serviceType, instanceContext, request) =
                                            new CommandBus(resolver, bus) |> unbox
                                        member this.ReleaseInstance(instanceContext, service) = () }

            let config = HttpHostConfiguration.Create().SetResourceFactory(commandBusFactory)
                                                    
            RouteCollectionExtensions.MapServiceRoute<CommandBus>(routes, "Commands", config)            

        static member Register(resolver : System.Func<string, Type>, 
                               bus : Zaz.ICommandBus) =

            let resolver2 = fun key -> match resolver.Invoke(key) with
                                        | null -> None
                                        | x -> Some x
            Registration.Register(resolver2, bus)


        