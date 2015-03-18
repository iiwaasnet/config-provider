using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TypedConfigProvider
{
    public class ConfigProvider : IConfigProvider
    {
        private const string DefaultConfigDir = "config";
        private const string ConfigClassNameSuffix = "configuration";
        private readonly string baseDir;
        private readonly IEnumerable<string> targets;
        private readonly ConcurrentDictionary<Type, object> configurations;
        private readonly ConcurrentDictionary<string, ConfigFileMetadata> metadatas;
        private volatile bool configLoaded;
        private readonly object @lock = new object();
        private static readonly JsonSerializerSettings jsonSerializerSettings;
        private readonly IConfigFileReader configFileReader;
        private readonly IConfigFileLocator configFileLocator;

        static ConfigProvider()
        {
            jsonSerializerSettings = new JsonSerializerSettings
                                     {
                                         ContractResolver = new CamelCasePropertyNamesContractResolver(),
                                         MissingMemberHandling = MissingMemberHandling.Error,
                                         DateFormatHandling = DateFormatHandling.IsoDateFormat,
                                         DateParseHandling = DateParseHandling.None,
                                         DateTimeZoneHandling = DateTimeZoneHandling.Utc
                                     };
        }

        public ConfigProvider(IConfigTargetProvider targetProvider)
            : this(targetProvider, DefaultConfigDir)
        {
        }

        public ConfigProvider(IConfigTargetProvider targetProvider,
                              string configBaseDir,
                              IConfigFileReader configFileReader = null,
                              IConfigFileLocator configFileLocator = null)
        {
            baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configBaseDir);
            targets = targetProvider.GetTargetsSequence().Select(t => t.ToLower().Trim());
            configurations = new ConcurrentDictionary<Type, object>();
            metadatas = new ConcurrentDictionary<string, ConfigFileMetadata>();
            configLoaded = false;
            this.configFileLocator = configFileLocator ?? new ConfigFileLocator(baseDir);
            this.configFileReader = configFileReader ?? new ConfigFileReader();
        }

        public T GetConfiguration<T>()
            where T : class, new()
        {
            var target = targets.First().ToLower();

            CheckLoadConfiguration();

            T config;
            if ((config = TryGetCachedConfiguration<T>()) == null)
            {
                config = TryGetTypedConfiguration<T>();
                if (config == null)
                {
                    throw new Exception(string.Format("Unable to get configuration of type {0}! Missing {1}.[{2}]?", 
                        typeof (T).Name,
                        TypeToSectionName(typeof (T)),
                        string.Join("|", configFileLocator.GetSupportedFileExtensions())));
                }

                configurations[typeof (T)] = config;
            }

            return config;
        }

        private T TryGetCachedConfiguration<T>()
        {
            object tmpConfig;
            configurations.TryGetValue(typeof (T), out tmpConfig);

            return (T) tmpConfig;
        }

        private T TryGetTypedConfiguration<T>()
            where T : class, new()
        {
            var config = default(T);

            foreach (var target in targets)
            {
                foreach (var section in metadatas.Values
                                                 .Where(v => v.ConfigName == TypeToSectionName(typeof (T)))
                                                 .SelectMany(v => v.Sections)
                                                 .Where(s => s.Target == target))
                {
                    var tmp = Clone(config) ?? new T();

                    JsonConvert.PopulateObject(section.SectionData, tmp, jsonSerializerSettings);

                    config = tmp;
                }
            }

            return config;
        }

        private string TypeToSectionName(Type type)
        {
            return type.Name.ToLower().Replace(ConfigClassNameSuffix, string.Empty);
        }

        private static T Clone<T>(T source)
            where T : class
        {
            return (source != null)
                       ? JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source))
                       : source;
        }

        private void LoadConfiguration()
        {
            var configFiles = configFileLocator.FindConfigFiles();

            foreach (var configFile in configFiles)
            {
                var metadata = configFileReader.Parse(configFile);
                metadatas[metadata.ConfigName] = metadata;
            }
        }

        private void CheckLoadConfiguration()
        {
            if (!configLoaded)
            {
                lock (@lock)
                {
                    if (!configLoaded)
                    {
                        LoadConfiguration();
                        configLoaded = true;
                    }
                }
            }
        }
    }
}