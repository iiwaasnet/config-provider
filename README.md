[![Build status](https://ci.appveyor.com/api/projects/status/8siydly69qfbk1bb?svg=true)](https://ci.appveyor.com/project/iiwaasnet/config-provider)

# config-provider
Typed configuration provider with default JSON configuration reader


#### Sample configuration file: Sample.config.json
```json
{
    "dev": {
        "stringProp": "dev-override-value"
    },
    "prod": {
        "stringProp": "value",
        "intProp": 1
    }
}
```

#### Sample app.config/web.config file section
```xml
<appSettings>
  <!-- override PROD settings with DEV ones  -->
  <add key="JsonConfig.EnvironmentSequence" value="PROD, DEV" />
</appSettings>
```

#### C# mapping class
```csharp
public class SampleConfiguration
{
  public string StringProp {get; set;}
  public int IntProp {get; set;}
}
```

Please, look into Tests project for more usage cases.
