using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using youtube.Application.Common.Interfaces;
using youtube.Application.Services.Implementation;
using youtube.Domain.Entities;

namespace youtube.Tests
{

    [TestClass]
    public class ChannelServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private ChannelService _channelService;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _channelService = new ChannelService(_unitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task CreateChannelAsync_CreatesChannel_ReturnsChannelData()
        {
            // Arrange
            string userId = "user123";
            string name = "TestChannel";
            var channel = new ChannelData { Id = 1, Name = name, Handle = "testchannel", UserId = userId };

            var channelRepoMock = new Mock<IChannelRepository>();
            channelRepoMock.Setup(repo => repo.CreateChannelAsync(userId, name)).ReturnsAsync(channel);
            _unitOfWorkMock.Setup(uow => uow.Channel).Returns(channelRepoMock.Object);

            // Act
            var result = await _channelService.CreateChannelAsync(userId, name);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(channel.Id, result.Id);
            Assert.AreEqual(channel.Name, result.Name);
            Assert.AreEqual(channel.Handle, result.Handle);
            Assert.AreEqual(channel.UserId, result.UserId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CreateChannelAsync_ThrowsException_WhenNameIsEmpty()
        {
            // Arrange
            string userId = "user123";
            string name = ""; // Violates [Required] and [MaxLength(100)]

            var channelRepoMock = new Mock<IChannelRepository>();
            channelRepoMock.Setup(repo => repo.CreateChannelAsync(userId, name)).ThrowsAsync(new ArgumentException("Name is required."));
            _unitOfWorkMock.Setup(uow => uow.Channel).Returns(channelRepoMock.Object);

            // Act
            await _channelService.CreateChannelAsync(userId, name);
        }

        [TestMethod]
        public async Task UpdateChannelDataAsync_UpdatesChannel_ReturnsUpdatedChannelData()
        {
            // Arrange
            int channelId = 1;
            string bannerImageUrl = "https://example.com/banner.jpg";
            string profilePictureUrl = "https://example.com/profile.jpg";
            string name = "UpdatedChannel";
            string handle = "updatedhandle";
            string description = "Updated description";
            var updatedChannel = new ChannelData
            {
                Id = channelId,
                BannerImageUrl = bannerImageUrl,
                ProfilePictureUrl = profilePictureUrl,
                Name = name,
                Handle = handle,
                Description = description,
                UserId = "user123"
            };

            var channelRepoMock = new Mock<IChannelRepository>();
            channelRepoMock.Setup(repo => repo.UpdateChannelDataAsync(channelId, bannerImageUrl, profilePictureUrl, name, handle, description))
                           .ReturnsAsync(updatedChannel);
            _unitOfWorkMock.Setup(uow => uow.Channel).Returns(channelRepoMock.Object);

            // Act
            var result = await _channelService.UpdateChannelDataAsync(channelId, bannerImageUrl, profilePictureUrl, name, handle, description);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(updatedChannel.Name, result.Name);
            Assert.AreEqual(updatedChannel.Handle, result.Handle);
            Assert.AreEqual(updatedChannel.Description, result.Description);
            Assert.AreEqual(updatedChannel.BannerImageUrl, result.BannerImageUrl);
            Assert.AreEqual(updatedChannel.ProfilePictureUrl, result.ProfilePictureUrl);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task UpdateChannelDataAsync_ThrowsException_WhenHandleExceedsMaxLength()
        {
            // Arrange
            int channelId = 1;
            string bannerImageUrl = "https://example.com/banner.jpg";
            string profilePictureUrl = "https://example.com/profile.jpg";
            string name = "UpdatedChannel";
            string handle = new string('a', 51); 
            string description = "Updated description";

            var channelRepoMock = new Mock<IChannelRepository>();
            channelRepoMock.Setup(repo => repo.UpdateChannelDataAsync(channelId, bannerImageUrl, profilePictureUrl, name, handle, description))
                           .ThrowsAsync(new ArgumentException("Handle exceeds maximum length of 50 characters."));
            _unitOfWorkMock.Setup(uow => uow.Channel).Returns(channelRepoMock.Object);

            // Act
            await _channelService.UpdateChannelDataAsync(channelId, bannerImageUrl, profilePictureUrl, name, handle, description);
        }
    }

    }
