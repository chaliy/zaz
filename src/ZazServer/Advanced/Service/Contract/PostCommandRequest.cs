﻿namespace Zaz.Server.Advanced.Service.Contract
{
    public class PostCommandRequest
    {
        public string Key { get; set; }
        public object Command { get; set; }
        public string[] Tags { get; set; }
    }
}
