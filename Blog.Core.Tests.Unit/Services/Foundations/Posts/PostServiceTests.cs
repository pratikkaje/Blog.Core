using System;
using System.Linq.Expressions;
using Blog.Core.Brokers.DateTimes;
using Blog.Core.Brokers.Loggings;
using Blog.Core.Brokers.Storages;
using Blog.Core.Models.Posts;
using Blog.Core.Services.Foundations.Posts;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace Blog.Core.Tests.Unit.Services.Foundations.Posts
{
    public partial class PostServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly IPostService postService;

        public PostServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();

            this.postService = new PostService(storageBroker: this.storageBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object);
        }

        private static Post CreateRandomPost() =>
            CreatePostFiller(dates: GetRandomDateTimeOffset()).Create();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Filler<Post> CreatePostFiller(DateTimeOffset dates)
        {
            var filler = new Filler<Post>();
            filler.Setup()
                   .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }

        private static Expression<Func<Exception, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);
    }
}