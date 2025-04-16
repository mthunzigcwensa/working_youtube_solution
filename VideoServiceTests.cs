using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using youtube.Application.Services.Implementation;
using youtube.Domain.Entities;
using youtube.Infrastrcture.Repository;

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
    public async Task GetVideoByIdAsync_ReturnsVideo_WhenVideoExists()
    {
        // Arrange
        int videoId = 1;
        var video = new Video { Id = videoId, Title = "Test Video", Views = 0 };

        // Mock the Video property and its GetByIdAsync method
        var videoRepoMock = new Mock<IVideoRepository>();
        videoRepoMock.Setup(repo => repo.GetByIdAsync(videoId)).ReturnsAsync(video);
        _unitOfWorkMock.Setup(uow => uow.Video).Returns(videoRepoMock.Object);

        // Act
        var result = await _videoService.GetVideoByIdAsync(videoId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(video.Id, result.Id);
        Assert.AreEqual(video.Title, result.Title);
    }

    [TestMethod]
    public async Task GetVideoByIdAsync_ReturnsNull_WhenVideoDoesNotExist()
    {
        // Arrange
        int videoId = 999;

        // Mock the Video property to return null
        var videoRepoMock = new Mock<IVideoRepository>();
        videoRepoMock.Setup(repo => repo.GetByIdAsync(videoId)).ReturnsAsync((Video)null);
        _unitOfWorkMock.Setup(uow => uow.Video).Returns(videoRepoMock.Object);

        // Act
        var result = await _videoService.GetVideoByIdAsync(videoId);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task AddView_CallsRepositoryMethod_Once()
    {
        // Arrange
        int videoId = 1;

        // Mock the Video property and its AddViewAsync method
        var videoRepoMock = new Mock<IVideoRepository>();
        videoRepoMock.Setup(repo => repo.AddViewAsync(videoId)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.Video).Returns(videoRepoMock.Object);

        // Act
        await _videoService.AddView(videoId);

        // Assert
        videoRepoMock.Verify(repo => repo.AddViewAsync(videoId), Times.Once());
    }
}