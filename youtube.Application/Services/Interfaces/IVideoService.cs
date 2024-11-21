using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using youtube.Domain.Entities;

namespace youtube.Application.Services.Interfaces
{
    public interface IVideoService
    {
        Task<IEnumerable<Video>> GetAllVideosAsync();
        Task<Video> GetVideoByIdAsync(int id);
        Task AddVideoAsync(Video video);
        Task UpdateVideoAsync(Video video);
        Task DeleteVideoAsync(int id);
        Task<IEnumerable<Video>> GetVideosByUserIdAsync(string userId);
    }
}
