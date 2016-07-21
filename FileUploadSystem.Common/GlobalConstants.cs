namespace FileUploadSystem.Common
{
    public class GlobalConstants
    {
        public const string AdminRole = "Admin";

        public const int RecordsPerPage = 10;

        public const int NumberOfUploadSubdirectoryes = 10;

        // Size in bytes, not bits
        public const int MaxUploadFileSize = 1000 *1000;

        public static string UploadDirectory { get; set; }

    }
}
