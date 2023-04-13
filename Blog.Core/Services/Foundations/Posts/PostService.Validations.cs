using Blog.Core.Models.Posts;
using Blog.Core.Models.Posts.Exceptions;

namespace Blog.Core.Services.Foundations.Posts
{
    public partial class PostService
    {
        public void ValidatePost(Post post)
        {
            ValidatePostIsNotNull(post);
        }

        private static void ValidatePostIsNotNull(Post post)
        {
            if (post == null)
            {
                throw new NullPostException();
            }
        }
    }
}
