using Microsoft.AspNetCore.Mvc;

namespace EduSchool.Controllers
{
    public class AbscenceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
