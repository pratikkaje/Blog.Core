using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Models.Posts;
using Blog.Core.Models.Posts.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace Blog.Core.Tests.Unit.Services.Foundations.Posts
{
    public partial class PostServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDepedencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid somePostId = Guid.NewGuid();
            SqlException sqlException = GetSqlException();

            var failedPostStorageException = 
                new FailedPostStorageException(sqlException);

            var expectedPostDependencyException = 
                new PostDependencyException(failedPostStorageException);

            this.storageBrokerMock.Setup(broker => 
                broker.SelectPostByIdAsync(somePostId))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Post> retrievePostByIdTask = 
                this.postService.RetrievePostByIdAsync(somePostId);

            // then
            await Assert.ThrowsAsync<PostDependencyException>(() =>
                retrievePostByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker => 
                broker.SelectPostByIdAsync(It.IsAny<Guid>()),
                Times.Once);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedPostDependencyException))),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
