using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using youtube.Domain.Entities;

namespace youtube.Application.Services.Interfaces
{
    public interface IChannelService
    {
        Task<ChannelData> CreateChannelAsync(string userId, string name);
        Task<ChannelData> GetChannelDataAsync(string userId);
        Task<ChannelData> GetChannelDataByIdAsync(int Id);
        Task<ChannelData> UpdateChannelDataAsync(int id, string bannerImageUrl, string profilePictureUrl, string name, string handle, string description);
    }
}
