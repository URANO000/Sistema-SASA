using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SASA.Controllers
{
    public class TiqueteController : Controller
    {
        //GET: TiqueteController
        public ActionResult Index()
        {
            return View();
        }
    }
}
