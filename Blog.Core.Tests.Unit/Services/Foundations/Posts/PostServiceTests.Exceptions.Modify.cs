using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Models.Posts;
using Blog.Core.Models.Posts.Exceptions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Blog.Core.Tests.Unit.Services.Foundations.Posts
{
    public partial class PostServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset randomDatetime = GetRandomDateTimeOffset();
            Post randomPost = CreateRandomModifyPost(randomDatetime);
            Post somePost = randomPost;

            Exception sqlException = GetSqlException();

            var failedPostStorageException = 
                new FailedPostStorageException(sqlException);

            var expectedPostDependencyException = 
                new PostDependencyException(failedPostStorageException);

            this.dateTimeBrokerMock.Setup(broker => 
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);
            // when
            ValueTask<Post> modifyPostTask = 
                this.postService.ModifyPostAsync(somePost);

            // then
            await Assert.ThrowsAsync<PostDependencyException>(() => 
                modifyPostTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker => 
                broker.GetCurrentDateTimeOffset(),
                Times.Once);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedPostDependencyException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.UpdatePostAsync(It.IsAny<Post>()), 
                Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();

            Post randomPost = 
                    CreateRandomModifyPost(randomDateTime);

            Post somePost = randomPost;

            DbUpdateException dbUpdateException = new DbUpdateException();

            var failedPostStorageException = 
                new FailedPostStorageException(dbUpdateException);

            var expectedPostDependencyException = 
                new PostDependencyException(failedPostStorageException);

            this.dateTimeBrokerMock.Setup(broker => 
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateException);
            // when
            ValueTask<Post> modifyPostTask = 
                this.postService.ModifyPostAsync(somePost);

            // then
            await Assert.ThrowsAsync<PostDependencyException>(() => 
                modifyPostTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker => 
                broker.GetCurrentDateTimeOffset(),
                Times.Once);            

            this.loggingBrokerMock.Verify(broker => 
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostDependencyException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.UpdatePostAsync(somePost), 
                Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
