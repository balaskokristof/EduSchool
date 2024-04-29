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
                TempData["ErrorMessage"] = "A kurzus kezdő dátuma nem lehet nagyobb, mint a vég dátuma";
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "A megadott adatok nem megfelelőek";
            }
            try
            {
                if(ModelState.IsValid)
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
                    TempData["SuccessMessage"] = "A kurzus sikeresen létrehozva";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hiba történt a kurzus létrehozása közben";
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

        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<IActionResult> Enroll(CourseEnrollViewModel course)
        {
            var loggedinUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingCourse = await _context.Courses.FindAsync(course.CourseId);
            if (existingCourse == null)
            {
                TempData["ErrorMessage"] = "A kurzus nem található.";
                return View("CourseEnroll");
            }
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseID == course.CourseId && e.StudentID == loggedinUserId);
            if (existingEnrollment != null)
            {
                TempData["ErrorMessage"] = "Ön már feliratkozott erre a kurzusra.";
                return View("CourseEnroll");
            }
            try
            {

                var enrollment = new Enrollment
                {
                    CourseID = course.CourseId,
                    StudentID = loggedinUserId
                };

                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hiba történt a kurzusra való feliratkozás közben.";
            }
       
            return RedirectToAction("Index", "Home");
        }
  
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CourseEnroll()
        {
            return View();
        }
    }
}


