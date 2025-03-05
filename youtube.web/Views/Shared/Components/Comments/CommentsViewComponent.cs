using Microsoft.AspNetCore.Mvc;
using youtube.Application.Services.Interfaces;

namespace youtube.web.Views.Shared.Components.Comments
{
    public class CommentsViewComponent : ViewComponent
    {
        private readonly ICommentService _commentService;

        public CommentsViewComponent(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int videoId)
        {
            var comments = await _commentService.GetVideoCommentsAsync(videoId);
            return View(comments);
        }


    }
}
