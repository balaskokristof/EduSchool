using EduSchool.Migrations.Edu;
using EduSchool.Models;
using EduSchool.Models.Context;
using EduSchool.Models.DataModel;
using EduSchool.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public IActionResult Create(int courseId, List<StudentGradeViewModel> students)
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
                                GradeDate = DateTime.Now
                            };

                            _context.Grades.Add(grade);
                        }
                    }

                    _context.SaveChanges();

                    return RedirectToAction("Index", "CourseDetails", new { courseID = courseId });
                }
            return RedirectToAction("Create", "Grade", new { courseId = courseId });
        }



    }
}
