using System;
using Blog.Core.Models.Posts;
using Blog.Core.Tests.Acceptance.Brokers;
using Tynamix.ObjectFiller;
using Xunit;

namespace Blog.Core.Tests.Acceptance.Apis.Posts
{
    [Collection(nameof(ApiTestCollection))]
    public partial class PostsApiTests
    {
        private readonly ApiBroker apiBroker;
        public PostsApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private static Post CreateRandomPost() =>
            CreateRandomPostFiller().Create();

        private static Filler<Post> CreateRandomPostFiller()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<Post>();

            filler.Setup()
                .OnProperty(post => post.CreatedDate).Use(now)
                .OnProperty(post => post.UpdatedDate).Use(now);

            return filler;
        }
    }
}
