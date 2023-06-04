using System;
using System.Reflection.Metadata;
using Blog.Core.Models.Posts;
using Blog.Core.Models.Posts.Exceptions;

namespace Blog.Core.Services.Foundations.Posts
{
    public partial class PostService
    {
        public void ValidatePostOnAdd(Post post)
        {
            ValidatePostIsNotNull(post);

            Validate(
                (Rule: IsInvalid(post.Id), Parameter: nameof(post.Id)),
                (Rule: IsInvalid(post.Title), Parameter: nameof(post.Title)),
                (Rule: IsInvalid(post.SubTitle), Parameter: nameof(post.SubTitle)),
                (Rule: IsInvalid(post.Content), Parameter: nameof(post.Content)),
                (Rule: IsInvalid(post.Author), Parameter: nameof(post.Author)),
                (Rule: IsInvalid(post.CreatedDate), Parameter: nameof(post.CreatedDate)),
                (Rule: IsInvalid(post.UpdatedDate), Parameter: nameof(post.UpdatedDate)),

                (Rule: IsNotSame(firstDate: post.UpdatedDate,
                secondDate: post.CreatedDate,
                secondDateName: nameof(post.CreatedDate)),
                Parameter: nameof(post.UpdatedDate)),

                (Rule: IsNotRecent(post.CreatedDate), Parameter: nameof(Post.CreatedDate))
                );
        }

        public void ValidatePostId(Guid postId) =>
            Validate((Rule: IsInvalid(postId), Parameter: nameof(Post.Id)));

        public void ValidateStoragePost(Post maybePost, Guid postId)
        {
            if(maybePost is null)
            {
                throw new NotFoundPostException(postId);
            }
        }

        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent."
        };

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime =
                this.dateTimeBroker.GetCurrentDateTimeOffset();

            TimeSpan timeDifference = currentDateTime.Subtract(date);
            TimeSpan oneMinute = TimeSpan.FromMinutes(1);

            return timeDifference.Duration() > oneMinute;
        }

        private static dynamic IsNotSame(DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not same as the {secondDateName}"
            };

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
