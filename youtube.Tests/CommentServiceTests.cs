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
    public class CommentServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private CommentService _commentService;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _commentService = new CommentService(_unitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task AddAsync_AddsComment_Successfully()
        {
            // Arrange
            var comment = new Comment
            {
                userComment = "Great video!",
                postedBy = DateTime.UtcNow,
                UserId = "user123",
                VideoId = 1
            };

            var commentRepoMock = new Mock<ICommentRepository>();
            commentRepoMock.Setup(repo => repo.AddAsync(comment)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(uow => uow.Comment).Returns(commentRepoMock.Object);

            // Act
            await _commentService.AddAsync(comment);

            // Assert
            commentRepoMock.Verify(repo => repo.AddAsync(comment), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddAsync_ThrowsException_WhenCommentExceedsMaxLength()
        {
            // Arrange
            var comment = new Comment
            {
                userComment = new string('a', 501), // Exceeds [MaxLength(500)]
                postedBy = DateTime.UtcNow,
                UserId = "user123",
                VideoId = 1
            };

            var commentRepoMock = new Mock<ICommentRepository>();
            commentRepoMock.Setup(repo => repo.AddAsync(comment)).ThrowsAsync(new ArgumentException("Comment exceeds maximum length of 500 characters."));
            _unitOfWorkMock.Setup(uow => uow.Comment).Returns(commentRepoMock.Object);

            // Act
            await _commentService.AddAsync(comment);
        }

        [TestMethod]
        public async Task GetVideoCommentsAsync_ReturnsComments_WhenCommentsExist()
        {
            // Arrange
            int videoId = 1;
            var comments = new List<Comment>
        {
            new Comment { Id = 1, userComment = "Great video!", postedBy = DateTime.UtcNow, UserId = "user123", VideoId = videoId },
            new Comment { Id = 2, userComment = "Nice content!", postedBy = DateTime.UtcNow, UserId = "user456", VideoId = videoId }
        };

            var commentRepoMock = new Mock<ICommentRepository>();
            commentRepoMock.Setup(repo => repo.GetVideoCommentsAsync(videoId)).ReturnsAsync(comments);
            _unitOfWorkMock.Setup(uow => uow.Comment).Returns(commentRepoMock.Object);

            // Act
            var result = await _commentService.GetVideoCommentsAsync(videoId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Great video!", result.ElementAt(0).userComment);
            Assert.AreEqual("Nice content!", result.ElementAt(1).userComment);
        }

        [TestMethod]
        public async Task GetVideoCommentsAsync_ReturnsEmptyList_WhenNoCommentsExist()
        {
            // Arrange
            int videoId = 999;
            var comments = new List<Comment>();

            var commentRepoMock = new Mock<ICommentRepository>();
            commentRepoMock.Setup(repo => repo.GetVideoCommentsAsync(videoId)).ReturnsAsync(comments);
            _unitOfWorkMock.Setup(uow => uow.Comment).Returns(commentRepoMock.Object);

            // Act
            var result = await _commentService.GetVideoCommentsAsync(videoId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task DeleteCommentAsync_ThrowsNotImplementedException()
        {
            // Act
            await _commentService.DeleteCommentAsync(1);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task UpdateAsync_ThrowsNotImplementedException()
        {
            // Arrange
            var comment = new Comment
            {
                Id = 1,
                userComment = "Updated comment",
                postedBy = DateTime.UtcNow,
                UserId = "user123",
                VideoId = 1
            };

            // Act
            await _commentService.UpdateAsync(comment);
        }
    }
}
