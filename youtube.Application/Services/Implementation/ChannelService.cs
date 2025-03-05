using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using youtube.Application.Common.Interfaces;
using youtube.Application.Services.Interfaces;
using youtube.Domain.Entities;

namespace youtube.Application.Services.Implementation
{
    public class ChannelService : IChannelService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChannelService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ChannelData> CreateChannelAsync(string userId, string name)
        {
            var channel = await _unitOfWork.Channel.CreateChannelAsync(userId, name);
            return new ChannelData
            {
                Id = channel.Id,
                Name = channel.Name,
                Handle = channel.Handle,
                UserId = channel.UserId
            };
        }



        public async Task<ChannelData> GetChannelDataAsync(string userId)
        {
            return await _unitOfWork.Channel.GetChannelDataAsync(userId);
        }

        public async Task<ChannelData> GetChannelDataByIdAsync(int Id)
        {
            return await _unitOfWork.Channel.GetChannelDataByIdAsync(Id);
        }

        public async Task<ChannelData> UpdateChannelDataAsync(int id, string bannerImageUrl, string profilePictureUrl, string name, string handle, string description)
        {
            return await _unitOfWork.Channel.UpdateChannelDataAsync(id, bannerImageUrl, profilePictureUrl, name, handle, description);
        }
    }
}
