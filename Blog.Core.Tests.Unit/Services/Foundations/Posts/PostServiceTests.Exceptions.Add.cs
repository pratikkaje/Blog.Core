using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Models.Posts;
using Blog.Core.Models.Posts.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Blog.Core.Tests.Unit.Services.Foundations.Posts
{
    public partial class PostServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccurrsAndLogItAsync()
        {
            // given
            var somePost = CreateRandomPost();
            SqlException sqlException = GetSqlException();

            var failedPostStorageException = 
                new FailedPostStorageException(sqlException);

            var expectedPostDependencyException = 
                new PostDependencyException(failedPostStorageException);

            this.dateTimeBrokerMock.Setup(broker => 
                broker.GetCurrentDateTimeOffset())
                .Throws(sqlException);

            // when
            ValueTask<Post> addPostTask = 
                this.postService.AddPostAsync(somePost);

            // then
            await Assert.ThrowsAsync<PostDependencyException>(() => 
                addPostTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker => 
                broker.GetCurrentDateTimeOffset(), 
                Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.InsertPostAsync(It.IsAny<Post>()), 
                Times.Never);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedPostDependencyException))), 
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfPostAlreadyExistsAndLogItAsync()
        {
            // given
            var randomPost = CreateRandomPost();
            var alreadyExistPost = randomPost;
            string randomMessage = GetRandomMessage();

            var duplicateKeyException = 
                new DuplicateKeyException(randomMessage);

            var alreadyExistsPostException = 
                new AlreadyExistsPostException(duplicateKeyException);

            var expectedPostDependencyValidationException = 
                new PostDependencyValidationException(alreadyExistsPostException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(duplicateKeyException);

            // when
            ValueTask<Post> addPostTask = 
                this.postService.AddPostAsync(randomPost);

            // then
            await Assert.ThrowsAsync<PostDependencyValidationException>(() => 
                addPostTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker => 
                broker.GetCurrentDateTimeOffset(), 
                Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.InsertPostAsync(randomPost), 
                Times.Never);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostDependencyValidationException))),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            var somePost = CreateRandomPost();

            var dbUpdateException = new DbUpdateException();

            var failedPostStorageException = 
                new FailedPostStorageException(dbUpdateException);

            var expectedPostDependencyException = 
                new PostDependencyException(failedPostStorageException);

            this.dateTimeBrokerMock.Setup(broker => 
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateException);

            // when
            ValueTask<Post> addPostTask = 
                this.postService.AddPostAsync(somePost);

            // then
            await Assert.ThrowsAsync<PostDependencyException>(() => 
                addPostTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker => 
                broker.GetCurrentDateTimeOffset(),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostDependencyException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.InsertPostAsync(It.IsAny<Post>()), 
                Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();            
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

    }
}
