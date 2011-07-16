using System;
using System.Net.Http;
using System.ServiceModel;
using Microsoft.ApplicationServer.Http.Description;

namespace Zaz.Server.Advanced.Service
{
    class CommandsServiceResourceFactory : IResourceFactory
    {            
        private readonly CommandsService _instance;

        public CommandsServiceResourceFactory(CommandsService instance)
        {
            _instance = instance;
        }

        public object GetInstance(Type serviceType, InstanceContext instanceContext, HttpRequestMessage request)
        {                
            return _instance;
        }

        public void ReleaseInstance(InstanceContext instanceContext, object service)
        {                
        }
    }
}