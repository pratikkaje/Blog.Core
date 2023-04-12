using Blog.Core.Models.Posts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;

namespace Blog.Core.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Post> Posts { get; set; }
        public async ValueTask<Post> InsertPostAsync(Post post)
        {
            using var broker = 
                new StorageBroker(this.configuration);

            EntityEntry<Post> postEntityEntry = 
                await broker.Posts.AddAsync(post);

            await broker.SaveChangesAsync();

            return postEntityEntry.Entity;
        }
    }
}
