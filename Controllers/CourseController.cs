using EduSchool.Migrations.Edu;
using EduSchool.Models.Context;
using EduSchool.Models.DataModel;
using EduSchool.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EduSchool.Controllers
{
    
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


        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> Create(CourseViewModel model)
        {
            var loggedinUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedinuser = await _context.Users.FindAsync(loggedinUser);
            if (model.StartDate > model.EndDate)
            {
                ModelState.AddModelError(string.Empty, "A befejezési dátum nem lehet korábbi a kezdési dátumnál.");
            }

            if(!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "A megadott adatok nem megfelelőek.");
            }
            else if(ModelState.IsValid)
            {
                var course = new Models.DataModel.Course
                {
                    Name = model.Name,
                    Description = model.Description,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Category = model.Category,
                    InstructorID = loggedinuser.UserID,
                    InstructorName = loggedinuser.LastName + " " + loggedinuser.FirstName
                    
                };

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Index()
        {
            var loggedinUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedinUser = await _context.Users.FindAsync(loggedinUserId);
            ViewBag.InstructorName = loggedinUser.LastName + " " + loggedinUser.FirstName;
            return View();

            /*
            var students = await _context.Users
                .Where(u => u.UserType == UserType.Student)
                .Select(s => s.UserID)
                .ToListAsync();

            var viewModel = new CourseViewModel
            {
                Students = students
            };

            return View(viewModel);
        }
            */
        }

        /*
        [HttpGet]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _context.Users
                .Where(u => u.UserType == UserType.Student)
                .Select(s => new { UserID = s.UserID, FullName = s.FirstName + " " + s.LastName })
                .ToListAsync();

            return Json(students);
        }
        */
        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<IActionResult> Enroll(CourseEnrollViewModel course)
        {
            var loggedinUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingCourse = await _context.Courses.FindAsync(course.CourseId);
            if (existingCourse == null)
            {
                ModelState.AddModelError(string.Empty, "A megadott kurzus nem létezik.");
                return View("CourseEnroll");
            }
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseID == course.CourseId && e.StudentID == loggedinUserId);
            if (existingEnrollment != null)
            {
                ModelState.AddModelError(string.Empty, "Ön már fel van véve erre a kurzusra.");
                return View("CourseEnroll");
            }

            var enrollment = new Enrollment
            {
                CourseID = course.CourseId,
                StudentID = loggedinUserId
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

    return RedirectToAction("Index", "Home");

        }

        [Authorize(Roles = "Student")]
        public IActionResult CourseEnroll()
        {
            return View();
        }
    }


}
