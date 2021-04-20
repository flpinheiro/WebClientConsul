# README

```xml
<PackageReference Include="Consul" Version="1.6.1.1" />
```

```json
   "Consul": {
    "Host": "http://consul:8500/",
    "Enabled": true,
    "Service": "WebClientConsul",
    "address": "webclientconsul",
    "Port": 80,
    "PingEnabled": true,
    "Tags": [ "consul", "client", "web" ],
    "MetaData": {
            "type": "client",
            "Company": "Compuletra",
            "Enviroment": "Development"
    }
  }
```

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddConsul(Configuration);
}
```

```c#
public void Configure(IApplicationBuilder app, ILogger<Startup> logger)
{
//for brevity
    app.UseConsul();
//for brevity
}
```
