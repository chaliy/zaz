Zaz Command Endpoint
=============

IMPORTANT : For now I am assuming that I am the only user of this lib, so contracts are changing without any notice or backward compatibility. If you are intersting in using this lib, let me know.

Simple HTTP Command Endpoint. Based on simple HTTP protocol. Very opinionated. Provides minimum configuaration, but maximum extensibility points. One of the goals is to have multiple clients like .NET client, PowerShell and JavaScript.

HTTP protocol is implemented with <a href="http://wcf.codeplex.com/wikipage?title=WCF HTTP">WCF Web API</a>. JSON serialization provided by JSON.NET.

Features
========

1. Send and Recieve commands over HTTP

Example
=======

Client is simple (more details in SampleApp):

	var bus = new CommandBus("http://localhost:9302/Commands/");            
	bus.Post(new PrintMessage
				{
					Message = "Hello world"
				});
				
Server side (more details in SampleEasyRemoteApp):

	// Global.asax
	public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {            
		    RouteTable.Routes.MapCommandsService("Commands");            
        }        
    }

Installation
============
	
To install Client execute:

	Install-Package ZazClient
	
To install Server execute:

	Install-Package ZazServer
	
If you need other distribution, pls contact me.

Upgrade
=======

In case if you have updated to the latest version you would have compilation errors. The following changes been introduced while upgraded the project to the new Web API.

Instead of routes based initialization

    RouteTable.Routes.MapCommandsService("Commands",
        new ServerConfiguration
        {
            Registry = registry,
            Broker = new NventreeCommandBroker()
        });

New central initialization class been introduced
	
    ZazServer.ConfigureAsWebHost("Commands", new ServerConfiguration
    {
        Registry = registry,
        Broker = new NventreeCommandBroker()
    });

License
=======

Licensed under the MIT