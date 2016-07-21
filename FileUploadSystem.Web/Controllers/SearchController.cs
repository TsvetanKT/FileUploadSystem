namespace FileUploadSystem.Web.Controllers
{
    using System.Web.Mvc;

    using PagedList;

    using FileUploadSystem.Common;
    using FileUploadSystem.Web.Infrastructure.Services.Contracts;
    using FileUploadSystem.Data.Contracts;

    public class SearchController : BaseController
    {
        private ISearchServices searchServices;

        public SearchController(IFileUploadSystemData data, ISearchServices searchServices)
            : base(data)
        {
            this.searchServices = searchServices;
        }

        public ActionResult Files(
            object id, int? page, string locationName, string fileName, long? fromSize, long? toSize, string fileType, string dim, string sortBy)
        {
            var fileSearchResult = this.searchServices.GetSearchResults(
                id, page, locationName, fileName, fromSize, toSize, fileType, dim, sortBy, GlobalConstants.RecordsPerPage);

            return View(fileSearchResult);
        }

        public ActionResult Users( int? page, string userName)
        {
            var usersResult = this.searchServices.UsersSearchResults(userName);

            return View(usersResult.ToPagedList(page ?? 1, GlobalConstants.RecordsPerPage));
        }

        public ActionResult Upper(int currentDirectory)
        {
            var upperDirectory = this.searchServices.GetUpperDirectory(currentDirectory);

            return RedirectToAction("Files", "Search", new
                {
                    id = upperDirectory.Id,
                    locationName = upperDirectory.LocationName 
                });
        }
    }
}