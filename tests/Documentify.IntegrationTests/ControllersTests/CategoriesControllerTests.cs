using Documentify.ApplicationCore.Features.Categories.GetAll;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace Documentify.IntegrationTests.ControllersTests
{
    public class CategoriesControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CategoriesControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_Categories_Returns_OK()
        {
            var response = await _client.GetAsync("/categories");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Get_Categories_Returns_Items()
        {
            var response = await _client.GetFromJsonAsync<GetAllCategoriesResponse>("/categories");
            Assert.NotNull(response);
            Assert.NotEmpty(response.items);
            Assert.Equal(response.count, response.items.Count);
        }
    }

}
