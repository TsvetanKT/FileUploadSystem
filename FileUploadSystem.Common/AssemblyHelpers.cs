namespace FileUploadSystem.Common
{
    using System;
    using System.IO;
    using System.Reflection;

    public static class AssemblyHelpers
    {
        public static string GetDirectoryForAssembyl(Assembly assembly)
        {
            var assemblyLocation = assembly.CodeBase;
            var location = new UriBuilder(assemblyLocation);
            var path = Uri.UnescapeDataString(location.Path);
            var directory = Path.GetDirectoryName(path);
            return directory;
        }

        public static string UploadDirectoryLocation(Assembly assembly)
        {
            var directory = AssemblyHelpers.GetDirectoryForAssembyl(assembly);
            DirectoryInfo dir = new DirectoryInfo(directory);
            return dir.Parent.FullName + "\\Uploads";
        }

        public static string ConvertFileSizeToReadableFormat(long fileSize)
        {
            if (fileSize < 0)
            {
                return "Directory";
            }

            double len = (double)fileSize;
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            while (len >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                len = len / 1024;
            }

            return String.Format("{0:0.##} {1}", len, sizes[order]);
        }

        public static long ConvertDimentionLiteralToByteMultiplier(string dimentionLiteral)
        {
            long byteMultiplier;

            switch (dimentionLiteral)
            {
                case "Bytes": byteMultiplier = 1; break;
                case "KB": byteMultiplier = 1024; break;
                case "MB": byteMultiplier = 1024 * 1024; break;
                case "GB": byteMultiplier = 1024 * 1024 * 1024; break;
                default: byteMultiplier = 1024 * 1024; break;
            }

            return byteMultiplier;
        }
    }
}
