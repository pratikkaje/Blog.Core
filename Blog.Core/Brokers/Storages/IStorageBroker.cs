using Blog.Core.Models.Posts;
using System.Threading.Tasks;

namespace Blog.Core.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Post> InsertPostAsync(Post post);
    }
}
