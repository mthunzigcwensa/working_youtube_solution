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
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddAsync(Comment comment)
        {
            await _unitOfWork.Comment.AddAsync(comment);
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Comment>> GetVideoCommentsAsync(int videoId)
        {
            return await _unitOfWork.Comment.GetVideoCommentsAsync(videoId);
        }

        public async Task UpdateAsync(Comment comment)
        {
            throw new NotImplementedException();
        }
    }


}
