
namespace FileUploadSystem.Web.Controllers
{
    using System.Web.Mvc;

    using FileUploadSystem.Data;

    public class HomeController : BaseController
    {
        public HomeController(FileUploadSystemData data)
            : base(data)
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}