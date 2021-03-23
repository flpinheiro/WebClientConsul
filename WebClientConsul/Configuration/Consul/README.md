```xml
<PackageReference Include="Consul" Version="1.6.1.1" />
```

```json
  "Consul": {
    "Id": "WebClientConsulId", //optional
    "Host": "http://localhost:8500",
    "Enabled": true,
    "service": "WebClientConsul",
    "address": "localhost",
    "Port": 8082,
    "PingEnabled": true,
    "Tags": [ "consul", "client", "web" ],
    "MetaData": {
      "type": "MyJsonDictionaryOfstringanyType:#Json_Dictionary_Test",
      "Street": "30 Rockefeller Plaza",
      "City": "New York City",
      "State": "NY"
    }
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