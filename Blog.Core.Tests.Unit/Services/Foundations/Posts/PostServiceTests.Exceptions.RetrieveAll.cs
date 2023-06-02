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
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlExceptionOccurrsAndLogItAsync()
        {
            // given
            SqlException sqlException = GetSqlException();

            var failedStorageException = 
                new FailedPostStorageException(sqlException);

            var expectedPostDependencyException = 
                new PostDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllPosts())
                    .Throws(sqlException);

            // when
            Action retrieveAllPostsAction = () => 
                this.postService.RetrieveAllPosts();

            // then
            Assert.Throws<PostDependencyException>(retrieveAllPostsAction);

            this.storageBrokerMock.Verify(broker => 
                broker.SelectAllPosts(),
                Times.Once);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedPostDependencyException))),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Exception serviceException = new Exception();

            var failedPostServiceException = 
                new FailedPostServiceException(serviceException);

            var expectedPostServiceException = 
                new PostServiceException(failedPostServiceException);

            this.storageBrokerMock.Setup(broker => 
                broker.SelectAllPosts())
                    .Throws(serviceException);

            // when
            Action retrieveAllPostsAction = () => 
                this.postService.RetrieveAllPosts();

            // then
            Assert.Throws<PostServiceException>(retrieveAllPostsAction);

            this.storageBrokerMock.Verify(broker => 
                broker.SelectAllPosts(), 
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostServiceException))), 
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }    
    }
}
