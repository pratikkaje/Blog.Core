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
        public async Task ShouldThrowValidationExceptionOnModifyIfPostIsNullAndLogItAsync()
        {
            // given
            Post inputPost = null;

            var nullPostException = 
                new NullPostException();

            var expectedPostValidationException = 
                new PostValidationException(nullPostException);

            // when
            ValueTask<Post> modifyPostTask = this.postService.ModifyPostAsync(inputPost);

            // then
            await Assert.ThrowsAsync<PostValidationException>(() => 
                modifyPostTask.AsTask());

            this.loggingBrokerMock.Verify(broker => 
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostValidationException))), 
                    Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.UpdatePostAsync(inputPost), 
                Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
