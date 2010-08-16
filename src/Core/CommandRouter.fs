namespace CommandRouter
    
type ICommandBus =
   abstract member Post : obj -> unit