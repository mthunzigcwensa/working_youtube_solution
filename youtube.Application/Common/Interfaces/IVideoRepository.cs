using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using youtube.Domain.Entities;

namespace youtube.Application.Common.Interfaces
{
    public interface IVideoRepository : IRepository<Video>
    {
        Task<IEnumerable<Video>> GetAllAsync();
        Task<Video> GetByIdAsync(int id);
        Task AddAsync(Video video);
        Task UpdateAsync(Video video);
        Task DeleteAsync(int id);
        Task<IEnumerable<Video>> GetVideosByUserIdAsync(string userId);
        Task AddViewAsync(int videoId);
    }
}
