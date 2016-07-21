namespace FileUploadSystem.Web.Controllers
{
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    using FileUploadSystem.Models;
    using FileUploadSystem.Common;
    using FileUploadSystem.Web.Infrastructure.Services.Contracts;
    using FileUploadSystem.Data.Contracts;

    [Authorize]
    public class UploadController : BaseController
    {
        private IUploadServices uploadServices;

        public UploadController(IFileUploadSystemData data, IUploadServices uploadServices)
            : base(data)
        {
            this.uploadServices = uploadServices;
        }

        public ActionResult Index (
             int? id, 
            int? page, 
            string fileName, 
            long? fromSize, 
            long? toSize, 
            string fileType, 
            string dim, 
            string sortBy)
        {
            var fileSearchResult = this.uploadServices.GetSearchResults(
                id, page, fileName, fromSize, toSize, fileType, dim, sortBy, this.UserProfile, GlobalConstants.RecordsPerPage);

            return View(fileSearchResult);
        }
        
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase uploadFile, int? currentDirectory)
        {
            var res = this.uploadServices.UploadFile(uploadFile, currentDirectory, this.UserProfile, this.Data);

            TempData["ErrorFound"] = res.ErrorFound;
            TempData["Message"] = res.Message;
            return RedirectToAction("Index", new { id = res.RedirectId == -1 ? null : res.RedirectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rename(int fileId, string newName)
        {
            var res = this.uploadServices.Rename(fileId, newName, this.UserProfile, this.Data, false);

            TempData["ErrorFound"] = res.ErrorFound;
            TempData["Message"] = res.Message;
            return RedirectToAction("Index", new { id = res.RedirectId == -1 ? null : res.RedirectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int fileId)
        {
            var res = this.uploadServices.Delete(fileId, this.UserProfile, this.Data, false);

            TempData["ErrorFound"] = res.ErrorFound;
            TempData["Message"] = res.Message;
            return RedirectToAction("Index", new { id = res.RedirectId == -1 ? null : res.RedirectId });
        }

        public ActionResult Open(int id, string locationName)
        {
            var fileToOpen = this.UserProfile.Files.Where(f => f.Id == id).FirstOrDefault();

            if (fileToOpen == null)
            {
                return RedirectToAction("Error", "Home");
            }

            if (fileToOpen is BinaryFile)
            {
                return RedirectToAction("Download", "Files", new { id = id });
            }

            return RedirectToAction("Index", "Upload", new { id = id, locationName = locationName });
        }

        public ActionResult Upper(int current)
        {
            var nextId = this.uploadServices.GetUpperDirectory(current, this.UserProfile);

            return RedirectToAction("Index", "Upload", new { id = nextId == -1 ? null : nextId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDirectory(int parentDirectoryId, string directoryName)
        {
            var res = this.uploadServices.CreateDirectory(parentDirectoryId, directoryName, this.UserProfile, this.Data);

            TempData["ErrorFound"] = res.ErrorFound;
            TempData["Message"] = res.Message;
            return RedirectToAction("Index", "Upload", new { id = res.RedirectId == -1 ? null : res.RedirectId });
        }
    }
}