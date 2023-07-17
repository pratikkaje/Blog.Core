using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Brokers.DateTimes;
using Blog.Core.Brokers.Loggings;
using Blog.Core.Brokers.Storages;
using Blog.Core.Models.Profiles;
using Blog.Core.Services.Foundations.Profiles;
using Moq;
using Tynamix.ObjectFiller;

namespace Blog.Core.Tests.Unit.Services.Foundations.Profiles
{
    public partial class ProfileServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IProfileService profileService;

        public ProfileServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.profileService = new ProfileService(
                storageBroker: this.storageBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object);
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Profile CreateRandomProfile(DateTimeOffset dates) =>
            CreateProfileFiller(dates: dates).Create();

        private static Profile CreateRandomProfile() =>
            CreateProfileFiller(dates: GetRandomDateTimeOffset()).Create();

        private static Filler<Profile> CreateProfileFiller(DateTimeOffset dates)
        {
            var filler = new Filler<Profile>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }


    }
}
