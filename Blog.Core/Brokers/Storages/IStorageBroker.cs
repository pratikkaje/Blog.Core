using System.Threading.Tasks;
using Blog.Core.Models.Posts;

namespace Blog.Core.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Post> InsertPostAsync(Post post);
    }
}
