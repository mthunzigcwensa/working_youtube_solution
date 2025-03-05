using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using youtube.Application.Services.Interfaces;
using youtube.Domain.Entities;

namespace youtube.web.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }
        [Authorize]
        public IActionResult addComment()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> addComment(Comment comment, int videoId)
        {
            ModelState.Remove("VideoData");
            ModelState.Remove("ApplicationUser");
            ModelState.Remove("UserId");
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            if (string.IsNullOrEmpty(userId))
            {

                return RedirectToAction("Login", "Account");
            }

            comment.UserId = userId;
            comment.VideoId = videoId;

            if (ModelState.IsValid)
            {
                // Assign the postedBy property with the current date and time
                comment.postedBy = DateTime.UtcNow;

                // Call the service to add the comment
                await _commentService.AddAsync(comment);

                // Redirect to a suitable page (e.g., back to the video details page)
                return RedirectToAction("watchvideo", "Video", new { videoId = comment.VideoId });
            }

            // If model validation fails, return to the form

            return RedirectToAction("watchvideo", "Video", new { videoId = videoId });
        }

    }
}
