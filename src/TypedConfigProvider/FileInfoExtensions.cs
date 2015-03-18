using System.IO;

namespace TypedConfigProvider
{
    public static class FileInfoExtensions
    {
        public static string Normalize(this FileInfo fileInfo)
        {
            return fileInfo.Name
                           .Split('.')[0]
                .Replace("-", "")
                .Replace("_", "")
                .ToLower();
        }
    }
}