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
         .Include(v => v.ChannelData) 
         .ToListAsync();
        }

        public async Task<Video> GetByIdAsync(int id)
        {
            
            Video video = null;

            
            await using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetVideoById";
                command.CommandType = System.Data.CommandType.StoredProcedure;

               
                var param = command.CreateParameter();
                param.ParameterName = "@Id";
                param.Value = id;
                command.Parameters.Add(param);

               
                if (command.Connection.State != System.Data.ConnectionState.Open)
                {
                    await command.Connection.OpenAsync();
                }
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        
                        video = new Video
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            viewCount = reader.GetInt32(reader.GetOrdinal("viewCount")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("Description")),
                            VideoUrl = reader.GetString(reader.GetOrdinal("VideoUrl")),
                            ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("ImageUrl")),
                            UserId = reader.GetString(reader.GetOrdinal("UserId")),
                            ChannelDataId = reader.GetInt32(reader.GetOrdinal("ChannelDataId")),
                            AddByDate = reader.GetDateTime(reader.GetOrdinal("AddByDate")),
                            ChannelData = new ChannelData
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ChannelDataId")),
                                BannerImageUrl = reader.IsDBNull(reader.GetOrdinal("BannerImageUrl"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("BannerImageUrl")),
                                ProfilePictureUrl = reader.IsDBNull(reader.GetOrdinal("ProfilePictureUrl"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("ProfilePictureUrl")),
                                Name = reader.IsDBNull(reader.GetOrdinal("Name"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("Name")),
                                Handle = reader.IsDBNull(reader.GetOrdinal("Handle"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("Handle")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("Description")),
                                UserId = reader.IsDBNull(reader.GetOrdinal("UserId"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("UserId"))

                            }
                            };

                        
                    }
                }
            }

            return video;
        }




        public async Task AddAsync(Video video)
        {
            await _context.Database.ExecuteSqlRawAsync(
                 "EXEC AddVideo @Title = {0}, @Description = {1}, @VideoUrl = {2}, @ImageUrl = {3}, @UserId = {4}, @AddByDate = {5}",
                  video.Title, video.Description, video.VideoUrl, video.ImageUrl, video.UserId, video.AddByDate);
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
            var videos = new List<Video>();

            await using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetVideosByUserId";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                var param = command.CreateParameter();
                param.ParameterName = "@UserId";
                param.Value = userId;
                command.Parameters.Add(param);

               
                if (command.Connection.State != System.Data.ConnectionState.Open)
                {
                    await command.Connection.OpenAsync();
                }

                
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var video = new Video
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            viewCount = reader.GetInt32(reader.GetOrdinal("viewCount")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("Description")),
                            VideoUrl = reader.GetString(reader.GetOrdinal("VideoUrl")),
                            ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("ImageUrl")),
                            UserId = reader.GetString(reader.GetOrdinal("UserId")),
                            ChannelDataId = reader.GetInt32(reader.GetOrdinal("ChannelDataId")),
                            AddByDate = reader.GetDateTime(reader.GetOrdinal("AddByDate"))
                        };

                       
                        if (!reader.IsDBNull(reader.GetOrdinal("ChannelDataId")))
                        {
                            video.ChannelData = new ChannelData
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ChannelDataId")),
                                
                            };
                        }

                        videos.Add(video);
                    }
                }
            }

            return videos;
        }

        public async Task AddViewAsync(int videoId)
        {
            var video = await _context.Videos.FindAsync(videoId);

            if (video == null)
            {
                throw new KeyNotFoundException("Video not found.");
            }

            video.viewCount += 1;

            await _context.SaveChangesAsync();
        }

    }
}
