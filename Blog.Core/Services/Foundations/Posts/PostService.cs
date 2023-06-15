using System;
using System.Linq;
using System.Threading.Tasks;
using Blog.Core.Brokers.DateTimes;
using Blog.Core.Brokers.Loggings;
using Blog.Core.Brokers.Storages;
using Blog.Core.Models.Posts;
using Microsoft.Extensions.Hosting;

namespace Blog.Core.Services.Foundations.Posts
{
    public partial class PostService : IPostService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        public PostService(IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<Post> AddPostAsync(Post post) =>
            TryCatch(async () =>
            {
                ValidatePostOnAdd(post);

                return await this.storageBroker.InsertPostAsync(post);
            });

        public IQueryable<Post> RetrieveAllPosts() =>
            TryCatch(() => this.storageBroker.SelectAllPosts());

        public ValueTask<Post> RetrievePostByIdAsync(Guid postId) =>
            TryCatch(async () =>
            {
                ValidatePostId(postId);

                Post maybePost =
                    await this.storageBroker.SelectPostByIdAsync(postId);

                ValidateStoragePost(maybePost, postId);

                return maybePost;

            });

        public ValueTask<Post> ModifyPostAsync(Post post) =>
            TryCatch(async () =>
            {
                ValidatePostOnModify(post);

                Post maybePost =
                    await this.storageBroker.SelectPostByIdAsync(post.Id);

                ValidateStoragePost(maybePost, post.Id);
                ValidateAgainstStoragePostOnModify(inputPost: post, storagePost: maybePost);

                return await this.storageBroker.UpdatePostAsync(post);
            });

        public ValueTask<Post> RemovePostByIdAsync(Guid postId) =>
            TryCatch(async () => 
            {
                ValidatePostId(postId);

                Post maybePost =
                    await this.storageBroker.SelectPostByIdAsync(postId);

                ValidateStoragePost(maybePost, postId);
                return await this.storageBroker.DeletePostAsync(maybePost);
            });
    }
}