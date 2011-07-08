Zaz Command Bus
=============

Super simple HTTP Command Bus. Based on simple REST protocol. Very opinionated. Provides minimum configuaration, but maximum extensibility points. REST protocol is implemented with <a href="http://wcf.codeplex.com/wikipage?title=WCF HTTP">WCF Web API</a>. JSON serialization provided by JSON.NET.

Features
========

1. Send and Recieve commands over HTTP

Example
=======

Client is simple (more details in SampleApp):

	var bus = new CommandBus("http://localhost:9302/");            
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
            ZazRegistration.Init(
                new LocalCommandBroker(typeof(SampleHandlersMarker).Assembly), 
                new Conventions
                {
                    CommandResolver = CommandRegistry.ResolveCommand2
                });
        }        
    }

Installation
============

To install both Client and Server

	Install-Package Zaz
	
or only Client

	Install-Package ZazClient
	
or only Server

	Install-Package ZazServer
	
License
=======

Licensed under the MIT
