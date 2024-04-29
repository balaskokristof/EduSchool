using EduSchool.Migrations.Edu;
using EduSchool.Models.Context;
using EduSchool.Models.DataModel;
using EduSchool.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EduSchool.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class CoursePostController : Controller
    {
        private readonly EduContext _context;

        public CoursePostController(EduContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CoursePostViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                    var loggedInUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var coursePost = new Models.DataModel.CoursePost
                {
                    CourseID = viewModel.CourseID,
                    Title = viewModel.Title,
                    Content = viewModel.Content,
                    AuthorID = loggedInUser,
                    PostDate = DateTime.Now
                };

                _context.CoursePost.Add(coursePost);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "A poszt sikeresen létrehozva";

                return Ok();

            }

            TempData["ErrorMessage"] = "A megadott adatok nem megfelelőek";
            return BadRequest(ModelState);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int postId)
        {
            var coursePost = await _context.CoursePost.FindAsync(postId);

            if (coursePost == null)
            {
                return NotFound();
            }

            _context.CoursePost.Remove(coursePost);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "CourseDetails", new { courseId = coursePost.CourseID });
        }

    }


}
