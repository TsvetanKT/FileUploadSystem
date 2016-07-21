using FileUploadSystem.Web.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploadSystem.Web.Infrastructure.Services.Contracts
{
    public interface ISearchServices
    {
        SearchViewModel GetSearchResults(
            object id,
            int? page,
            string locationName,
            string fileName,
            long? fromSize,
            long? toSize,
            string fileType,
            string dim,
            string sortBy,
            int recordsOnPage);

        IQueryable<UserViewModel> UsersSearchResults(string userName);

        UpperDirectoryViewModel GetUpperDirectory(int currentDirectoryId);
    }
}
