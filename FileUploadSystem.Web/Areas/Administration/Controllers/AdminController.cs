namespace FileUploadSystem.Web.Areas.Administration.Controllers
{
    using System.Web.Mvc;
    using FileUploadSystem.Common;
    using FileUploadSystem.Web.Controllers;
    using FileUploadSystem.Data.Contracts;

    [Authorize(Roles = GlobalConstants.AdminRole)]
    public abstract class AdminController : BaseController
    {
        public AdminController(IFileUploadSystemData data)
            : base(data)
        {

        }
    }
}