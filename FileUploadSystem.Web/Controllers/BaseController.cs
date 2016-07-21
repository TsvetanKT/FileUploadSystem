namespace FileUploadSystem.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;

    using FileUploadSystem.Data.Contracts;
    using FileUploadSystem.Models;

    public class BaseController : Controller
    {
        protected IFileUploadSystemData Data { get; private set; }

        protected User UserProfile { get; private set; }

        public BaseController(IFileUploadSystemData data)
        {
            this.Data = data;
        }

        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            this.UserProfile = this.Data.Users.All().Where(u => u.UserName == requestContext.HttpContext.User.Identity.Name).FirstOrDefault();

            return base.BeginExecute(requestContext, callback, state);
        }
    }
}