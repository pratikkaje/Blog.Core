using System;
using System.Threading.Tasks;
using Blog.Core.Models.Posts;
using Blog.Core.Models.Posts.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Blog.Core.Tests.Unit.Services.Foundations.Posts
{
    public partial class PostServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid randomPostId = Guid.NewGuid();
            Guid inputPostId = randomPostId;
            SqlException sqlException = GetSqlException();

            var failedPostStorageException =
                new FailedPostStorageException(sqlException);

            var expectedPostDependencyException =
                new PostDependencyException(failedPostStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPostByIdAsync(inputPostId))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Post> removePostByIdTask =
                this.postService.RemovePostByIdAsync(inputPostId);

            // then
            await Assert.ThrowsAsync<PostDependencyException>(() =>
                removePostByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPostByIdAsync(It.IsAny<Guid>()),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedPostDependencyException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeletePostAsync(It.IsAny<Post>()),
                Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDBUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid somePostId = Guid.NewGuid();

            var dbUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedPostException =
                new LockedPostException(dbUpdateConcurrencyException);

            var expectedPostDependencyValidationException =
                new PostDependencyValidationException(lockedPostException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPostByIdAsync(somePostId))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<Post> removePostByIdTask =
                this.postService.RemovePostByIdAsync(somePostId);

            // then
            await Assert.ThrowsAsync<PostDependencyValidationException>(() =>
                removePostByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPostByIdAsync(It.IsAny<Guid>()),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostDependencyValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeletePostAsync(It.IsAny<Post>()),
                Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid somPostId = Guid.NewGuid();

            var serviceException = new Exception();

            var failedPostServiceException =
                new FailedPostServiceException(serviceException);

            var expectedPostServiceException =
                new PostServiceException(failedPostServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPostByIdAsync(somPostId))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Post> removePostByIdTask =
                this.postService.RemovePostByIdAsync(somPostId);

            // then
            await Assert.ThrowsAsync<PostServiceException>(() =>
                removePostByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPostByIdAsync(It.IsAny<Guid>()),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostServiceException))),
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
