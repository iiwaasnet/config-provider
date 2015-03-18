using System.IO;

namespace TypedConfigProvider
{
    public interface IConfigFileReader
    {
        ConfigFileMetadata Parse(FileInfo configFile);
    }
}