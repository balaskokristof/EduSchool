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
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    AbsenceTypeID = model.AbsenceTypeID
                };

                _context.Absences.Add(absence);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            var students = await _context.Users
                .Where(u => u.Enrollments.Any(e => e.CourseID == model.CourseID))
                .ToListAsync();

            var absenceTypes = await _context.AbsenceTypes.ToListAsync();

            model.Students = students;
            model.AbsenceTypes = absenceTypes;

            return View(model);
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> AbsenceList(int courseId)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = await _context.Courses.FindAsync(courseId);

            if (course == null || course.InstructorID != loggedInUserId)
            {
                return NotFound();
            }

            var absenceTypes = await _context.AbsenceTypes.ToListAsync();
            var absences = await _context.Absences
                .Include(a => a.Student)
                .Include(a => a.AbsenceType)
                .Where(a => a.CourseID == courseId)
                .ToListAsync();

            var viewModel = new AbsenceListViewModel
            {
                CourseID = courseId,
                Absences = absences,
                AbsenceTypes = absenceTypes
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int absenceId)
        {
            var absence = await _context.Absences.FindAsync(absenceId);
            if (absence == null)
            {
                return NotFound();
            }

            return PartialView(absence);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(Absence absence)
        {
            var courseExists = await _context.Courses.AnyAsync(c => c.CourseID == absence.CourseID);
            if (!courseExists)
            {
                ModelState.AddModelError("", "A tanfolyam nem található.");
                return RedirectToAction("Index", "Home");
            }

            _context.Update(absence);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "A módosítás sikeresen megtörtént!";

            return RedirectToAction("AbsenceList", new { courseId = absence.CourseID });
        }

    }
}
