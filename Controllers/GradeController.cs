using EduSchool.Models;
using EduSchool.Models.Context;
using EduSchool.Models.DataModel;
using EduSchool.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EduSchool.Controllers
{
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
            if (students != null)
            {
                foreach (var student in students)
                {
                    if (student.SelectedGradeValue != 0 && !string.IsNullOrEmpty(student.Comment))
                    {
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

                _context.SaveChanges();

                return RedirectToAction("Index", "CourseDetails", new { courseID = courseId });
            }
            return RedirectToAction("Create", "Grade", new { courseId = courseId });
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
                            GradeDate = g.GradeDate

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
                    GradeDate = g.GradeDate
                }).ToList();
            }

            return View(viewModel);
        }



    }
}
