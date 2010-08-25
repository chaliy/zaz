namespace Zaz.Remote.Contract

open System.Xml.Linq
open System.ServiceModel
open System.Runtime.Serialization

[<DataContract(Namespace="urn:org:zaz:command-bus-v1.0")>]
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

[<ServiceContract(Namespace = "urn:org:zaz:command-bus-v1.0")>]
type CommandBus =    
    [<OperationContract>]
    abstract member Post : env : CommandEnvelope -> unit

namespace Zaz.Remote.Client

    open System.IO
    open System.Xml
    open System.Xml.Linq
    open System.Runtime.Serialization
    open System.ServiceModel
    open Zaz.Remote.Contract    

    type RemoteCommandBus(url) =
        let serialize cmd = 
            use mem =  new MemoryStream()
            let ser = new DataContractSerializer(cmd.GetType())
            ser.WriteObject(mem, cmd)
            mem.Position <- int64 0
            XElement.Load(new XmlTextReader(mem))    
        interface Zaz.ICommandBus with
            member this.Post(cmd) =
                let cmdKey = cmd.GetType().FullName
                printfn "Posting command %s" cmdKey
                let envelope = CommandEnvelope(
                                    Key = cmdKey,
                                    Data = serialize cmd,
                                    Tags = List.empty
                                )
                
                let factory = new ChannelFactory<CommandBus>(WSHttpBinding())
                let channel = factory.CreateChannel(EndpointAddress(url)) 
                channel.Post(envelope)

namespace Zaz.Remote.Server

    open System.IO
    open System.Xml
    open System.Xml.Linq
    open System.Runtime.Serialization
    open Zaz.Remote.Contract

    [<AbstractClass>] 
    type CommandBusService() =
        let deserialize (xml : XElement) cmdType =
            let ser = new DataContractSerializer(cmdType)
            ser.ReadObject(xml.CreateReader())

        interface CommandBus with
            member this.Post(env) = 
                let bus = this.CreateCommandBus()
                let cmdTypes = this.ResolveCommand(env.Key)
                cmdTypes
                |> Seq.map(fun t -> deserialize env.Data t)
                |> Seq.iter(bus.Post)
        
        abstract member CreateCommandBus : unit -> Zaz.ICommandBus
        abstract member ResolveCommand : key : string -> System.Type seq
