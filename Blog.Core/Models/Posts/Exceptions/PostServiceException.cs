using Xeptions;

namespace Blog.Core.Models.Posts.Exceptions
{
    public class PostServiceException : Xeption
    {
        public PostServiceException(Xeption innerException) :
            base(message: "Post service error occurred, contact support.", innerException)
        { }
    }
}
