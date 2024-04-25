using EduSchool.Models.Context;
using EduSchool.Models.DataModel;
using EduSchool.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System;
using System.Linq;
using System.Threading.Tasks;

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
                return View("NotFound");
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
                if (model.StudentID == null)
                {
                    TempData["ErrorMessage"] = "Kérjük, válasszon ki egy tanulót!";
                    return RedirectToAction("RecordAbsence", new { courseId = model.CourseID });
                }

                var absence = new Absence
                {
                    StudentID = model.StudentID,
                    CourseID = model.CourseID,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    AbsenceTypeID = model.AbsenceTypeID
                };

                _context.Absences.Add(absence);

                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Sikeres rögzítés!";
                    return RedirectToAction("Index", "Home");
                }
                catch (DbUpdateException ex)
                {
                    TempData["ErrorMessage"] = "Hiba történt a mulasztás rögzítésekor. Kérjük, próbálja újra.";
                    return RedirectToAction("RecordAbsence", new { courseId = model.CourseID });
                }
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> AbsenceList(int courseId)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = await _context.Courses.FindAsync(courseId);

            if (course == null || course.InstructorID != loggedInUserId)
            {
                return View("NotFound");
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
                return View("NotFound");
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
                TempData["ErrorMessage"] = "A tanfolyam nem található.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                _context.Update(absence);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Sikeres módosítás!";
                return RedirectToAction("AbsenceList", new { courseId = absence.CourseID });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hiba történt a művelet során";
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentAbsence(int courseId)
        {
            var loggedInStudentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isEnrolled = await _context.Enrollments
                .AnyAsync(e => e.CourseID == courseId && e.StudentID == loggedInStudentId);

            if (!isEnrolled)
            {
                return View("NotFound");
            }

            var absences = await _context.Absences
                .Include(a => a.Course)
                .Include(a => a.AbsenceType)
                .Where(a => a.CourseID == courseId && a.StudentID == loggedInStudentId)
                .ToListAsync();

            var course = await _context.Courses.FindAsync(courseId);

            var viewModel = new StudentAbsenceViewModel
            {
                CourseId = courseId,
                CourseName = course.Name,
                Absences = absences
            };

            return View(viewModel);
        }
    }
}
