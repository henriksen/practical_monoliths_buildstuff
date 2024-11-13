using System.Net;
using System.Text;

using Commerce.Features.Customer.Application;

using FluentAssertions;

using Newtonsoft.Json;

namespace Commerce.Tests.Integration.Features.Customers.Endpoints;

[TestFixture]
public class CustomerEndpointsTests : IntegrationTest
{
    [Test]
    public async Task CreateCustomer_ShouldReturnCreatedCustomer()
    {
        // Arrange
        var requestUrl = "/api/Customer";
        var newCustomer = new { Name = "John Doe", Email = "john.doe@example.com" };
        var content = new StringContent(JsonConvert.SerializeObject(newCustomer), Encoding.UTF8, "application/json");

        // Act
        var response = await _testClient.PostAsync(requestUrl, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = JsonConvert.DeserializeObject<Create.Response>(responseContent);
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
    }

    [Test]
    public async Task GetCustomer_ShouldReturnCustomer()
    {
        // Arrange
        var customerId = await CreateTestCustomerAsync();
        var requestUrl = $"/api/Customer/{customerId}";

        // Act
        var response = await _testClient.GetAsync(requestUrl);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = JsonConvert.DeserializeObject<Get.Response>(responseContent);
        result.Should().NotBeNull();
        result!.Name.Should().NotBeEmpty("Test Customer");
        result!.Email.Should().NotBeEmpty("test@example.com");
    }

    private async Task<string> CreateTestCustomerAsync()
    {
        var requestUrl = "/api/Customer";
        var newCustomer = new { Name = "Test Customer", Email = "test@example.com" };
        var content = new StringContent(JsonConvert.SerializeObject(newCustomer), Encoding.UTF8, "application/json");

        var response = await _testClient.PostAsync(requestUrl, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<Create.Response>(responseContent);
        return result!.Id.ToString();
    }
}