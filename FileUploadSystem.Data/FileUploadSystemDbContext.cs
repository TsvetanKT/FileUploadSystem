namespace FileUploadSystem.Data
{
    using System.Data.Entity;

    using FileUploadSystem.Models;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class FileUploadSystemDbContext : IdentityDbContext<User>
    {
        public FileUploadSystemDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public virtual IDbSet<FileEntity> Files { get; set; }

        
        public virtual IDbSet<DirectoryEntity> Directories { get; set; }

        public static FileUploadSystemDbContext Create()
        {
            return new FileUploadSystemDbContext();
        }
    }
}
