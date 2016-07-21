namespace FileUploadSystem.Models
{
    using System.Collections.Generic;

    public class DirectoryEntity : FileEntity
    {
        private ICollection<FileEntity> filesContained;

        public DirectoryEntity()
        {
            this.filesContained = new HashSet<FileEntity>();
        }

        public virtual ICollection<FileEntity> FilesContained
        {
            get { return this.filesContained;}
            set { this.filesContained = value; }
        }
    }
}
