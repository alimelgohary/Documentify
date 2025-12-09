using Documentify.ApplicationCore.Features.Categories.Add;
using System.Net.Http.Json;

namespace Documentify.IntegrationTests.MiddlewaresTests
{
    public class IdempotencyTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        public IdempotencyTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }
        [Fact]
        public async Task Post_Same_Request_Twice_Returns_Same_Response()
        {
            string path = "/categories";
            string name = "Idempotent Category";
            AddCategoryCommand newCategory = new(name);

            var request1 = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = JsonContent.Create(newCategory)
            };
            request1.Headers.Add("Idempotency-Key", "unique-key-123");
            var response1 = await _client.SendAsync(request1);
            var responseContent1 = await response1.Content.ReadAsStringAsync();

            var request2 = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = JsonContent.Create(newCategory)
            };
            request2.Headers.Add("Idempotency-Key", "unique-key-123");
            var response2 = await _client.SendAsync(request2);
            var responseContent2 = await response2.Content.ReadAsStringAsync();
            Assert.Equal(response1.StatusCode, response2.StatusCode);
            Assert.Equal(responseContent1, responseContent2);
        }
    }
}
