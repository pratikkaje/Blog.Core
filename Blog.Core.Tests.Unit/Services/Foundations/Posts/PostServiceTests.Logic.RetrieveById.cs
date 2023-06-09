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
        public async Task ShouldRetrievePostByIdAsync()
        {
            //given
            Guid randomPostId = Guid.NewGuid();
            Guid inputPostId = randomPostId;
            var randomPost = CreateRandomPost();
            var storagePost = randomPost;
            var expectedPost = storagePost.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPostByIdAsync(inputPostId))
                    .ReturnsAsync(storagePost);

            // when
            Post actualPost =
                await this.postService.RetrievePostByIdAsync(inputPostId);

            // then
            actualPost.Should().BeEquivalentTo(expectedPost);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPostByIdAsync(It.IsAny<Guid>()),
                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

    }
}
