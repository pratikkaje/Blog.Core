using System;
using System.Linq;
using System.Threading.Tasks;
using Blog.Core.Models.Posts;

namespace Blog.Core.Services.Foundations.Posts
{
    public interface IPostService
    {
        ValueTask<Post> AddPostAsync(Post post);
        IQueryable<Post> RetrieveAllPosts();
        ValueTask<Post> RetrievePostByIdAsync(Guid PostId);
    }
}
