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
        private readonly string searchPath;
        private readonly IEnumerable<string> targets;
        private readonly ConcurrentDictionary<Type, object> configurations;
        private readonly ConcurrentDictionary<string, ConfigFileMetadata> metadatas;
        private volatile bool configLoaded;
        private readonly object @lock = new object();
        private static readonly JsonSerializerSettings jsonSerializerSettings;
        private readonly IConfigFileReader configFileReader;
        private readonly IConfigFileLocator configFileLocator;

        static ConfigProvider()
            => jsonSerializerSettings = new JsonSerializerSettings
                                        {
                                            ContractResolver = new CamelCasePropertyNamesContractResolver(),
                                            MissingMemberHandling = MissingMemberHandling.Error,
                                            DateFormatHandling = DateFormatHandling.IsoDateFormat,
                                            DateParseHandling = DateParseHandling.None,
                                            DateTimeZoneHandling = DateTimeZoneHandling.Utc
                                        };

        public ConfigProvider(IConfigTargetProvider targetProvider)
            : this(targetProvider, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultConfigDir), null, null)
        {
        }

        public ConfigProvider(IConfigTargetProvider targetProvider,
                              string searchPath,
                              IConfigFileReader configFileReader = null)
            : this(targetProvider, searchPath, null, configFileReader)
        {
        }

        public ConfigProvider(IConfigTargetProvider targetProvider,
                              IConfigFileLocator configFileLocator,
                              IConfigFileReader configFileReader = null)
            : this(targetProvider, null, configFileLocator, configFileReader)
        {
        }

        private ConfigProvider(IConfigTargetProvider targetProvider,
                               string searchPath,
                               IConfigFileLocator configFileLocator,
                               IConfigFileReader configFileReader)
        {
            this.searchPath = searchPath;
            targets = NormalizeTargets(targetProvider?.GetTargetsSequence() ?? Enumerable.Empty<string>());
            configurations = new ConcurrentDictionary<Type, object>();
            metadatas = new ConcurrentDictionary<string, ConfigFileMetadata>();
            configLoaded = false;
            this.configFileLocator = configFileLocator ?? new ConfigFileLocator(this.searchPath);
            this.configFileReader = configFileReader ?? new ConfigFileReader();
        }

        public T GetConfiguration<T>()
            where T : class, new()
        {
            AssertTargetsIsSet(targets);

            CheckLoadConfiguration();

            T config;
            if ((config = TryGetCachedConfiguration<T>()) == null)
            {
                config = TryGetTypedConfiguration<T>(targets);

                configurations[typeof(T)] = config
                                            ?? ThrowConfigNotFoundException<T>(targets);
            }

            return config;
        }

        public T GetConfigurationForTargets<T>(params string[] targets)
            where T : class, new()
        {
            AssertTargetsIsSet(targets);

            CheckLoadConfiguration();

            return TryGetTypedConfiguration<T>(NormalizeTargets(targets))
                   ?? ThrowConfigNotFoundException<T>(targets);
        }

        private static void AssertTargetsIsSet(IEnumerable<string> targets)
        {
            if (targets == null || !targets.Any())
            {
                throw new ArgumentException("targets is empty or null!");
            }
        }

        private T ThrowConfigNotFoundException<T>(IEnumerable<string> targets)
            where T : class, new()
            => throw new Exception($"Unable to get configuration of type {typeof(T).Name}! "
                                   + $"Configuration targets checked: {string.Join(",", targets)}, "
                                   + $"path checked: [{searchPath}] ."
                                   + $"Missing {TypeToSectionName(typeof(T))}."
                                   + $"[{string.Join("|", configFileLocator.GetSupportedFileExtensions())}]?");

        private static IEnumerable<string> NormalizeTargets(IEnumerable<string> targets)
            => targets.Select(t => t.ToLower().Trim());

        private T TryGetCachedConfiguration<T>()
        {
            configurations.TryGetValue(typeof(T), out var tmpConfig);

            return (T) tmpConfig;
        }

        private T TryGetTypedConfiguration<T>(IEnumerable<string> targets)
            where T : class, new()
        {
            var config = default(T);

            foreach (var target in targets)
            {
                foreach (var section in metadatas.Values
                                                 .Where(v => v.ConfigName == TypeToSectionName(typeof(T)))
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

        private static string TypeToSectionName(Type type)
            => type.Name.ToLower().Replace(ConfigClassNameSuffix, string.Empty);

        private static T Clone<T>(T source)
            where T : class
            => (source != null)
                   ? JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source))
                   : null;

        private void LoadConfiguration()
        {
            var configFiles = configFileLocator.FindConfigFiles();

            AssertFilesFound();

            foreach (var configFile in configFiles)
            {
                var metadata = configFileReader.Parse(configFile);
                metadatas[metadata.ConfigName] = metadata;
            }

            void AssertFilesFound()
            {
                if (!configFiles.Any())
                {
                    throw new Exception($"No config files found in [{searchPath}]!");
                }
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