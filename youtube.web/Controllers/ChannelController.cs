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

        public ChannelController(IUnitOfWork unitOfWork, IChannelService channelService, IVideoService videoService)
        {
           
            _channelService = channelService;
            _videoService = videoService;
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

            if (BannerImage != null && BannerImage.Length > 0)
            {
                var bannerImagePath = Path.Combine("wwwroot/uploads", BannerImage.FileName);
                using (var stream = new FileStream(bannerImagePath, FileMode.Create))
                {
                    await BannerImage.CopyToAsync(stream);
                }
                bannerImageUrl = $"/uploads/{BannerImage.FileName}";
            }

            if (ProfilePicture != null && ProfilePicture.Length > 0)
            {
                var profilePicturePath = Path.Combine("wwwroot/uploads", ProfilePicture.FileName);
                using (var stream = new FileStream(profilePicturePath, FileMode.Create))
                {
                    await ProfilePicture.CopyToAsync(stream);
                }
                profilePictureUrl = $"/uploads/{ProfilePicture.FileName}";
            }

            var updatedChannel = await _channelService.UpdateChannelDataAsync(id, bannerImageUrl ?? "", profilePictureUrl ?? "", name, handle, description);

            if (updatedChannel == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }


    }
}
