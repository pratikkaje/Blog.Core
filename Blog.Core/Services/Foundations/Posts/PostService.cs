﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Blog.Core.Brokers.DateTimes;
using Blog.Core.Brokers.Loggings;
using Blog.Core.Brokers.Storages;
using Blog.Core.Models.Posts;

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

        public async ValueTask<Post> ModifyPostAsync(Post post)
        {
            this.dateTimeBroker.GetCurrentDateTimeOffset();

            Post retrievedPost = 
                await this.storageBroker.SelectPostByIdAsync(post.Id);

            return await this.storageBroker.UpdatePostAsync(post);
        }
    }
}