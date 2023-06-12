using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        private static Post UpdateRandomPost(Post inputPost)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            var filler = new Filler<Post>();

            filler.Setup()
                    .OnProperty(post => post.Id).Use(inputPost.Id)
                    .OnProperty(post => post.CreatedDate).Use(inputPost.CreatedDate)
                    .OnProperty(post => post.UpdatedDate).Use(now);
            //.OnType<DateTimeOffset>().Use

            return filler.Create();
        }

        private static Post CreateRandomPost() =>
            CreateRandomPostFiller().Create();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private async ValueTask<List<Post>> CreateRandomPostedPostAsync()
        {
            var randomPost = CreateRandomPost();
            int randomNumber = GetRandomNumber();
            var randomPosts = new List<Post>();

            for (int i = 0; i < randomNumber; i++)
            {
                randomPosts.Add(await PostRandomPostAsync());
            }

            return randomPosts;
        }

        private async ValueTask<Post> PostRandomPostAsync()
        {
            var randomPost = CreateRandomPost();
            var postedPost =
                await this.apiBroker.PostPostAsync(randomPost);

            return postedPost;
        }

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
