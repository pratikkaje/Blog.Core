using System;
using Xeptions;

namespace Blog.Core.Models.Posts.Exceptions
{
    public class NotFoundPostException : Xeption
    {
        public NotFoundPostException(Guid postId) : base(message: $"Couldn't find post with id: {postId}")
        { }
    }
}
