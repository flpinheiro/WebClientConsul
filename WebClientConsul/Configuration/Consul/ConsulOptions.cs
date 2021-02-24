﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClientConsul.Configuration.Consul
{
    public class ConsulOptions
    {
        public string Id { get; set; }
        public bool Enabled { get; set; }
        public string Host { get; set; }
        public string Service { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public bool PingEnabled { get; set; }
        public string[] Tags { get; set; }
        public IDictionary<string, string> MetaData { get;  set; }
    }
}
