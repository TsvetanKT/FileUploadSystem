namespace FileUploadSystem.Web.Infrastructure.Services.Search
{
    using System;
    using System.Linq;

    using AutoMapper.QueryableExtensions;
    
    using FileUploadSystem.Common;
    using FileUploadSystem.Models;
    using FileUploadSystem.Web.Infrastructure.Services.Base;
    using FileUploadSystem.Web.Infrastructure.Services.Contracts;
    using FileUploadSystem.Web.ViewModels.Search;
    using FileUploadSystem.Web.Infrastructure.ServiceHelpers;
    using FileUploadSystem.Data.Contracts;

    using PagedList;
    
    public class SearchServices : BaseServices, ISearchServices
    {
        public SearchServices(IFileUploadSystemData data)
            : base(data)
        {
        }

        public SearchViewModel GetSearchResults(
            object id, 
            int? page, 
            string locationName, 
            string fileName, 
            long? fromSize, 
            long? toSize, 
            string fileType, 
            string dim, 
            string sortBy,
            int recordsOnPage)
        {
            long byteMultiplier = AssemblyHelpers.ConvertDimentionLiteralToByteMultiplier(dim);

            long fromSizeInDim = (fromSize ?? 0) * byteMultiplier;
            long toSizeInDim = (toSize ?? 0) * byteMultiplier;

            IQueryable<FileEntity> files = null;
            int dirId;
            Guid userId;
            string searchLocation = null;
            bool inUserPage = false;

            // When we are searching in directory
            if (int.TryParse(id.ToString(), out dirId))
            {
                files = ((DirectoryEntity)this.Data.Files.GetById(dirId)).FilesContained.AsQueryable();
                searchLocation = "Directory " + locationName;
            }
            // When we are searching files of specific user
            else if (Guid.TryParse(id.ToString(), out userId))
            {
                if (dim == null)
                {
                    files = this.Data.Users.GetById(id).Files.Where(f => f.LocationId == -1).AsQueryable();
                }
                else
                {
                    files = this.Data.Users.GetById(id).Files.AsQueryable();
                }

                searchLocation = "User " + locationName;
                inUserPage = true;
            }
            // When we are searching in the whole database
            else
            {
                files = this.Data.Files.All().AsQueryable();
            }

            fileName = fileName ?? string.Empty;

            var searchResult = CommonFunc.FileSearchQuery(
                    files, fileName, fileType, fromSizeInDim, toSizeInDim)
                    .Project()
                    .To<FileViewModel>();

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
                case "Owner desc":
                    searchResult = searchResult.OrderByDescending(f => f.OwnerName);
                    break;
                case "Owner":
                    searchResult = searchResult.OrderBy(f => f.OwnerName);
                    break;
                default:
                    searchResult = searchResult.OrderBy(f => f.Name);
                    break;
            }

            var founded = searchResult.ToPagedList(page ?? 1, recordsOnPage);
            var result = new SearchViewModel(founded, searchLocation, inUserPage, sortBy);

            return result;
        }

        public IQueryable<UserViewModel> UsersSearchResults(string userName)
        {
            userName = (userName ?? string.Empty).ToLower();

            var usersResult = this.Data
                .Users
                .All()
                .Where(u => u.UserName.ToLower().Contains(userName) || userName == string.Empty)
                .OrderBy(u => u.UserName)
                .Project()
                .To<UserViewModel>();

            return usersResult;
        }

        public UpperDirectoryViewModel GetUpperDirectory(int currentDirectoryId)
        {
            object id = null;
            string locationName = string.Empty;
            if (currentDirectoryId > 0)
            {
                var directory = this.Data.Files.GetById(currentDirectoryId);
                id = directory.LocationId;

                if (directory.LocationId != -1)
                {
                    locationName = this.Data.Files.GetById(id).Name;
                }
                else
                {
                    id = directory.OwnerId;
                    locationName = directory.Owner.UserName;
                }
            }

            return new UpperDirectoryViewModel(id, locationName);
        }

    }
}