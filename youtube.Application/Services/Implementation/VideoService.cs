using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using youtube.Application.Common.Interfaces;
using youtube.Application.Services.Interfaces;
using youtube.Domain.Entities;

namespace youtube.Application.Services.Implementation
{
    public class VideoService : IVideoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VideoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Video>> GetAllVideosAsync()
        {
            return await _unitOfWork.Video.GetAllAsync();
        }

        public async Task<Video> GetVideoByIdAsync(int id)
        {
            return await _unitOfWork.Video.GetByIdAsync(id);
        }

        public async Task AddVideoAsync(Video video)
        {
            await _unitOfWork.Video.AddAsync(video);
        }

        public async Task UpdateVideoAsync(Video video)
        {
            await _unitOfWork.Video.UpdateAsync(video);
        }

        public async Task DeleteVideoAsync(int id)
        {
            await _unitOfWork.Video.DeleteAsync(id);
        }

        public async Task<IEnumerable<Video>> GetVideosByUserIdAsync(string userId)
        {
            return await _unitOfWork.Video.GetVideosByUserIdAsync(userId);
        }

        public async Task AddView(int videoId)
        {
            await _unitOfWork.Video.AddViewAsync(videoId);
        }
    }
}
