using System.Threading.Tasks;
using Blog.Core.Models.Posts;
using Blog.Core.Models.Posts.Exceptions;
using Xeptions;

namespace Blog.Core.Services.Foundations.Posts
{
    public partial class PostService
    {

        private delegate ValueTask<Post> ReturningPostFunction();

        private async ValueTask<Post> TryCatch(ReturningPostFunction returningPostFunction)
        {
            try
            {
                return await returningPostFunction();
            }
            catch (NullPostException nullPostException)
            {
                throw CreateAndLogValidationException(nullPostException);
            }
            catch (InvalidPostException invalidPostException)
            {
                throw CreateAndLogValidationException(invalidPostException);
            }
        }

        private PostValidationException CreateAndLogValidationException(Xeption exception)
        {
            var postValidationException =
                new PostValidationException(exception);

            this.loggingBroker.LogError(postValidationException);

            return postValidationException;
        }

    }
}
