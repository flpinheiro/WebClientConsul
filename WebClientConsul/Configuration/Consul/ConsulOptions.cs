using System;
using System.Collections.Generic;

namespace WebClientConsul.Configuration.Consul
{
    public class ConsulOptions
    {
        public string Id { get; init; } = Guid.NewGuid().ToString();
        public bool Enabled { get; init; }
        public string Host { get; init; } = "http://localhost:8500/";
        public string Service { get; init; }
        public string Address { get; init; }
        public int Port { get; init; }
        public bool PingEnabled { get; init; }
        public string[] Tags { get; init; }
        public IDictionary<string, string> MetaData { get; init; }
    }
}
