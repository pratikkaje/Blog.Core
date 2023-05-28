using Xeptions;

namespace Blog.Core.Models.Posts.Exceptions
{
    public class PostDependencyException : Xeption
    {
        public PostDependencyException(Xeption innerException)
            : base(message: "Post dependency erro occurred, contact support.", innerException)
        { }
    }
}
