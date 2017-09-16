[![Build status](https://ci.appveyor.com/api/projects/status/8siydly69qfbk1bb?svg=true)](https://ci.appveyor.com/project/iiwaasnet/config-provider)
[![NuGet version](https://badge.fury.io/nu/TypedConfigProvider.svg)](https://badge.fury.io/nu/TypedConfigProvider)

# TypedConfigProvider
During its life-cycle, software is deployed on multiple environments: developer's machine, test stage, production servers, etc. Each environment may require different configuration to be provided for the software to work properly.
**TypedConfigProvider** makes management of environment-dependent configuration simple:
* create default configuration section
* provide only needed overrides for each of the environments
* set the environment sequence, i.e. order in which configuration sections should be applied

### How to start
Let's assume, for the below configuration the sequence of environments is *"prod, dev"*:
```json
// Sample.config.json
{
    "prod": {
        "stringProp": "value",
        "intProp": 1
    },
    "dev": {
        "stringProp": "dev-override-value"
    }
}
```
Final configuration, available for application, will look like the following:
```json
{
    "stringProp": "dev-override-value",
    "intProp": 1
}
```

Environment sequence may be provided, for instance, via *app.config* or *Web.config* files. *AppConfigTargetProvider* class implements reading of the environment sequence from *app.config* file.
```xml
<appSettings>
  <!-- override PROD settings with DEV ones  -->
  <add key="JsonConfig.EnvironmentSequence" value="PROD, DEV" />
</appSettings>
```



Application configuration class for the example above should look like this:
```csharp
public class SampleConfiguration
{
  public string StringProp {get; set;}
  public int IntProp {get; set;}
}
```

### Naming and other conventions
Default directory to search for configuration files - *config* subfolder of the main executable's directory. You can override it by providing a list of directories to search for via *ConfigFileLocator* ctor.

All configuration classes should have their names ending with *Configuration* suffix. Corresponding config files should be named without *Configuration* suffix and have extension *.config.json:
* Class name => **MyApplication**Configuration
* File name => **MyApplication**.config.json


Sequence of configuration sections in **.config.json* file is not important, as they are applied based on provided environment sequence.

### How to handle array properties in configuration
There are some tricks though with array properties. Let's assume the following environment sequence *"prod, dev"* for the sample below:
```json
{
    "prod": {
        "urls": [
            "http://server1.prod",
            "http://server2.prod"
        ]
    },
    "dev": {
        "urls": [
            "http://server.dev"
        ]
    }
}
```
Final configuration will contain **all** three urls configured: two from *"prod"* section and one from *"dev"* section. If this was not your intention, you may introduce kind of **”reset”** (name could be any) section to fix this:

```json
{
    "prod": {
        "urls": [
            "http://server1.prod",
            "http://server2.prod"
        ]
    },
    "dev": {
        "urls": [
            "http://server.dev"
        ]
    },
    "reset": {
        "urls": null
    }
}
```
Sequence *"prod, reset, dev"* gives the following resulting configuration:
```json
{
    "urls": [
       "http://server.dev"
    ]
}
```
--------
Please, look into [Tests](https://github.com/iiwaasnet/config-provider/tree/master/src/Tests) project for more use cases.
