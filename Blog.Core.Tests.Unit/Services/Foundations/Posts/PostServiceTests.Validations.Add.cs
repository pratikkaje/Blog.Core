using System;
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
                key: nameof(Post.SubTitle),
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
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateDateIsNotSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            int randomNumber = GetRandomNumber();
            Post randomPost = CreateRandomPost(randomDateTime);
            Post invalidPost = randomPost;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTime);

            invalidPost.UpdatedDate =
                invalidPost.CreatedDate.AddDays(randomNumber);

            var invalidPostException =
                new InvalidPostException();

            invalidPostException.AddData(
                key: nameof(Post.UpdatedDate),
                values: $"Date is not same as the {nameof(Post.CreatedDate)}");

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

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPostAsync(It.IsAny<Post>()),
                Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async Task ShouldThrowValidationExcptionOnAddIfCreatedDateIsNotRecent(
            int minutesBeforeOrAfter)
        {
            // given
            DateTimeOffset randomDateTime =
                GetRandomDateTimeOffset();

            DateTimeOffset invalidDateTime =
                randomDateTime.AddMinutes(minutesBeforeOrAfter);

            var randomPost = CreateRandomPost(invalidDateTime);
            var invalidPost = randomPost;

            var invalidPostException =
                new InvalidPostException();

            invalidPostException.AddData(
                key: nameof(Post.CreatedDate),
                values: "Date is not recent.");

            var expectedPostValidationException =
                new PostValidationException(invalidPostException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTime);

            // when
            ValueTask<Post> addPropertyTask = this.postService.AddPostAsync(invalidPost);

            // then
            await Assert.ThrowsAsync<PostValidationException>(() =>
                addPropertyTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostValidationException))),
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