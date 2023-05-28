using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
