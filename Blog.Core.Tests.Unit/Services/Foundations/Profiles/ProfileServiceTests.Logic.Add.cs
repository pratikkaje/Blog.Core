using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Models.Profiles;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace Blog.Core.Tests.Unit.Services.Foundations.Profiles
{
    public partial class ProfileServiceTests
    {
        [Fact]
        public async Task ShouldAddProfileAsync()
        {
            // given
            var randomProfile = CreateRandomProfile();
            var inputProfile = randomProfile;
            var insertedProfile = inputProfile;
            var expectedProfile = insertedProfile.DeepClone();

            this.storageBrokerMock.Setup(broker => 
                broker.InsertProfileAsync(inputProfile))
                    .ReturnsAsync(insertedProfile);
            // when
            var actualProfile = 
                await this.profileService.AddProfileAsync(inputProfile); 

            // then
            actualProfile.Should().BeEquivalentTo(expectedProfile);

            this.storageBrokerMock.Verify(broker => 
                broker.InsertProfileAsync(It.IsAny<Profile>()),
                Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
