using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Models.Posts;
using Blog.Core.Models.Posts.Exceptions;
using Moq;
using Xunit;

namespace Blog.Core.Tests.Unit.Services.Foundations.Posts
{
    public partial class PostServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidPostId = Guid.Empty;

            var invalidPostException = 
                new InvalidPostException();

            invalidPostException.AddData(
                key: nameof(Post.Id), 
                values: "Id is required.");

            var expectedPostValidationException = 
                new PostValidationException(invalidPostException);

            // when
            ValueTask<Post> removePostByIdTask = 
                this.postService.RemovePostByIdAsync(invalidPostId);

            // then
            await Assert.ThrowsAsync<PostValidationException>(() => 
                removePostByIdTask.AsTask());

            this.loggingBrokerMock.Verify(broker => 
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostValidationException))),Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.DeletePostAsync(It.IsAny<Post>()),
                Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveIfPostNotFoundAndLogItAsync()
        {
            // given
            Guid invalidPostId = Guid.NewGuid();
            Post noPost = null;

            var notFoundPostException = 
                new NotFoundPostException(invalidPostId);

            var expectedPostValidationException = 
                new PostValidationException(notFoundPostException);

            this.storageBrokerMock.Setup(broker => 
                broker.SelectPostByIdAsync(invalidPostId))
                    .ReturnsAsync(noPost);

            // when
            ValueTask<Post> removePostByIdTask = 
                this.postService.RemovePostByIdAsync(invalidPostId);

            // then
            await Assert.ThrowsAsync<PostValidationException>(() => 
                removePostByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker => 
                broker.SelectPostByIdAsync(It.IsAny<Guid>()), 
                Times.Once);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostValidationException))), 
                    Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.DeletePostAsync(It.IsAny<Post>()),
                Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
