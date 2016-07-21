namespace FileUploadSystem.Web.Infrastructure.Services.Upload
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Web;

    using AutoMapper.QueryableExtensions;

    using FileUploadSystem.Common;
    using FileUploadSystem.Data;
    using FileUploadSystem.Data.Contracts;
    using FileUploadSystem.Models;
    using FileUploadSystem.Web.Infrastructure.ServiceHelpers;
    using FileUploadSystem.Web.Infrastructure.Services.Base;
    using FileUploadSystem.Web.Infrastructure.Services.Contracts;
    using FileUploadSystem.Web.ViewModels.Upload;

    using PagedList;
    

    public class UploadServices : BaseServices, IUploadServices
    {
        public UploadServices(IFileUploadSystemData data)
            : base(data)
        {
        }

        public UploadViewModel GetSearchResults(
            int? id,
            int? page,
            string fileName,
            long? fromSize,
            long? toSize,
            string fileType,
            string dim,
            string sortBy,
            User currentUser,
            int recordsOnPage)
        {
            long byteMultiplier = AssemblyHelpers.ConvertDimentionLiteralToByteMultiplier(dim);
            long fromSizeInDim = (fromSize ?? 0) * byteMultiplier;
            long toSizeInDim = (toSize ?? 0) * byteMultiplier;

            id = id ?? -1;

            IQueryable<FileEntity> files = null;
            fileName = (fileName ?? string.Empty).ToLower();
            IQueryable<FileViewModel> searchResult = null;
            string searchLocation = null;
            int? currentDirectoryId = 0;

            if (dim == null)
            {
                string currenDirectoryName = string.Empty;
                currentDirectoryId = id;
                if (id == -1)
                {
                    files = currentUser.Files.Where(f => f.LocationId == -1).AsQueryable();
                    currenDirectoryName = "<Main Directory>";
                }
                else
                {
                    files = ((DirectoryEntity)currentUser.Files.Where(f => f.Id == id).FirstOrDefault()).FilesContained.AsQueryable();
                    currenDirectoryName = currentUser.Files.Where(f => f.Id == id).FirstOrDefault().Name;
                }
                
                searchLocation = "Directory - (" + currenDirectoryName + ")";
                searchResult = files.Project().To<FileViewModel>(); ;
            }
            else
            {
                files = currentUser.Files.AsQueryable();

                searchResult = CommonFunc.FileSearchQuery(
                    files, fileName, fileType, fromSizeInDim, toSizeInDim)
                    .Project()
                    .To<FileViewModel>();
            }

            sortBy = sortBy ?? "Name";
            switch (sortBy)
            {
                case "Name desc":
                    searchResult = searchResult.OrderByDescending(f => f.Name);
                    break;
                case "Size desc":
                    searchResult = searchResult.OrderByDescending(f => f.Size);
                    break;
                case "Size":
                    searchResult = searchResult.OrderBy(f => f.Size);
                    break;
                default:
                    searchResult = searchResult.OrderBy(f => f.Name);
                    break;
            }

            var founded = searchResult.ToPagedList(page ?? 1, recordsOnPage);
            var result = new UploadViewModel(founded, searchLocation, currentDirectoryId, sortBy);

            return result;
        }

        public int? GetUpperDirectory(int currentDirectoryId, User currentUser)
        {
            int? id = null;

            if (currentDirectoryId != -1)
            {
                id = currentUser.Files.Where(f => f.Id == currentDirectoryId).FirstOrDefault().LocationId;
            }

            if (id == -1)
            {
                id = null;
            }

            return id;
        }

        public ServiceReview UploadFile(HttpPostedFileBase uploadFile, int? currentDirectory, User currenUser, IFileUploadSystemData data)
        {
            var res = new ServiceReview();
            var currentDirectoryId = currentDirectory ?? -1;
            res.RedirectId = currentDirectory;
            var errorFound = false;

            if (uploadFile == null ||
               uploadFile.ContentLength == 0 ||
               uploadFile.ContentLength > GlobalConstants.MaxUploadFileSize ||
               uploadFile.FileName.Trim().Length < 1)
            {
                res.Message = "Bad file name/size!";
                res.ErrorFound = true;
                return res;
            }

            var fileName = Path.GetFileName(uploadFile.FileName);
            var fileExtension = Path.GetExtension(uploadFile.FileName);

            // Check if the same filename allready exists in current directory
            errorFound = CommonFunc.SameFileExistInSameDir(fileName, currentDirectoryId, currenUser);
            if (errorFound)
            {
                res.ErrorFound = true;
                res.Message = "A file with that name already exists in the current directory!";
                return res;
            }
                
            var newFile = new BinaryFile
            {
                Name = fileName,
                Type = fileExtension,
                Size = uploadFile.ContentLength,
                Owner = currenUser,
                OwnerId = currenUser.Id,
                LocationId = currentDirectoryId
            };

            if (currentDirectory != -1)
            {
                ((DirectoryEntity)currenUser.Files.Where(f => f.Id == currentDirectory).FirstOrDefault()).FilesContained.Add(newFile);
            }
            else
            {
                currenUser.Files.Add(newFile);
            }

            data.SaveChanges();
            int id = newFile.Id;

            FileManipulator.UploadFile(uploadFile, id, fileExtension);

            res.Message = string.Format("File ({0}) uploaded successfully!", fileName);
            return res;
        }

        public ServiceReview CreateDirectory(int parentDirectoryId, string directoryName, User currenUser, IFileUploadSystemData data)
        {
            var errorFound = false;
            string message = null;

            if (!(directoryName != null && directoryName.Length > 0))
            {
                errorFound = true;
                message = "Bad directory name!";
            }
            else if (CommonFunc.SameFileExistInSameDir(directoryName, parentDirectoryId, currenUser))
            {
                errorFound = true;
                message = "Dublicate names in same directory!";
            }
            else
            {
                var newDirectory = new DirectoryEntity
                {
                    Name = directoryName,
                    LocationId = parentDirectoryId,
                    Owner = currenUser,
                    OwnerId = currenUser.Id
                };

                if (parentDirectoryId == -1)
                {
                    currenUser.Files.Add(newDirectory);
                }
                else
                {
                    ((DirectoryEntity)(currenUser.Files
                        .Where(f => f.Id == parentDirectoryId)
                        .FirstOrDefault()))
                        .FilesContained.Add(newDirectory);
                }

                data.SaveChanges();
            }

            if (!errorFound)
            {
                message = "Directory (" + directoryName + ") created!";
            }

            return new ServiceReview(message, parentDirectoryId, errorFound);
        }

        public ServiceReview Rename(int fileId, string newName, User currenUser, IFileUploadSystemData data, bool isAdminAction)
        {
            var errorFound = true;
            string message = null;

            FileEntity file = null;

            if (isAdminAction)
            {
                file = data.Files.GetById(fileId);
            }
            else
            {
                file = currenUser.Files.Where(i => i.Id == fileId).FirstOrDefault();
            }
            
            int containingDirectoryId = file.LocationId;

            if (newName == null || newName.Trim().Length == 0)
            {
                message = "Bad name!";
            }
            else if (file.Name == newName)
            {
                message = "Cannot rename to the same name!";
            }
            else if (CommonFunc.SameFileExistInSameDir(newName, containingDirectoryId, currenUser))
            {
                message = "File with that name already exists in same directory!";
            }
            else
            {
                // If we are changing the file type - .jpg to.png
                // To enable - FileManipulator must rename the file on disk too, or remove all file types
                var binFile = file as BinaryFile;
                if (binFile != null)
                {
                    var newType = Path.GetExtension(newName);
                    var areTypesMatch = newType.ToLower() == binFile.Type.ToLower();
                    if (!areTypesMatch)
                    {
                        message = string.Format("Changing file types is currently forbidden ({0} to {1})!", binFile.Type, newType);
                        return new ServiceReview(message, containingDirectoryId, errorFound);
                    }
                }

                var oldName = file.Name;
                file.Name = newName;

                data.Context.Entry(file).State = EntityState.Modified;
                data.SaveChanges();

                errorFound = false;
                message = string.Format("File ({0}) renamed to ({1})!", oldName, newName);
            }

            return new ServiceReview(message, containingDirectoryId, errorFound);
        }

        public ServiceReview Delete(int fileId, User currenUser, IFileUploadSystemData data, bool isAdminAction)
        {
            FileEntity file = null;
            if (isAdminAction)
            {
                file = data.Files.GetById(fileId);
            }
            else
            {
                file = currenUser.Files.Where(f => f.Id == fileId).FirstOrDefault();;
            }

            string message = string.Empty;

            if ((file as DirectoryEntity) != null)
            {
                var childrenFiles = DFSNestedElementFinder(currenUser, fileId);
                foreach (var element in childrenFiles)
                {
                    this.Data.Files.Delete(element.Id);

                    // If element is a Binary file it must be deleted from the file system too
                    if (element.IsDirectory == false)
                    {
                        FileManipulator.DeleteFile(element.Id, element.Type);
                    }
                }

                message = string.Format("Directory ({0}) deleted successfully!", file.Name);
            }
            else
            {
                var binFile = file as BinaryFile;
                FileManipulator.DeleteFile(binFile.Id, binFile.Type);

                this.Data.Files.Delete(fileId);
                message = string.Format("File ({0}) deleted successfully!", binFile.Name);
            }

            this.Data.SaveChanges();

            return new ServiceReview(message, file.LocationId, false);
        }

        // Using Depth-First Search (DFS) algorithm
        public static IList<FileDeleteModel> DFSNestedElementFinder(User userData, int rootId)
        {
            var result = new List<FileDeleteModel>();
            var stack = new Stack<int>();
            stack.Push(rootId);

            while (stack.Any())
            {
                var currentFileId = stack.Pop();
                var nextFile = userData.Files.Where(f => f.Id == currentFileId).FirstOrDefault();
                var nextId = nextFile.Id;
                result.Add(new FileDeleteModel(nextId, false, null));

                var nextDir = nextFile as DirectoryEntity;
                if (nextDir != null)
                {
                    result[result.Count - 1].IsDirectory = true;
                    foreach (var child in nextDir.FilesContained)
                    {
                        stack.Push(child.Id);
                    }
                }
                else
                {
                    result[result.Count - 1].Type = (nextFile as BinaryFile).Type;
                }
            }

            return result.AsEnumerable().Reverse().ToList();
        }
    }
}