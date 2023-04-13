using Xeptions;

namespace Blog.Core.Models.Posts.Exceptions
{
    public class PostValidationException : Xeption
    {
        public PostValidationException(Xeption innerException) : base(message: "Post validation error occurred, please try again.", innerException)
        {

        }
    }
}
