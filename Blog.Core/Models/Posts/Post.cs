using System;

namespace Blog.Core.Models.Posts
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string BriefDescription { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}