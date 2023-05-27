using System;
using Xeptions;

namespace Blog.Core.Models.Posts.Exceptions
{
    public class PostServiceException : Xeption
    {
        public PostServiceException(Exception innerException) :
            base(message: "Post service error occurred, contact support.", innerException)
        { }
    }
}
