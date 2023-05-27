using Xeptions;

namespace Blog.Core.Models.Posts.Exceptions
{
    public class PostDependencyValidationException : Xeption
    {
        public PostDependencyValidationException(Xeption innerException) :
            base(message: "Post dependency validation exception occurred, please try again.", innerException)
        { }
    }
}
