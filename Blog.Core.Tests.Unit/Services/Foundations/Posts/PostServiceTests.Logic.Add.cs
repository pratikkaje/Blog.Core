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
        public async Task ShouldAddPostAsync()
        {
            // given
            DateTimeOffset dateTime = GetRandomDateTimeOffset();
            Post randomPost = CreateRandomPost(dateTime);
            Post inputPost = randomPost;
            Post storagePost = inputPost;
            Post expectedPost = storagePost.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(dateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertPostAsync(inputPost))
                    .ReturnsAsync(storagePost);

            // when
            Post actualPost =
                await this.postService.AddPostAsync(inputPost);

            // then
            actualPost.Should().BeEquivalentTo(expectedPost);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPostAsync(It.IsAny<Post>()),
                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}