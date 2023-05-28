using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Models.Posts;

namespace Blog.Core.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string PostsRelativeUrl = "api/posts";

        public async ValueTask<Post> PostPostAsync(Post post) =>
            await this.apiFactoryClient.PostContentAsync(PostsRelativeUrl, post);
    }
}
