using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using youtube.Infrastrcture.Data;
using youtube.Infrastrcture.Repository;

namespace youtube.Tests
{
    [TestClass]
    public class ChannelRepositoryTests
    {
        private ApplicationDbContext _context;
        private ChannelRepository _channelRepository;

        [TestInitialize]
        public void Setup()
        {
            // Set up the in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database per test
                .Options;

            _context = new ApplicationDbContext(options);
            _channelRepository = new ChannelRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task CreateChannelAsync_CreatesChannel_ReturnsChannelData()
        {
            // Arrange
            string userId = "user123";
            string name = "Test Channel";

            // Act
            var result = await _channelRepository.CreateChannelAsync(userId, name);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.UserId);
            Assert.AreEqual(name, result.Name);
            Assert.AreEqual(name, result.Handle);
            Assert.AreEqual("", result.BannerImageUrl);
            Assert.AreEqual("", result.ProfilePictureUrl);
            Assert.AreEqual("", result.Description);

            // Verify the channel was added to the database
            var channelInDb = await _context.ChannelData.FindAsync(result.Id);
            Assert.IsNotNull(channelInDb);
            Assert.AreEqual(name, channelInDb.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public async Task CreateChannelAsync_ThrowsException_WhenUserIdIsNull()
        {
            // Arrange
            string userId = null; // Violates [Required] constraint
            string name = "Test Channel";

            // Act
            await _channelRepository.CreateChannelAsync(userId, name);

            // Assert: Expecting DbUpdateException due to [Required] constraint on UserId
        }

        
    }
}
