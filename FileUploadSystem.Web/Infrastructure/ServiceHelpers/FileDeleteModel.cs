namespace FileUploadSystem.Web.Infrastructure.ServiceHelpers
{
    public class FileDeleteModel
    {
        public FileDeleteModel(int id, bool isDirectory, string type)
        {
            this.Id = id;
            this.IsDirectory = isDirectory;
            this.Type = type;
        }

        public int Id { get; set; }

        public bool IsDirectory { get; set; }

        public string Type { get; set; }
    }
}