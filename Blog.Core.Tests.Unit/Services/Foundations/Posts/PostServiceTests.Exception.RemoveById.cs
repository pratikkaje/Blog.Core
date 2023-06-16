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
    }
}
