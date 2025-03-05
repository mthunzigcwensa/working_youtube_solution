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
            // Define a new instance of the Video class to map the data
            Video video = null;

            // Use raw SQL to call the stored procedure and read the result
            await using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetVideoById"; // Stored procedure name
                command.CommandType = System.Data.CommandType.StoredProcedure;

                // Add the parameter for the stored procedure
                var param = command.CreateParameter();
                param.ParameterName = "@Id";
                param.Value = id;
                command.Parameters.Add(param);

                // Open the connection if it's not already open
                if (command.Connection.State != System.Data.ConnectionState.Open)
                {
                    await command.Connection.OpenAsync();
                }

                // Execute the command and read the results
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        // Manually map the result to the Video class
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
                command.CommandText = "GetVideosByUserId"; // Stored procedure name
                command.CommandType = System.Data.CommandType.StoredProcedure;

                // Add the parameter for the stored procedure
                var param = command.CreateParameter();
                param.ParameterName = "@UserId";
                param.Value = userId;
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

                        // Map the related ChannelData if it exists
                        if (!reader.IsDBNull(reader.GetOrdinal("ChannelDataId")))
                        {
                            video.ChannelData = new ChannelData
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ChannelDataId")),
                                // Map other ChannelData properties here if needed
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
