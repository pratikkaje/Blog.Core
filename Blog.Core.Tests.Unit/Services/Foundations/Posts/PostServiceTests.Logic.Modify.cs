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
        public async Task ShouldModifyPostAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDateTimeOffset();
            Post randomPost = CreateRandomPost(randomDate);
            Post storagePost = randomPost.DeepClone();

            Post inputPost = storagePost;
            inputPost.UpdatedDate =
                storagePost.CreatedDate.AddDays(GetRandomNumber());

            Post updatedPost = inputPost;
            Post expectedPost = updatedPost.DeepClone();
            Guid postId = inputPost.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPostByIdAsync(postId))
                    .ReturnsAsync(storagePost);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdatePostAsync(inputPost))
                    .ReturnsAsync(updatedPost);

            // when
            Post actualPost = await this.postService.ModifyPostAsync(inputPost);

            // then
            actualPost.Should().BeEquivalentTo(expectedPost);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPostByIdAsync(postId),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdatePostAsync(storagePost),
                Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
