using System.Threading.Tasks;
using Blog.Core.Tests.Acceptance.Brokers;
using FluentAssertions;
using Xunit;

namespace Blog.Core.Tests.Acceptance.Apis.Homes
{
    [Collection(nameof(ApiTestCollection))]
    public class HomeApiTests
    {
        private readonly ApiBroker apiBroker;

        public HomeApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        [Fact]
        public async Task ShouldReturnHomeMessageAsync()
        {
            // given
            string expectedMessage = "Hello World!";

            // when
            string actualMessage =
                await this.apiBroker.GetHomeMessageAsync();

            // then
            expectedMessage.Should().BeEquivalentTo(actualMessage);
        }
    }
}