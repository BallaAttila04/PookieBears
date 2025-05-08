using DotNetNuke.Web.Mvc.Framework.Controllers;
using System.Web.Mvc;

public class ItemController : DnnController
{
    [HttpGet]
    public ActionResult Index()
    {
        return View();  // az általad készített egyetlen Index.cshtml-t rendereli
    }
}
