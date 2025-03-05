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
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public VideoController(IVideoService videoService, IConfiguration configuration)
        {
            _videoService = videoService;
            var connectionString = configuration["AzureBlobStorage:ConnectionString"];
            _containerName = configuration["AzureBlobStorage:ContainerName"];
            _blobServiceClient = new BlobServiceClient(connectionString);

            // Ensure the container exists (run this once during startup or here for simplicity)
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            containerClient.CreateIfNotExistsAsync().GetAwaiter().GetResult();
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

            // Get the Blob container client
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            // Handle video file upload
            if (videoFile != null && videoFile.Length > 0)
            {

                // Use a unique name to avoid overwrites (e.g., GUID + original filename)
                string videoBlobName = $"{Guid.NewGuid()}_{Path.GetFileName(videoFile.FileName)}";
                var videoBlobClient = containerClient.GetBlobClient(videoBlobName);

                // Upload the video file
                using (var stream = videoFile.OpenReadStream())
                {
                    await videoBlobClient.UploadAsync(stream, new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders { ContentType = videoFile.ContentType }
                    });
                }

                // Get the public URL
                videoUrl = videoBlobClient.Uri.ToString();
            }

            // Handle image file upload
            if (imageFile != null && imageFile.Length > 0)
            {
                string imageBlobName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
                var imageBlobClient = containerClient.GetBlobClient(imageBlobName);

                // Upload the image file
                using (var stream = imageFile.OpenReadStream())
                {
                    await imageBlobClient.UploadAsync(stream, new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders { ContentType = imageFile.ContentType }
                    });
                }

                // Get the public URL
                imageUrl = imageBlobClient.Uri.ToString();
            }

            // Create the video entity
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

            // Save to database via service
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
