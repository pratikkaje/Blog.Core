﻿using System.Threading.Tasks;
using Blog.Core.Models.Posts;
using Blog.Core.Models.Posts.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Xeptions;

namespace Blog.Core.Services.Foundations.Posts
{
    public partial class PostService
    {

        private delegate ValueTask<Post> ReturningPostFunction();

        private async ValueTask<Post> TryCatch(ReturningPostFunction returningPostFunction)
        {
            try
            {
                return await returningPostFunction();
            }
            catch (NullPostException nullPostException)
            {
                throw CreateAndLogValidationException(nullPostException);
            }
            catch (InvalidPostException invalidPostException)
            {
                throw CreateAndLogValidationException(invalidPostException);
            }
            catch (SqlException sqlException)
            {
                var failedPostStorageException = 
                    new FailedPostStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedPostStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsPostException = 
                    new AlreadyExistsPostException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsPostException);
            }
        }

        private PostDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var postDependencyValdationException = 
                new PostDependencyValidationException(exception);

            this.loggingBroker.LogError(postDependencyValdationException);

            throw postDependencyValdationException;
        }

        private PostDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var postDependencyException = 
                new PostDependencyException(exception);

            this.loggingBroker.LogCritical(postDependencyException);

            throw postDependencyException;
        }

        private PostValidationException CreateAndLogValidationException(Xeption exception)
        {
            var postValidationException =
                new PostValidationException(exception);

            this.loggingBroker.LogError(postValidationException);

            return postValidationException;
        }

    }
}
