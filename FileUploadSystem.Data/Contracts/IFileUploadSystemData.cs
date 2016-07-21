namespace FileUploadSystem.Data.Contracts
{
    using System;
    using System.Data.Entity;
    using FileUploadSystem.Models;

    public interface IFileUploadSystemData
    {
        DbContext Context { get; }

        IRepository<FileEntity> Files { get; }

        IRepository<DirectoryEntity> Directories { get; }

        IRepository<User> Users { get; }

        void Dispose();

        int SaveChanges();
    }
}
