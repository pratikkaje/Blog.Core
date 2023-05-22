using System.Threading.Tasks;
using Blog.Core.Models.Posts;
using FluentAssertions;
using Moq;
using Xunit;

namespace Blog.Core.Tests.Unit.Services.Foundations.Posts
{
    public partial class PostServiceTests
    {
        [Fact]
        public async Task ShouldAddPostAsync()
        {
            // given
            Post randomPost = CreateRandomPost();
            Post inputPost = randomPost;
            Post storagePost = inputPost;

            this.storageBrokerMock.Setup(broker =>
                broker.InsertPostAsync(inputPost)).ReturnsAsync(storagePost);

            // when
            Post actualPost =
                await this.postService.AddPostAsync(inputPost);

            // then
            actualPost.Should().BeEquivalentTo(storagePost);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPostAsync(It.IsAny<Post>())
                    , Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}