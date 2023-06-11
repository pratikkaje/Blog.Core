using System;
using System.Threading.Tasks;
using Blog.Core.Models.Posts;
using Blog.Core.Models.Posts.Exceptions;
using Force.DeepCloner;
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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfPostIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            Post invalidPost = new Post
            {
                Author = invalidText
            };

            var invalidPostException = new InvalidPostException();

            invalidPostException.AddData(
                key: nameof(Post.Id),
                values: "Id is required.");

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
                "Date is required.",
                $"Date is same as {nameof(Post.CreatedDate)}");

            var expectedPostValidationException =
                new PostValidationException(invalidPostException);

            // when
            ValueTask<Post> modifyPostTask =
                this.postService.ModifyPostAsync(invalidPost);

            // then
            await Assert.ThrowsAsync<PostValidationException>(() =>
                modifyPostTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdatePostAsync(It.IsAny<Post>()),
                Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Post randomPost = CreateRandomPost(randomDateTime);
            Post invalidPost = randomPost;
            var invalidPostException = new InvalidPostException();

            invalidPostException.AddData(
                key: nameof(Post.UpdatedDate),
                values: $"Date is same as {nameof(Post.CreatedDate)}");

            var expectedPostValidationException =
                new PostValidationException(invalidPostException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTime);

            // when
            ValueTask<Post> modifyPostTask = this.postService.ModifyPostAsync(invalidPost);

            // then
            await Assert.ThrowsAsync<PostValidationException>(() =>
                modifyPostTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPostByIdAsync(It.IsAny<Guid>()),
                Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidMinuteCases))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(int minutes)
        {
            // given
            DateTimeOffset dateTime = GetRandomDateTimeOffset();
            Post randomPost = CreateRandomPost(dateTime);
            Post inputPost = randomPost;

            inputPost.UpdatedDate = 
                dateTime.AddMinutes(minutes);

            var invalidPostException = 
                new InvalidPostException();

            invalidPostException.AddData(
                key: nameof(Post.UpdatedDate), 
                values: "Date is not recent.");

            var expectedPostValidationException = 
                new PostValidationException(invalidPostException);

            this.dateTimeBrokerMock.Setup(broker => 
                broker.GetCurrentDateTimeOffset())
                    .Returns(dateTime);

            // when
            ValueTask<Post> modifyPostTask = 
                this.postService.ModifyPostAsync(inputPost);

            // then
            await Assert.ThrowsAsync<PostValidationException>(() =>
                modifyPostTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker => 
                broker.GetCurrentDateTimeOffset(),
                Times.Once());

            this.loggingBrokerMock.Verify(broker => 
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.SelectPostByIdAsync(It.IsAny<Guid>()), 
                Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValdationExceptionOnModifyIfPostDoesNotExistAndLogItAsync()
        {
            // given
            int randomNegativeMinutes = GetRandomNegativeNumber();
            DateTimeOffset dateTime = GetRandomDateTimeOffset();
            Post nonExistPost = CreateRandomPost(dateTime);
            nonExistPost.CreatedDate = dateTime.AddMinutes(randomNegativeMinutes);
            Guid incorrectPostId = nonExistPost.Id;
            Post nullPost = null;

            var notFoundPostException = 
                new NotFoundPostException(incorrectPostId);

            var expectedPostValidationException = 
                new PostValidationException(notFoundPostException);

            this.storageBrokerMock.Setup(broker => 
                broker.SelectPostByIdAsync(incorrectPostId))
                    .ReturnsAsync(nullPost);

            this.dateTimeBrokerMock.Setup(broker => 
                broker.GetCurrentDateTimeOffset())
                    .Returns(dateTime);

            // when
            ValueTask<Post> modifyPostTask = 
                this.postService.ModifyPostAsync(nonExistPost);

            // then
            await Assert.ThrowsAsync<PostValidationException>(() => 
                modifyPostTask.AsTask());

            this.storageBrokerMock.Verify(broker => 
                broker.SelectPostByIdAsync(incorrectPostId),
                Times.Once);
            
            this.dateTimeBrokerMock.Verify(broker => 
                broker.GetCurrentDateTimeOffset(),
                Times.Once);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.UpdatePostAsync(It.IsAny<Post>()), 
                Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            Post randomPost = 
                CreateRandomModifyPost(randomDateTimeOffset);

            Post invalidPost = randomPost.DeepClone();
            Post storagePost = invalidPost.DeepClone();

            storagePost.CreatedDate = 
                storagePost.CreatedDate.AddMinutes(randomMinutes);

            storagePost.UpdatedDate = 
                storagePost.UpdatedDate.AddMinutes(randomMinutes);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker => 
                broker.SelectPostByIdAsync(invalidPost.Id))
                    .ReturnsAsync(storagePost);

            var invalidPostException =
                new InvalidPostException();

            invalidPostException.AddData(
                key: nameof(Post.CreatedDate),
                values: $"Date is not same as {nameof(Post.CreatedDate)}");

            var expectedPostValidationException =
                new PostValidationException(invalidPostException);

            // when
            ValueTask<Post> modifyPostTask = 
                this.postService.ModifyPostAsync(invalidPost); ;

            // then
            await Assert.ThrowsAsync<PostValidationException>(() => 
                modifyPostTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker => 
                broker.GetCurrentDateTimeOffset(),
                Times.Once());

            this.storageBrokerMock.Verify(broker => 
                broker.SelectPostByIdAsync(invalidPost.Id),
                Times.Once());

            this.loggingBrokerMock.Verify(broker => 
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostValidationException))),
                    Times.Once());

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Post randomPost = CreateRandomModifyPost(randomDateTime);
            Post invalidPost = randomPost;
            Post storagePost = randomPost;

            this.dateTimeBrokerMock.Setup(broker => 
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTime);
            
            this.storageBrokerMock.Setup(broker => 
                broker.SelectPostByIdAsync(invalidPost.Id))
                    .ReturnsAsync(storagePost);

            var invalidPostException = new InvalidPostException();

            invalidPostException.AddData(
                key: nameof(Post.UpdatedDate), 
                values: $"Date is same as {nameof(Post.UpdatedDate)}");

            var expectedPostValidationException = 
                new PostValidationException(invalidPostException);

            // when
            ValueTask<Post> modifyPostTask = 
                this.postService.ModifyPostAsync(invalidPost);

            // then
            await Assert.ThrowsAsync<PostValidationException>(() => 
                modifyPostTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker => 
                broker.GetCurrentDateTimeOffset(),
                Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.SelectPostByIdAsync(invalidPost.Id),
                Times.Once());

            this.loggingBrokerMock.Verify(broker => 
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostValidationException))),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();            
        }
    }
}
