using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using youtube.Application.Common.Interfaces;
using youtube.Domain.Entities;
using youtube.Infrastrcture.Data;

namespace youtube.Infrastrcture.Repository
{
    public class VideoRepository : Repository<Video>, IVideoRepository
    {
        private readonly ApplicationDbContext _context;

        public VideoRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Video>> GetAllAsync()
        {
            return await _context.Videos
         .Include(v => v.ChannelData) // Includes related ChannelData
         .ToListAsync();
        }

        public async Task<Video> GetByIdAsync(int id)
        {
            return await _context.Videos
                .Include(v => v.ChannelData)
                .FirstOrDefaultAsync(v => v.Id == id);
        }



        public async Task AddAsync(Video video)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC AddVideo @Title = {0}, @Description = {1}, @VideoUrl = {2}, @ImageUrl = {3}, @UserId = {4}",
                video.Title, video.Description, video.VideoUrl, video.ImageUrl, video.UserId);
        }

        public async Task UpdateAsync(Video video)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC UpdateVideo @VideoId = {0}, @Title = {1}, @Description = {2}, @VideoUrl = {3}, @ImageUrl = {4}, @UserId = {5}",
                video.Id, video.Title, video.Description, video.VideoUrl, video.ImageUrl, video.UserId);
        }

        public async Task DeleteAsync(int id)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC DeleteVideo @VideoId = {0}", id);
        }

        public async Task<IEnumerable<Video>> GetVideosByUserIdAsync(string userId)
        {
            return await _context.Videos
          .Where(v => v.UserId == userId)
          .Include(v => v.ChannelData) 
          .ToListAsync();
        }
    }
}
