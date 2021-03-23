using System;
using System.Collections.Generic;

namespace WebClientConsul.Configuration.Consul
{
    public class ConsulOptions
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool Enabled { get; set; }
        public string Host { get; set; } = "http://127.0.0.1:8500/";
        public string Service { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public bool PingEnabled { get; set; }
        public string[] Tags { get; set; }
        public IDictionary<string, string> MetaData { get; set; }
    }
}
