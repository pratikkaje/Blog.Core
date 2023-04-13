using Xeptions;

namespace Blog.Core.Models.Posts.Exceptions
{
    public class NullPostException : Xeption
    {
        public NullPostException() : base(message: "Post is null.")
        {

        }
    }
}
