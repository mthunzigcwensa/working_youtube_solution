using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using youtube.Domain.Entities;
using youtube.Infrastrcture.Data;
using youtube.Infrastrcture.Repository;

namespace youtube.Tests
{
    [TestClass]
    public class VideoRepositoryTests
    {
        private ApplicationDbContext _context;
        private VideoRepository _videoRepository;

        [TestInitialize]
        public void Setup()
        {
           
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            _context = new ApplicationDbContext(options);
            _videoRepository = new VideoRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        

        [TestMethod]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoVideosExist()
        {
            // Act
            var result = await _videoRepository.GetAllAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task AddViewAsync_ThrowsException_WhenVideoDoesNotExist()
        {
            // Act
            await _videoRepository.AddViewAsync(999); 
        }
    }
}
