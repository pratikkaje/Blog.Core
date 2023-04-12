using Blog.Core.Models.Posts;
using System.Threading.Tasks;

namespace Blog.Core.Services.Foundations.Posts
{
    public interface IPostService
    {
        ValueTask<Post> AddPostAsync(Post post);
    }
}
