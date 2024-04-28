using EduSchool.Models.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EduSchool.Controllers
{
    public class CourseDetailsController : Controller
    {
        private readonly EduContext _context;
        public CourseDetailsController(EduContext context)
        {
            _context = context;
        }
        [Authorize]
        public async Task<IActionResult> IndexAsync(int courseID)
        {

            var course = await _context.Courses
                            .Include(c => c.CoursePosts)
                            .FirstOrDefaultAsync(c => c.CourseID == courseID);
            if (course == null)
            {
                return View("NotFound");
            }
            var loggedinUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedinuser = await _context.Users.FindAsync(loggedinUser);
            var isEnrolled = _context.Enrollments.Any(e => e.CourseID == courseID && e.StudentID == loggedinuser.UserID);
            if (User.IsInRole("Teacher"))
            {
                if (course.InstructorID != loggedinuser.UserID)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View(course);
                }
            }
            else if (User.IsInRole("Student"))
            {
                if (!isEnrolled)
                {
                    return RedirectToAction("Index", "Home"); 
                }
                else
                {
                    return View(course);
                }
            }


            return RedirectToAction("Index", "Home");
        }

        public IActionResult CourseStudents(int courseId)
        {
            var course = _context.Courses
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
                .FirstOrDefault(c => c.CourseID == courseId);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEnrollment(int courseId, string studentId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseID == courseId && e.StudentID == studentId);

            if (enrollment == null)
            {
                return NotFound();
            }

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "CourseDetails", new { courseId = courseId });
        }
    }
}