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
       
        public ChannelController(IUnitOfWork unitOfWork, IChannelService channelService, IVideoService videoService, IConfiguration configuration)
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
