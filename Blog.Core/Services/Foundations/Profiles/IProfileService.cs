using System.Threading.Tasks;
using Blog.Core.Models.Profiles;

namespace Blog.Core.Services.Foundations.Profiles
{
    public interface IProfileService
    {
        ValueTask<Profile> AddProfileAsync(Profile profile);
    }
}
