using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TypedConfigProvider
{
    public class ConfigFileLocator : IConfigFileLocator
    {
        private readonly IEnumerable<string> paths;
        private readonly IEnumerable<string> fileExtensions;
        public ConfigFileLocator(params string[] paths)
        {
            this.paths = paths;
            fileExtensions = new[] {"config.json"};
        }

        public IEnumerable<FileInfo> FindConfigFiles()
        {
            return paths.SelectMany(FindConfigFilesInFolder).ToArray();
        }

        private IEnumerable<FileInfo> FindConfigFilesInFolder(string path)
        {
            var dir = new DirectoryInfo(path);

            return fileExtensions.SelectMany(ext => FindConfigFileByMask(dir, ext));
        }

        private static IEnumerable<FileInfo> FindConfigFileByMask(DirectoryInfo dir, string fileExt)
        {
            return dir.GetFiles("*." + fileExt, SearchOption.TopDirectoryOnly);
        }

        public IEnumerable<string> GetSupportedFileExtensions()
        {
            return fileExtensions;
        }
    }
}