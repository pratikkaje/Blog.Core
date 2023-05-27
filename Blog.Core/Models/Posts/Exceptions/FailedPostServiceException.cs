using System;
using Xeptions;

namespace Blog.Core.Models.Posts.Exceptions
{
    public class FailedPostServiceException : Xeption
    {
        public FailedPostServiceException(Exception innerException) :
            base(message: "Failed post service occurred, please contact support.", innerException)
        { }
    }
}
