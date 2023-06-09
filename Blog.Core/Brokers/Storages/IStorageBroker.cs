using System;
using System.Linq;
using System.Threading.Tasks;
using Blog.Core.Models.Posts;

namespace Blog.Core.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Post> InsertPostAsync(Post post);
        IQueryable<Post> SelectAllPosts();
        ValueTask<Post> SelectPostByIdAsync(Guid postId);
        ValueTask<Post> UpdatePostAsync(Post post);

    }
}
