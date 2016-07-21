namespace FileUploadSystem.Web.Controllers
{
    using System.IO;
    using System.Web.Mvc;

    using FileUploadSystem.Models;
    using FileUploadSystem.Data;
    using FileUploadSystem.Data.Contracts;

    public class FilesController : BaseController
    {
        public FilesController(IFileUploadSystemData data)
            : base(data)
        {

        }

        public ActionResult Download(int id)
        {
           try
            {
                var what = this.Data.Files;
                var fileToDownload = this.Data.Files.GetById(id) as BinaryFile;

                byte[] fileBytes = FileManipulator.DownloadFile(id, fileToDownload.Type);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileToDownload.Name);

            }
            catch (FileNotFoundException)
            {
                return RedirectToAction("Error", "Home");
            }
        }


        public ActionResult Open(int id, string locationName)
        {
            var fileToOpen = this.Data.Files.GetById(id);
            if (fileToOpen == null)
            {
                return RedirectToAction("Error", "Home");
            }

            if (fileToOpen is BinaryFile)
            {
                return RedirectToAction("Download", "Files", new { id = id });
            }

            return RedirectToAction("Files", "Search", new { id = id, locationName = locationName });
        }
    }
}