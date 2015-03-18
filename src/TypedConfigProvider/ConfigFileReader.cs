using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TypedConfigProvider
{
    public class ConfigFileReader : IConfigFileReader
    {
        private static readonly JsonSerializerSettings jsonSerializerSettings;

        static ConfigFileReader()
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

        public ConfigFileMetadata Parse(FileInfo configFile)
        {
            using (var reader = new StreamReader(configFile.FullName))
            {
                var content = reader.ReadToEnd();

                var sections = JsonConvert.DeserializeObject<Dictionary<string, object>>(content, jsonSerializerSettings);

                return new ConfigFileMetadata
                       {
                           ConfigName = configFile.Normalize(),
                           ConfigFile = configFile,
                           Sections = sections.Keys.Select(k => new ConfigSections
                                                                {
                                                                    Target = k,
                                                                    SectionData = sections[k].ToString()
                                                                })
                       };
            }
        }
    }
}