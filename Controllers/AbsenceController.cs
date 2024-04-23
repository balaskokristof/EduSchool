using EduSchool.Models.Context;
using EduSchool.Models.DataModel;
using EduSchool.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EduSchool.Controllers
{
    public class AbsenceController : Controller
    {
        private readonly EduContext _context;
        public AbsenceController(EduContext context)
        {
            _context = context;
        }


        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> RecordAbsence(int courseId)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = await _context.Courses.FindAsync(courseId);

            if (course == null || course.InstructorID != loggedInUserId)
            {
                return NotFound();
            }

            var students = await _context.Users
                .Where(u => u.Enrollments.Any(e => e.CourseID == courseId))
                .ToListAsync();

            var absenceTypes = await _context.AbsenceTypes.ToListAsync();

            var viewModel = new RecordAbsenceViewModel
            {
                CourseID = courseId,
                AbsenceTypes = absenceTypes,
                Students = students
            };

            return View(viewModel);
        }


        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> RecordAbsence(RecordAbsenceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var absence = new Absence
                {
                    StudentID = model.StudentID,
                    CourseID = model.CourseID,
                    Date = model.Date,
                    AbsenceTypeID = model.AbsenceTypeID
                };

                _context.Absences.Add(absence);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            model.AbsenceTypes = await _context.AbsenceTypes.ToListAsync();
            return View(model);
        }

    }
}
