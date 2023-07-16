using System.Threading.Tasks;
using Blog.Core.Models.Profiles;

namespace Blog.Core.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Profile> InsertProfileAsync(Profile profile);
    }
}
