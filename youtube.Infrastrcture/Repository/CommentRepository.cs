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
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddAsync(Comment comment)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC AddComment @UserComment = {0}, @PostedBy = {1}, @UserId = {2}, @VideoId = {3}",
                comment.userComment, comment.postedBy, comment.UserId, comment.VideoId);
        }


        public async Task DeleteCommentAsync(int commentId)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC DeleteComment @CommentId = {0}",
                commentId);
        }


        public async Task<IEnumerable<Comment>> GetVideoCommentsAsync(int videoId)
        {
            var comments = new List<Comment>();

            await using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetVideoComments"; // Stored procedure name
                command.CommandType = System.Data.CommandType.StoredProcedure;

                // Add the parameter for the stored procedure
                var param = command.CreateParameter();
                param.ParameterName = "@VideoId";
                param.Value = videoId;
                command.Parameters.Add(param);

                // Open the connection if it's not already open
                if (command.Connection.State != System.Data.ConnectionState.Open)
                {
                    await command.Connection.OpenAsync();
                }

                // Execute the command and read the results
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var comment = new Comment
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("CommentId")),
                            userComment = reader.GetString(reader.GetOrdinal("userComment")),
                            postedBy = reader.GetDateTime(reader.GetOrdinal("postedBy")),
                            UserId = reader.GetString(reader.GetOrdinal("UserId")),
                            VideoId = reader.GetInt32(reader.GetOrdinal("VideoId"))
                        };

                       

                        if (!reader.IsDBNull(reader.GetOrdinal("UserId")))
                        {
                            comment.ApplicationUser = new ApplicationUser
                            {
                                Id = reader.GetString(reader.GetOrdinal("UserId")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                ProfilePic = reader.GetString(reader.GetOrdinal("ProfilePicUrl"))
                            };
                        }
                            comments.Add(comment);
                    }
                }
            }

            return comments;
        }


        public async Task UpdateAsync(Comment comment)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC UpdateComment @Id = {0}, @UserComment = {1}, @PostedBy = {2}, @UserId = {3}, @VideoId = {4}",
                comment.Id, comment.userComment, comment.postedBy, comment.UserId, comment.VideoId);
        }

    }
}
