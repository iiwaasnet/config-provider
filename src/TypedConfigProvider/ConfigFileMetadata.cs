using System.Collections.Generic;
using System.IO;

namespace TypedConfigProvider
{
    public class ConfigFileMetadata
    {
        public string ConfigName { get; set; }

        public IEnumerable<ConfigSections> Sections { get; set; }

        public FileInfo ConfigFile { get; set; }
    }
}