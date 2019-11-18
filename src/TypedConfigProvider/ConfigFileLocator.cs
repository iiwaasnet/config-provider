using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TypedConfigProvider
{
    public class ConfigFileLocator : IConfigFileLocator
    {
        private readonly string path;
        private readonly IEnumerable<string> fileExtensions;

        public ConfigFileLocator(string path)
        {
            this.path = path;
            fileExtensions = new[] {"config.json"};
        }

        public IEnumerable<FileInfo> FindConfigFiles()
            => FindConfigFilesInFolder().ToArray();

        private IEnumerable<FileInfo> FindConfigFilesInFolder()
            => fileExtensions.SelectMany(ext => FindConfigFileByMask(new DirectoryInfo(path), ext));

        private static IEnumerable<FileInfo> FindConfigFileByMask(DirectoryInfo dir, string fileExt)
            => dir.GetFiles("*." + fileExt, SearchOption.TopDirectoryOnly);

        public IEnumerable<string> GetSupportedFileExtensions()
            => fileExtensions;
    }
}