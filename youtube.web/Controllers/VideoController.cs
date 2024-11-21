using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using youtube.Application.Services.Interfaces;
using youtube.Domain.Entities;

namespace youtube.web.Controllers
{
   
    public class VideoController : Controller
    {

        private readonly IVideoService _videoService;

        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        public async Task<IActionResult> Index()
        {
            var videos = await _videoService.GetAllVideosAsync();
            return View(videos);
        }
        public async Task<IActionResult> _uvideos()
        {
            var videos = await _videoService.GetAllVideosAsync();
            return View(videos);
        }
        
        public async Task<IActionResult> getUsersVideos()
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var videos = await _videoService.GetVideosByUserIdAsync(UserId);
            return PartialView("_uservideos", videos);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(string title, string description, IFormFile videoFile, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(new Video { Title = title, Description = description });
            }

            string videoUrl = null;
            string imageUrl = null;

            if (videoFile != null && videoFile.Length > 0)
            {
                var videoPath = Path.Combine("wwwroot/uploads", videoFile.FileName);
                using (var stream = new FileStream(videoPath, FileMode.Create))
                {
                    await videoFile.CopyToAsync(stream);
                }
                videoUrl = $"/uploads/{videoFile.FileName}";
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var imagePath = Path.Combine("wwwroot/uploads", imageFile.FileName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                imageUrl = $"/uploads/{imageFile.FileName}";
            }

            var video = new Video
            {
                Title = title,
                Description = description,
                VideoUrl = videoUrl,
                ImageUrl = imageUrl,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            };

            await _videoService.AddVideoAsync(video);
            return RedirectToAction("Index"); 
        }

        [HttpGet]
        public async Task<IActionResult> watchvideo(int videoId)
        {

            var video = await _videoService.GetVideoByIdAsync(videoId);
            return View(video);

        }
     


    }
}
