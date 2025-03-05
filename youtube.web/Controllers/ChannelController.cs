using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using youtube.Application.Common.Interfaces;
using youtube.Application.Services.Implementation;
using youtube.Application.Services.Interfaces;
using youtube.Domain.Entities;
using youtube.Domain.ViewModels;

namespace youtube.web.Controllers
{
    public class ChannelController : Controller
    {

        private readonly IChannelService _channelService;
        private readonly IVideoService _videoService;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _profilePictureContainerName;
        private readonly string _bannerContainerName;
        public ChannelController(IUnitOfWork unitOfWork, IChannelService channelService, IVideoService videoService, IConfiguration configuration)
        {

            _channelService = channelService;
            _videoService = videoService;
            var connectionString = configuration["AzureBlobStorage:ConnectionString"];
            _profilePictureContainerName = configuration["AzureBlobStorage:ProfilePictureContainerName"];
            _bannerContainerName = configuration["AzureBlobStorage:BannerContainerName"];
            _blobServiceClient = new BlobServiceClient(connectionString);

            // Ensure containers exist (run once during startup or here for simplicity)
            _blobServiceClient.GetBlobContainerClient(_profilePictureContainerName)
                .CreateIfNotExistsAsync().GetAwaiter().GetResult();
            _blobServiceClient.GetBlobContainerClient(_bannerContainerName)
                .CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {

                return RedirectToAction("Login", "Account");
            }


            var channelData = await _channelService.GetChannelDataAsync(userId);

            if (channelData == null)
            {

                return RedirectToAction("Create", "Channel");
            }

            var userVideos = await _videoService.GetVideosByUserIdAsync(userId);

            var viewModel = new ChannelVM
            {
                ChannelData = channelData,
                UserVideos = userVideos
            };

            return View(viewModel);
        }

        public async Task<IActionResult> channelData(string id)
        {


            var channelData = await _channelService.GetChannelDataAsync(id);

            if (channelData == null)
            {
                return RedirectToAction("Create", "Channel");
            }

            var userVideos = await _videoService.GetVideosByUserIdAsync(id);

            var viewModel = new ChannelVM
            {
                ChannelData = channelData,
                UserVideos = userVideos
            };

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Update(int channelId)
        {
            var channelData = await _channelService.GetChannelDataByIdAsync(channelId);

            if (channelData == null)
            {
                return NotFound();
            }

            return View(channelData);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, string name, string handle, string description, IFormFile BannerImage, IFormFile ProfilePicture)
        {
            if (!ModelState.IsValid)
            {
                return View(new ChannelData { Id = id, Name = name, Handle = handle, Description = description });
            }

            string bannerImageUrl = null;
            string profilePictureUrl = null;

            // Handle banner image upload
            if (BannerImage != null && BannerImage.Length > 0)
            {
                var bannerContainerClient = _blobServiceClient.GetBlobContainerClient(_bannerContainerName);
                string bannerBlobName = $"{Guid.NewGuid()}_{Path.GetFileName(BannerImage.FileName)}";
                var bannerBlobClient = bannerContainerClient.GetBlobClient(bannerBlobName);

                // Upload the banner image
                using (var stream = BannerImage.OpenReadStream())
                {
                    await bannerBlobClient.UploadAsync(stream, new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders { ContentType = BannerImage.ContentType }
                    });
                }

                bannerImageUrl = bannerBlobClient.Uri.ToString();
            }

            // Handle profile picture upload
            if (ProfilePicture != null && ProfilePicture.Length > 0)
            {
                var profileContainerClient = _blobServiceClient.GetBlobContainerClient(_profilePictureContainerName);
                string profileBlobName = $"{Guid.NewGuid()}_{Path.GetFileName(ProfilePicture.FileName)}";
                var profileBlobClient = profileContainerClient.GetBlobClient(profileBlobName);

                // Upload the profile picture
                using (var stream = ProfilePicture.OpenReadStream())
                {
                    await profileBlobClient.UploadAsync(stream, new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders { ContentType = ProfilePicture.ContentType }
                    });
                }

                profilePictureUrl = profileBlobClient.Uri.ToString();
            }

            // Update channel data with new URLs (or keep existing URLs if no new file uploaded)
            var updatedChannel = await _channelService.UpdateChannelDataAsync(
                id,
                bannerImageUrl ?? "",
                profilePictureUrl ?? "",
                name,
                handle,
                description
            );

            if (updatedChannel == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }

    }
}
