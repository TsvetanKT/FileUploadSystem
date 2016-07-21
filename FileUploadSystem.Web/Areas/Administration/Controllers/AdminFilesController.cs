namespace FileUploadSystem.Web.Areas.Administration.Controllers
{
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using FileUploadSystem.Data;
    using FileUploadSystem.Models;

    using AutoMapper.QueryableExtensions;

    using PagedList;

    using FileUploadSystem.Web.Infrastructure.Services.Contracts;
    using FileUploadSystem.Web.Infrastructure.Services.Upload;
    using FileUploadSystem.Web.ViewModels.Search;
    using FileUploadSystem.Data.Contracts;

    public class AdminFilesController : AdminController
    {
        private ISearchServices searchServices;
        private IUploadServices uploadServices;

        public AdminFilesController(IFileUploadSystemData data, ISearchServices searchServices, IUploadServices uploadServices)
            : base(data)
        {
            this.searchServices = searchServices;
            this.uploadServices = uploadServices;
        }

        // GET: Administration/AdminFiles
        public ActionResult Index(
            object id, int? page, string locationName, string fileName, long? fromSize, long? toSize, string fileType, string dim, string sortBy)
        {
            var fileSearchResult = this.searchServices.GetSearchResults(
                id, page, locationName, fileName, fromSize, toSize, fileType, dim, sortBy, 50);

            return View(fileSearchResult);
        }

        public ActionResult CheckDbFileValidity(int? page)
        {
            var unvalidFiles = this.Data.
                Files
                .All()
                .ToList()
                .Where(f => f is BinaryFile)
                .Select(f => (BinaryFile)f)
                .Where(f => FileManipulator.CheckFileValidity(f.Id, f.Type, f.Size) == false)
                .AsQueryable()
                .Project()
                .To<FileViewModel>()
                .OrderBy(f => f.Name)
                .ToPagedList(page ?? 1, 50);

            var res = new SearchViewModel(unvalidFiles, null, false, null);

            return View(res);
        }

        public ActionResult Upper(int currentDirectory)
        {
            var upperDirectory = this.searchServices.GetUpperDirectory(currentDirectory);

            return RedirectToAction("Index", "AdminFiles", new
            {
                id = upperDirectory.Id,
                locationName = upperDirectory.LocationName
            });
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
                return RedirectToAction("Download", "Files", new { area = string.Empty, id = id });
            }

            return RedirectToAction("Index", "AdminFiles", new { id = id, locationName = locationName });
        }

        // GET: Administration/AdminFiles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var fileEntity = this.Data.Files.GetById(id);
            if (fileEntity == null)
            {
                return HttpNotFound();
            }

            return View(fileEntity);
        }

        // GET: Administration/AdminFiles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var fileEntity = this.Data.Files.GetById(id);
            if (fileEntity == null)
            {
                return HttpNotFound();
            }

            return View(fileEntity);
        }

        // POST: Administration/AdminFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,LocationId,OwnerId")] FileEntity fileEntity)
        {
            if (fileEntity != null && ModelState.IsValid)
            {
                var oldFile = this.Data.Files.GetById(fileEntity.Id);
                TempData["ErrorFound"] = true;

                if (fileEntity.Name.Trim().Length == 0)
                {
                    TempData["Message"] = "Bad file name!";
                    return RedirectToAction("Edit", new { id = fileEntity.Id });
                }

                var oldOwnerUser = oldFile.Owner;
                var oldOwner = oldFile.Owner.UserName;
                var oldLocation = oldFile.LocationId == -1 ? 
                    "User directory"  : 
                    this.Data.Files.GetById(oldFile.LocationId).Name;
                var oldName = oldFile.Name;

                var newOwner = this.Data.Users.All().Where(u => u.Id == fileEntity.OwnerId).FirstOrDefault();

                // Validate new user is valid user
                if (newOwner == null)
                {
                    TempData["Message"] = "User with that Id: " + fileEntity.OwnerId + " does not exist!";
                    return RedirectToAction("Edit", new { id = fileEntity.Id });
                }

                if (fileEntity.LocationId != -1)
                {
                    var newFileLocation = newOwner.Files.Where(f => f.Id == fileEntity.LocationId).FirstOrDefault();
                    DirectoryEntity newDir = null;

                    if (newFileLocation != null)
                    {
                        newDir = newFileLocation as DirectoryEntity;
                    }

                    if (newFileLocation == null || newDir == null)
                    {
                        TempData["Message"] = "User " + newOwner.UserName + " does not have directory with id: " + fileEntity.OwnerId;
                        return RedirectToAction("Edit", new { id = fileEntity.Id });
                    }
                }

                var binFile = oldFile as BinaryFile;

                if (binFile == null)
                {
                    var nestedFiles = UploadServices.DFSNestedElementFinder(oldOwnerUser, fileEntity.Id);

                    // Validate that new dir is not inner to our current dir
                    foreach (var item in nestedFiles)
                    {
                        if (item.Id == fileEntity.LocationId)
                        {
                            TempData["Message"] = "Can not desplace the current directory to it's own inner directory";
                            return RedirectToAction("Edit", new { id = fileEntity.Id });
                        }
                    }

                    foreach (var item in nestedFiles)
                    {
                        var currentFile = oldOwnerUser.Files.Where(f => f.Id == item.Id).FirstOrDefault();
                        this.Data.Context.Entry(currentFile).State = EntityState.Modified;
                        currentFile.OwnerId = fileEntity.OwnerId;
                        currentFile.Owner = newOwner;
                        newOwner.Files.Add(currentFile);
                    }
                }
                else
                {
                    oldFile.Name = fileEntity.Name;
                    oldFile.LocationId = fileEntity.LocationId;
                    oldFile.OwnerId = fileEntity.OwnerId;
                    oldFile.Owner = newOwner;
                }

                this.Data.Context.Entry(oldFile).State = EntityState.Modified;
                this.Data.Context.SaveChanges();

                TempData["ErrorFound"] = false;
                TempData["Message"] = "File " + fileEntity.Name + " (" + oldFile.Name + ") edited.";
                return RedirectToAction("Index", new { id = fileEntity.LocationId == -1 ? fileEntity.OwnerId : fileEntity.LocationId.ToString() });
            }

            TempData["Message"] = "Unknown error!";
            return RedirectToAction("Index");
        }

        // GET: Administration/AdminFiles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var fileEntity = this.Data.Files.GetById(id);
            if (fileEntity == null)
            {
                return HttpNotFound();
            }

            return View(fileEntity);
        }

        // POST: Administration/AdminFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string action, string controller)
        {
            var res = this.uploadServices.Delete(id, this.UserProfile, this.Data, true);
            action = action ?? "Index";
            controller = controller ?? "AdminFilesController";

            TempData["ErrorFound"] = res.ErrorFound;
            TempData["Message"] = res.Message;
            return RedirectToAction(action, controller, new { id = res.RedirectId == -1 ? null : res.RedirectId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Data.Context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
