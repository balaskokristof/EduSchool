using EduSchool.Migrations.Edu;
using EduSchool.Models.Context;
using EduSchool.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace EduSchool.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class CourseController : Controller
    {
        private readonly EduContext _context;

        public CourseController(EduContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CreateAsync()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseViewModel model)
        {
            var loggedinUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedinuser = await _context.Users.FindAsync(loggedinUser);
            if (model.StartDate > model.EndDate)
            {
                ModelState.AddModelError("EndDate", "A befejezési dátum nem lehet korábbi a kezdési dátumnál.");
            }
            else
            {
                var course = new Models.DataModel.Course
                {
                    Name = model.Name,
                    Description = model.Description,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Category = model.Category,
                    InstructorID = loggedinuser.UserID
                };

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(model);
        }
        public async Task<IActionResult> Index()
        {
            var loggedinUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedinuser = await _context.Users.FindAsync(loggedinUser);
            ViewBag.InstructorName = loggedinuser.LastName + " " + loggedinuser.FirstName;
            Console.WriteLine(loggedinuser.LastName + " " + loggedinuser.FirstName);
            return View();
        }

    }
}
