using System;
using System.Threading.Tasks;
using Blog.Core.Models.Posts;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace Blog.Core.Tests.Unit.Services.Foundations.Posts
{
    public partial class PostServiceTests
    {
        [Fact]
        public async Task ShouldRemovePostById()
        {
            // given
            Guid randomPostId = Guid.NewGuid();
            Guid inputPostId = randomPostId;
            Post randomPost = CreateRandomPost();
            Post storagePost = randomPost;
            Post deletedPost = storagePost.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPostByIdAsync(inputPostId))
                    .ReturnsAsync(storagePost);

            this.storageBrokerMock.Setup(broker =>
                broker.DeletePostAsync(storagePost))
                    .ReturnsAsync(deletedPost);

            // when
            Post actualPost =
                await this.postService.RemovePostByIdAsync(inputPostId);

            // then
            actualPost.Should().BeEquivalentTo(deletedPost);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPostByIdAsync(It.IsAny<Guid>()),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeletePostAsync(It.IsAny<Post>()),
                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
