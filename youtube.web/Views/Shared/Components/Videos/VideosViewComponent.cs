using Microsoft.AspNetCore.Mvc;
using youtube.Application.Services.Interfaces;

namespace youtube.web.Views.Shared.Components.Videos
{
    public class VideosViewComponent : ViewComponent
    {
        private readonly IVideoService _videoService;

        public VideosViewComponent(IVideoService videoService)
        {
            _videoService = videoService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var videos = await _videoService.GetAllVideosAsync();
            return View(videos); 
        }
    }

}
