using System.Linq;
using System.Threading.Tasks;
using Blog.Core.Models.Posts;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace Blog.Core.Tests.Unit.Services.Foundations.Posts
{
    public partial class PostServiceTests
    {
        [Fact]
        public async Task ShouldReturnPosts()
        {
            // given
            IQueryable<Post> randomPosts = CreateRandomPosts();
            IQueryable<Post> storagePosts = randomPosts;
            IQueryable<Post> expectedPosts = storagePosts.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllPosts())
                    .Returns(storagePosts);

            // when
            var actualPosts =
                this.postService.RetrieveAllPosts();

            // then
            actualPosts.Should().BeEquivalentTo(expectedPosts);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllPosts(),
                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
