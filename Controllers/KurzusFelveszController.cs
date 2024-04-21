using Microsoft.AspNetCore.Mvc;

namespace EduSchool.Controllers
{
    public class KurzusFelveszController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
