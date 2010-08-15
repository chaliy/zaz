namespace CommandRouter.Remote.Contract

open System.Xml.Linq
open System.ServiceModel
open System.Runtime.Serialization

[<DataContract(Namespace="urn:org:mir:command-rounter:command-bus-v1.0")>]
type CommandEnvelope() =      
    let mutable key : string = null
    let mutable data : XElement = null
    let mutable tags : string list = []
    
    [<DataMember(Name = "Key", IsRequired = false, Order = 0)>]
    member x.Key with get() = key and set(v) = key <- v
    [<DataMember(Name = "Data", IsRequired = false, Order = 1)>]
    member x.Data with get() = data and set(v) = data <- v
    [<DataMember(Name = "Tags", IsRequired = false, Order = 2)>]
    member x.Tags with get() = tags and set(v) = tags <- v


[<ServiceContract(Namespace = "urn:org:mir:command-rounter:command-bus-v1.0")>]
type CommandBus =    
    [<OperationContract(IsOneWay = true)>]
    abstract member Post : env : CommandEnvelope -> unit

namespace CommandRouter.Remote.Client

    open System.IO
    open System.Xml
    open System.Xml.Linq
    open System.Runtime.Serialization
    open System.ServiceModel
    open CommandRouter.Remote.Contract    

    type RemoteCommandBus(url) =
        let serialize cmd = 
            use mem =  new MemoryStream()
            let ser = new DataContractSerializer(cmd.GetType())
            ser.WriteObject(mem, cmd)
            mem.Position <- int64 0
            XElement.Load(new XmlTextReader(mem))    
        interface CommandRouter.ICommandBus with
            member this.Post(cmd) =
                let envelope = CommandEnvelope()
                envelope.Key <- cmd.GetType().Name
                envelope.Data <- serialize cmd
                envelope.Tags <- List.empty
                
                let factory = new ChannelFactory<CommandBus>(WSHttpBinding())
                let channel = factory.CreateChannel(EndpointAddress(url)) 
                channel.Post(envelope)
                ()

namespace CommandRouter.Remote.Server

    open System.IO
    open System.Xml
    open System.Xml.Linq
    open System.Runtime.Serialization
    open CommandRouter.Remote.Contract

    [<AbstractClass>] 
    type CommandBusService() =
        let deserialize (xml : XElement) cmdType =                     
            let ser = new DataContractSerializer(cmdType)
            ser.ReadObject(xml.CreateReader())

        interface CommandBus with
            member this.Post(env) = 
                let cmdType = this.ResolveCommand(env.Key)
                let cmd = deserialize env.Data cmdType
                let bus = this.CreateCommandBus()
                bus.Post(cmd)

        
        abstract member CreateCommandBus : unit -> CommandRouter.ICommandBus
        abstract member ResolveCommand : key : string -> System.Type
