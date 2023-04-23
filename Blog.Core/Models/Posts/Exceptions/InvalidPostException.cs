using Xeptions;

namespace Blog.Core.Models.Posts.Exceptions
{
    public class InvalidPostException : Xeption
    {
        public InvalidPostException()
            : base(message: "Invalid property. Please correct the errors and try again.")
        { }
    }
}
