using System;
using Xeptions;

namespace Blog.Core.Models.Posts.Exceptions
{
    public class LockedPostException : Xeption
    {
        public LockedPostException(Exception innerException) :
            base(message: "Locked post record exception, please try again later.", innerException)
        { }
    }
}
