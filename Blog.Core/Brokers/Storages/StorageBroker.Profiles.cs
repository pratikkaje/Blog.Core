using Blog.Core.Models.Posts;
using Blog.Core.Models.Profiles;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Blog.Core.Brokers.Storages
{
    public partial class StorageBroker : IStorageBroker
    {
        public DbSet<Profile> Profiles { get; set; }

        public ValueTask<Profile> InsertProfileAsync(Profile profile)
        {
            throw new System.NotImplementedException();
        }
    }
}
