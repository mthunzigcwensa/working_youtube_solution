using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
       

        public VideoController(IVideoService videoService, IConfiguration configuration)
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
        [RequestFormLimits(MultipartBodyLengthLimit = 1073741824)]
        public async Task<IActionResult> Add(string title, string description, IFormFile videoFile, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(new Video { Title = title, Description = description });
            }

            string videoUrl = null;
            string imageUrl = null;

           

            

            var video = new Video
            {
                Title = title,
                Description = description,
                VideoUrl = videoUrl,
                ImageUrl = imageUrl,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                AddByDate = DateTime.UtcNow,
                viewCount = 0,
            };

           
            await _videoService.AddVideoAsync(video);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> watchvideo(int videoId)
        {
            await _videoService.AddView(videoId);
            var video = await _videoService.GetVideoByIdAsync(videoId);
            
            return View(video);

        }



    }
}
