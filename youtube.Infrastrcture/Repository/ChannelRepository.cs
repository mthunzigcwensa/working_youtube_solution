using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using youtube.Application.Common.Interfaces;
using youtube.Domain.Entities;
using youtube.Infrastrcture.Data;

namespace youtube.Infrastrcture.Repository
{
    public class ChannelRepository : Repository<ChannelData>, IChannelRepository
    {
        private readonly ApplicationDbContext _context;

        public ChannelRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ChannelData> CreateChannelAsync(string userId, string name)
        {
            var channel = new ChannelData
            {
                UserId = userId,
                Name = name,
                Handle = name,
                BannerImageUrl = "",
                ProfilePictureUrl = "",
                Description = ""
            };

            _context.ChannelData.Add(channel);
            await _context.SaveChangesAsync();

            return channel;
        }



        public async Task<ChannelData> GetChannelDataAsync(string userId)
        {
            var userIdParam = new SqlParameter("@UserId", userId);

            var channelData = _context.ChannelData
                .FromSqlRaw("EXEC GetChannelDataByUserId @UserId", userIdParam)
                .AsEnumerable()
                .FirstOrDefault();

            return channelData;
        }

        public async Task<ChannelData> GetChannelDataByIdAsync(int Id)
        {
            var IdParam = new SqlParameter("@Id", Id);

            var channelData = _context.ChannelData
                .FromSqlRaw("EXEC GetChannelDataById @Id", IdParam)
                .AsEnumerable()
                .FirstOrDefault();

            return channelData;
        }

        public async Task<ChannelData> UpdateChannelDataAsync(int id, string bannerImageUrl, string profilePictureUrl, string name, string handle, string description)
        {

            var result = await _context.Database.ExecuteSqlRawAsync(
                "EXEC UpdateChannelData @Id = {0}, @BannerImageUrl = {1}, @ProfilePictureUrl = {2}, @Name = {3}, @Handle = {4}, @Description = {5}",
                id, bannerImageUrl, profilePictureUrl, name, handle, description);


            if (result > 0)
            {
                return await _context.ChannelData.FindAsync(id);
            }

            return null;
        }





    }
}
