using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using youtube.Application.Common.Interfaces;
using youtube.Application.Services.Implementation;
using youtube.Domain.Entities;

namespace youtube.Tests
{
    [TestClass]
    public class VideoServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private VideoService _videoService;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _videoService = new VideoService(_unitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task AddVideoAsync_AddsVideo_Successfully()
        {
            // Arrange
            var video = new Video
            {
                Title = "Test Video",
                VideoUrl = "https://example.com/video.mp4",
                UserId = "user123",
                ChannelDataId = 1,
                viewCount = 0,
                AddByDate = DateTime.Now
            };

            var videoRepoMock = new Mock<IVideoRepository>();
            videoRepoMock.Setup(repo => repo.AddAsync(video)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(uow => uow.Video).Returns(videoRepoMock.Object);

            // Act
            await _videoService.AddVideoAsync(video);

            // Assert
            videoRepoMock.Verify(repo => repo.AddAsync(video), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddVideoAsync_ThrowsException_WhenVideoUrlIsInvalid()
        {
            // Arrange
            var video = new Video
            {
                Title = "Test Video",
                VideoUrl = "invalid-url", 
                UserId = "user123",
                ChannelDataId = 1,
                viewCount = 0,
                AddByDate = DateTime.Now
            };

            var videoRepoMock = new Mock<IVideoRepository>();
            videoRepoMock.Setup(repo => repo.AddAsync(video)).ThrowsAsync(new ArgumentException("Invalid VideoUrl format."));
            _unitOfWorkMock.Setup(uow => uow.Video).Returns(videoRepoMock.Object);

            // Act
            await _videoService.AddVideoAsync(video);
        }

        [TestMethod]
        public async Task GetVideosByUserIdAsync_ReturnsVideos_WhenVideosExist()
        {
            // Arrange
            string userId = "user123";
            var videos = new List<Video>
        {
            new Video { Id = 1, Title = "Video 1", VideoUrl = "https://example.com/video1.mp4", UserId = userId, ChannelDataId = 1, viewCount = 10, AddByDate = DateTime.Now },
            new Video { Id = 2, Title = "Video 2", VideoUrl = "https://example.com/video2.mp4", UserId = userId, ChannelDataId = 1, viewCount = 20, AddByDate = DateTime.Now }
        };

            var videoRepoMock = new Mock<IVideoRepository>();
            videoRepoMock.Setup(repo => repo.GetVideosByUserIdAsync(userId)).ReturnsAsync(videos);
            _unitOfWorkMock.Setup(uow => uow.Video).Returns(videoRepoMock.Object);

            // Act
            var result = await _videoService.GetVideosByUserIdAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Video 1", result.ElementAt(0).Title);
            Assert.AreEqual("Video 2", result.ElementAt(1).Title);
        }

        [TestMethod]
        public async Task GetVideosByUserIdAsync_ReturnsEmptyList_WhenNoVideosExist()
        {
            // Arrange
            string userId = "user999";
            var videos = new List<Video>();

            var videoRepoMock = new Mock<IVideoRepository>();
            videoRepoMock.Setup(repo => repo.GetVideosByUserIdAsync(userId)).ReturnsAsync(videos);
            _unitOfWorkMock.Setup(uow => uow.Video).Returns(videoRepoMock.Object);

            // Act
            var result = await _videoService.GetVideosByUserIdAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }
    }
 }
