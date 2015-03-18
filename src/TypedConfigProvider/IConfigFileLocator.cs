using System.Collections.Generic;
using System.IO;

namespace TypedConfigProvider
{
    public interface IConfigFileLocator
    {
        IEnumerable<FileInfo> FindConfigFiles();
        IEnumerable<string> GetSupportedFileExtensions();
    }
}