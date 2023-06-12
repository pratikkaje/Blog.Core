using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Core.Models.Posts;

namespace Blog.Core.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string PostsRelativeUrl = "api/posts";

        public async ValueTask<Post> PostPostAsync(Post post) =>
            await this.apiFactoryClient.PostContentAsync(PostsRelativeUrl, post);

        public async ValueTask<List<Post>> GetAllPostsAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<Post>>($"{PostsRelativeUrl}/");

        public async ValueTask<Post> GetPostByIdAsync(Guid postId) =>
            await this.apiFactoryClient.GetContentAsync<Post>($"{PostsRelativeUrl}/{postId}");

        public async ValueTask<Post> PutPostByIdAsync(Post post) =>
            await this.apiFactoryClient.PutContentAsync(PostsRelativeUrl, post);
    }
}
