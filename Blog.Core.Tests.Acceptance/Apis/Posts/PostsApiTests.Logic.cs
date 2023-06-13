using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Core.Models.Posts;
using FluentAssertions;
using Xunit;

namespace Blog.Core.Tests.Acceptance.Apis.Posts
{
    public partial class PostsApiTests
    {
        [Fact]
        public async Task ShouldPostPostAsync()
        {
            // given
            Post randomPost = CreateRandomPost();
            Post inputPost = randomPost;
            Post expectedPost = inputPost;

            // when
            Post actualPost = await this.apiBroker.PostPostAsync(inputPost);

            // then
            actualPost.Should().BeEquivalentTo(expectedPost);
        }

        [Fact]
        public async Task ShouldGetAllPostsAsync()
        {
            // given
            List<Post> randomPosts = await CreateRandomPostedPostAsync();

            List<Post> expectedPosts = randomPosts;

            // when
            List<Post> actualPosts = await this.apiBroker.GetAllPostsAsync();

            // then
            foreach (Post expectedPost in expectedPosts)
            {
                Post actualPost =
                    actualPosts.Single(post => post.Id == expectedPost.Id);

                actualPost.Should().BeEquivalentTo(expectedPost);
                //delete call to be added
            }

        }

        [Fact]
        public async Task ShouldGetPostByIdAsync()
        {
            // given
            Post randomPost = await PostRandomPostAsync();
            Post expectedPost = randomPost;

            // when
            Post actualPost = await this.apiBroker.GetPostByIdAsync(randomPost.Id);

            // then
            actualPost.Should().BeEquivalentTo(expectedPost);
            //Add deletecall
        }

        [Fact]
        public async Task ShouldPutPostAsync()
        {
            // given
            Post randomPost = await PostRandomPostAsync();
            Post modifiedPost = UpdateRandomPost(randomPost);

            // when
            await this.apiBroker.PutPostByIdAsync(modifiedPost);

            Post actualPost =
                await this.apiBroker.GetPostByIdAsync(randomPost.Id);

            // then
            actualPost.Should().BeEquivalentTo(modifiedPost);
            //delete post
        }
    }
}
