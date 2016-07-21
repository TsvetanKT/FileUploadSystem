namespace FileUploadSystem.Web.Infrastructure.ServiceHelpers
{
    using System.Linq;

    using FileUploadSystem.Models;

    public static class CommonFunc
    {
        public static IQueryable<FileEntity> FileSearchQuery(
            IQueryable<FileEntity> allFiles, string fileName, string fileType, long fromSizeInDim, long toSizeInDim)
        {
            var searchResult = allFiles
                    .Where
                    (f =>
                        (f.Name.ToLower().Contains(fileName) || fileName == string.Empty) &&
                        (fileType == "FilesAndDirectoryes" || fileType == null ?
                            ((f as BinaryFile) == null) ||
                                (((f as BinaryFile) != null &&
                                ((f as BinaryFile).Size >= fromSizeInDim || fromSizeInDim == 0) &&
                                ((f as BinaryFile).Size <= toSizeInDim || toSizeInDim == 0))) :
                        (fileType == "Directoryes" ?
                            ((f as BinaryFile) == null) :
                        (((f as BinaryFile) != null) &&
                            ((f as BinaryFile).Size >= fromSizeInDim || fromSizeInDim == 0) &&
                            ((f as BinaryFile).Size <= toSizeInDim || toSizeInDim == 0))
                    )));

            return searchResult;
        }

        public static bool SameFileExistInSameDir(string fileName, int directoryId, User currenUser)
        {
            bool noSameFiles = false;

            var sameFiles = currenUser.Files.Where(f => (f.Name == fileName) && (f.LocationId == directoryId)).FirstOrDefault();
            if (sameFiles != null)
            {
                noSameFiles = true;
            }

            return noSameFiles;
        }
    }
}