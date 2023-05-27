using System;
using Xeptions;

namespace Blog.Core.Models.Posts.Exceptions
{
    public class AlreadyExistsPostException : Xeption
    {
        public AlreadyExistsPostException(Exception innerException) :
            base(message: "Post with same id already exists. ", innerException)
        { }
    }
}
