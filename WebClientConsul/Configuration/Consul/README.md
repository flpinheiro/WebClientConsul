```xml
<PackageReference Include="Consul" Version="1.6.1.1" />
```

```json
"Consul": {
    "Host": "http://localhost:8500",
    "Enabled": true,
    "service": "mainservice",
    "address": "localhost",
    "Port": 8082,
    "PingEnabled": false
  },
  "ConsulConfig": {
    "Host": "http://localhost:8500",
    "ServiceName": "catalogService",
    "ServiceId": "catalogService-Id"
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