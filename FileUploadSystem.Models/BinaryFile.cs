namespace FileUploadSystem.Models
{
    public class BinaryFile : FileEntity
    {
        /// <summary>
        /// Size in bytes
        /// </summary>
        public long Size { get; set; }

        public string Type { get; set; }
    }
}
