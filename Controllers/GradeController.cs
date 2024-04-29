using EduSchool.Models;
using EduSchool.Models.Context;
using EduSchool.Models.DataModel;
using EduSchool.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EduSchool.Controllers
{
    [Authorize]
    public class GradeController : Controller
    {
        private readonly EduContext _context;

        public GradeController(EduContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }

            var loggedinUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedinUser = await _context.Users.FindAsync(loggedinUserId);
            if (loggedinUser == null)
            {
                return NotFound();
            }

            if (course.InstructorID != loggedinUser.UserID)
            {
                return RedirectToAction("Index", "Home");
            }

            var students = _context.Users
                .Where(u => u.Enrollments.Any(e => e.CourseID == courseId))
                .Select(u => new StudentGradeViewModel
                {
                    StudentID = u.UserID,
                    StudentName = u.LastName + " " + u.FirstName
                })
                .ToList();

            var viewModel = new GradeCreateViewModel
            {
                CourseID = courseId,
                Students = students
            };

            return View(viewModel);
        }
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public IActionResult Create(int courseId, List<StudentGradeViewModel> students, string gradeTitle)
        {
            var loggedinUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = _context.Courses.FirstOrDefault(c => c.CourseID == courseId);
            if(loggedinUserId != course.InstructorID)
            {
                return RedirectToAction("Index", "Home");
            }

            if (students == null || students.All(s => s.SelectedGradeValue == 0 && s.Weight == 0 && string.IsNullOrEmpty(s.Comment)))
            {
                TempData["ErrorMessage"] = "Nem adott meg egyetlen tanulót sem.";
                return RedirectToAction("Create", "Grade", new { courseId = courseId });
            }

            bool anyStudentFilled = false;

            foreach (var student in students)
            {
                if (student.SelectedGradeValue != 0)
                {
                    anyStudentFilled = true;

                    if (student.Weight == 0 || string.IsNullOrEmpty(student.Comment))
                    {
                        TempData["WarningMessage"] = "Nem minden tanuló értékelése teljes. Kérjük, töltse ki az összes mezőt.";
                        return RedirectToAction("Create", "Grade", new { courseId = courseId });
                    }

                    var grade = new Grade
                    {
                        CourseID = courseId,
                        StudentID = student.StudentID,
                        GradeValue = student.SelectedGradeValue,
                        Weight = student.Weight,
                        Comment = student.Comment,
                        GradeDate = DateTime.Now,
                        GradeTitle = gradeTitle
                    };

                    _context.Grades.Add(grade);
                }
            }

            if (!anyStudentFilled)
            {
                TempData["ErrorMessage"] = "Egyetlen tanuló se lett naplózva.";
                return RedirectToAction("Create", "Grade", new { courseId = courseId });
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Érdemjegyek sikeresen hozzáadva.";

            return RedirectToAction("Index", "CourseDetails", new { courseId = courseId });
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> AllCourseGrade(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);

            if (course == null)
            {
                return NotFound();
            }

            var loggedinUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (course.InstructorID != loggedinUserId)
            {
                return RedirectToAction("Index", "Home");
            }

            var students = await _context.Users
                .Where(u => u.Enrollments.Any(e => e.CourseID == courseId))
                .Select(u => new StudentGradeViewModel1
                {
                    StudentID = u.UserID,
                    StudentName = u.LastName + " " + u.FirstName,
                    Grades = _context.Grades
                        .Where(g => g.CourseID == courseId && g.StudentID == u.UserID)
                        .Select(g => new GradeViewModel1
                        {
                            GradeValue = g.GradeValue,
                            GradeTitle = g.GradeTitle,
                            Weight = g.Weight,
                            Comment = g.Comment,
                            GradeDate = g.GradeDate,
                            GradeID = g.GradeID

                        })
                        .ToList()
                })
                .ToListAsync();

            var model = new List<AllCourseGradesViewModel1>
    {
        new AllCourseGradesViewModel1
        {
            CourseID = course.CourseID,
            CourseName = course.Name,
            Students = students
        }
    };

            return View(model);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentGrade(int courseId)
        {
            var loggedinUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var student = await _context.Users.Include(u => u.Enrollments)
                                              .Include(u => u.Grades)
                                              .FirstOrDefaultAsync(u => u.UserID == loggedinUserId);

            if (student == null)
            {
                return NotFound();  
            }

            var enrollment = student.Enrollments.FirstOrDefault(e => e.CourseID == courseId);
            if (enrollment == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseID == courseId);
            if (course == null)
            {
                return NotFound();
            }

            var grades = student.Grades.Where(g => g.CourseID == courseId).ToList();
            if (grades.Count == 0)
            {
                ViewBag.Message = "Nincsenek jegyei ehhez a kurzushoz.";
            }

            var viewModel = new StudentGradeViewModel1
            {
                StudentID = student.UserID,
                StudentName = student.LastName + " " + student.FirstName,
                Grades = new List<GradeViewModel1>()
            };

            if (grades.Count > 0)
            {
                viewModel.Grades = grades.Select(g => new GradeViewModel1
                {
                    GradeValue = g.GradeValue,
                    GradeTitle = g.GradeTitle,
                    Weight = g.Weight,
                    Comment = g.Comment,
                    GradeDate = g.GradeDate,
                    GradeID = g.GradeID
                   
                }).ToList();
            }

            return View(viewModel);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<ActionResult> Edit(int id, int gradeValue, string gradeTitle, int weight, string comment, DateTime gradeDate)
        {
            var grade = await _context.Grades.FindAsync(id);

            if (grade == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            if (gradeValue < 1 || gradeValue > 5)
            {
                return Json(new { success = false, message = "A jegy értékének 1 és 5 között kell lennie!" });
            }

            grade.GradeValue = gradeValue;
            grade.GradeTitle = gradeTitle;
            grade.Weight = weight;
            grade.Comment = comment;
            grade.GradeDate = gradeDate;

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [Authorize(Roles="Teacher, Student")]
        [HttpPost]
        public async Task<ActionResult> GetGrade(int id)
        {
            var grade = await _context.Grades.FindAsync(id);

            if (grade == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            return View("Edit", grade);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<ActionResult> Delete(int gradeId)
        {
            var grade = await _context.Grades.FindAsync(gradeId);

            if (grade == null)
            {
                return NotFound();
            }

            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
