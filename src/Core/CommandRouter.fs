namespace Zaz
    
type ICommandBus =
   abstract member Post : obj -> unit