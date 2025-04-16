
using System.Threading.Tasks;
using youtube.Application.Services.Implementation;
using youtube.Domain.Entities;
using youtube.Infrastrcture.Repository;

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
        var channel = new Channel { Id = 1, Name = name, Handle = "testchannel", UserId = userId };

        // Mock the Channel property and its CreateChannelAsync method
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
    public async Task GetChannelDataByIdAsync_ReturnsChannelData_WhenChannelExists()
    {
        // Arrange
        int channelId = 1;
        var channelData = new ChannelData { Id = channelId, Name = "TestChannel", Handle = "testchannel", UserId = "user123" };

        // Mock the Channel property and its GetChannelDataByIdAsync method
        var channelRepoMock = new Mock<IChannelRepository>();
        channelRepoMock.Setup(repo => repo.GetChannelDataByIdAsync(channelId)).ReturnsAsync(channelData);
        _unitOfWorkMock.Setup(uow => uow.Channel).Returns(channelRepoMock.Object);

        // Act
        var result = await _channelService.GetChannelDataByIdAsync(channelId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(channelData.Id, result.Id);
        Assert.AreEqual(channelData.Name, result.Name);
    }

    [TestMethod]
    public async Task GetChannelDataByIdAsync_ReturnsNull_WhenChannelDoesNotExist()
    {
        // Arrange
        int channelId = 999;

        // Mock the Channel property to return null
        var channelRepoMock = new Mock<IChannelRepository>();
        channelRepoMock.Setup(repo => repo.GetChannelDataByIdAsync(channelId)).ReturnsAsync((ChannelData)null);
        _unitOfWorkMock.Setup(uow => uow.Channel).Returns(channelRepoMock.Object);

        // Act
        var result = await _channelService.GetChannelDataByIdAsync(channelId);

        // Assert
        Assert.IsNull(result);
    }
}
