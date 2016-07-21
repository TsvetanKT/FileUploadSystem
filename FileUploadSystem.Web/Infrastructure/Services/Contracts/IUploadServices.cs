namespace FileUploadSystem.Web.Infrastructure.Services.Contracts
{
    using System.Web;

    using FileUploadSystem.Data.Contracts;
    using FileUploadSystem.Models;
    using FileUploadSystem.Web.Infrastructure.ServiceHelpers;
    using FileUploadSystem.Web.ViewModels.Upload;

    public interface IUploadServices
    {
        UploadViewModel GetSearchResults(
            int? id,
            int? page,
            string fileName,
            long? fromSize,
            long? toSize,
            string fileType,
            string dim,
            string sortBy,
            User currentUser,
            int recordsOnPage);

        int? GetUpperDirectory(int currentDirectoryId, User currentUser);

        ServiceReview CreateDirectory(int parentDirectoryId, string directoryName, User currenUser, IFileUploadSystemData data);

        ServiceReview Delete(int fileId, User currenUser, IFileUploadSystemData data, bool isAdminAction);

        ServiceReview Rename(int fileId, string newName, User currenUser, IFileUploadSystemData data, bool isAdminAction);

        ServiceReview UploadFile(HttpPostedFileBase uploadFile, int? currentDirectory, User currenUser, IFileUploadSystemData data);
    }
}
