using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using youtube.Domain.Entities;

namespace youtube.Application.Services.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<Comment>> GetVideoCommentsAsync(int videoId);


        Task AddAsync(Comment comment);

        Task UpdateAsync(Comment comment);

        Task DeleteCommentAsync(int commentId);
    }
}
