using System.Net;

using Commerce.Features.Identity.Application;

using FluentAssertions;

using Newtonsoft.Json;


namespace Commerce.Tests.Integration.Features.Identity.Endpoints;

[TestFixture]
public class IdentityEndpointsTests : IntegrationTest
{
    [Test]
    public async Task GetUsers_WithNoUsers_ShouldReturnEmptyList()
    {
        // Arrange
        var requestUrl = $"/api/user";

        // Act
        var response = await _testClient.GetAsync(requestUrl);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = JsonConvert.DeserializeObject<GetPage.Response>(responseContent);
        result.Should().NotBeNull();
        result!.Users.Should().BeEmpty("No test users yet");
    }
}