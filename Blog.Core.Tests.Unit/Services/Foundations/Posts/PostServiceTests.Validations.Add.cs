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
        public async Task ShouldThrowValidationExceptionOnAddIfPropertyIsNullAndLogItAsync()
        {
            // given
            Post nullPost = null;

            var nullPostException =
                new NullPostException();

            var expectedPostValidationException =
                new PostValidationException(nullPostException);

            // when
            ValueTask<Post> addPostTask =
                this.postService.AddPostAsync(nullPost);

            // then
            await Assert.ThrowsAsync<PostValidationException>(() =>
                addPostTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPostAsync(It.IsAny<Post>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfPostIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            var invalidPost = new Post
            {
                Title = invalidText
            };

            var invalidPostException =
                new InvalidPostException();

            invalidPostException.AddData(
                key: nameof(Post.Id),
                values: "Id is required.");

            invalidPostException.AddData(
                key: nameof(Post.Title),
                values: "Text is required.");

            invalidPostException.AddData(
                key: nameof(Post.BriefDescription),
                values: "Text is required.");

            invalidPostException.AddData(
                key: nameof(Post.Content),
                values: "Text is required.");

            invalidPostException.AddData(
                key: nameof(Post.Author),
                values: "Text is required.");

            invalidPostException.AddData(
                key: nameof(Post.CreatedDate),
                values: "Date is required.");

            invalidPostException.AddData(
                key: nameof(Post.UpdatedDate),
                values: "Date is required.");

            var expectedPostValidationException =
                new PostValidationException(invalidPostException);

            // when
            ValueTask<Post> addPostTask =
                this.postService.AddPostAsync(invalidPost);

            // then
            await Assert.ThrowsAsync<PostValidationException>(() =>
                addPostTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPostAsync(It.IsAny<Post>()),
                Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}