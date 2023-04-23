using System;
using System.Data;
using System.Reflection.Metadata;
using Blog.Core.Models.Posts;
using Blog.Core.Models.Posts.Exceptions;

namespace Blog.Core.Services.Foundations.Posts
{
    public partial class PostService
    {

        public void ValidatePost(Post post)
        {
            ValidatePostIsNotNull(post);

            Validate(
                (Rule:IsInvalid(post.Id),Parameter:nameof(post.Id)),
                (Rule:IsInvalid(post.Title),Parameter:nameof(post.Title)),
                (Rule: IsInvalid(post.BriefDescription), Parameter: nameof(post.BriefDescription)),
                (Rule: IsInvalid(post.Content), Parameter: nameof(post.Content)),
                (Rule: IsInvalid(post.Author), Parameter: nameof(post.Author)),
                (Rule: IsInvalid(post.CreatedDate), Parameter: nameof(post.CreatedDate)),
                (Rule: IsInvalid(post.UpdatedDate), Parameter: nameof(post.UpdatedDate)));
        }

        private static void ValidatePostIsNotNull(Post post)
        {
            if (post == null)
            {
                throw new NullPostException();
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required."
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required."
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required."
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidPostExeption = new InvalidPostException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidPostExeption.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidPostExeption.ThrowIfContainsErrors();
        }
    }
}
