namespace CommandRouter

open System.Web
open System.Web.Routing
    
type RegistrationConfig = {
    Url : string
    CommandResolveStrategy : RegistrationConfig -> obj
    CommandHandlerResolveStrategy : RegistrationConfig -> obj
    CommandHandlerCreateStrategy : RegistrationConfig -> obj
}

type CommandRoute(config : RegistrationConfig) =
    inherit RouteBase()

    let httpHandler = { new IHttpHandler with
                            member this.ProcessRequest(ctx : HttpContext) = ()
                            member this.IsReusable = false }

    let routeHandler = { new IRouteHandler with
                            member this.GetHttpHandler(ctx : RequestContext) : IHttpHandler = httpHandler }
    
    let route = Route(config.Url, routeHandler)
    
    override this.GetRouteData(ctx) = route.GetRouteData(ctx)
    override this.GetVirtualPath(ctx, values) = route.GetVirtualPath(ctx, values)
    
    new() = CommandRoute({
                            Url = "Commands"
                            CommandResolveStrategy = fun c -> box "sdf"
                            CommandHandlerResolveStrategy = fun c -> box "sdf"
                            CommandHandlerCreateStrategy = fun c -> box "sdf"
                        })

    
