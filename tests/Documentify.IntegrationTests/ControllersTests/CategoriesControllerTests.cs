using Documentify.ApplicationCore.Features.Categories;
using Documentify.ApplicationCore.Features.Categories.Add;
using Documentify.ApplicationCore.Features.Categories.GetAll;
using Documentify.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Documentify.IntegrationTests.ControllersTests
{
    public class CategoriesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly string path = "/categories";
        public CategoriesControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
        }

        [Fact]
        public async Task Get_By_Id_Returns_Category()
        {
            string name = "New Category for testing";
            var id = await PostCategory(name);

            var getResponse = await _client.GetAsync($"{path}/{id}");
            var category = await getResponse.Content.ReadFromJsonAsync<CategoryDto>();

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.NotNull(category);
            Assert.Equal(id, category!.Id);
            Assert.Equal(name, category.Name);
        }

        [Fact]
        public async Task Get_By_Id_Returns_NotFound()
        {
            var response = await _client.GetAsync($"{path}/{Guid.NewGuid()}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        async Task<Guid> PostCategory(string name)
        {
            AddCategoryCommand newCategory = new(name);
            var response = await _client.PostAsJsonAsync(path, newCategory);
            var createdResponse = await response.Content.ReadFromJsonAsync<AddCategoryResponse>();
            return createdResponse!.categoryId;
        }

        [Fact]
        public async Task Get_Categories_Returns_OK()
        {
            var response = await _client.GetAsync(path);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Get_Categories_Returns_Items()
        {
            string name = "New Category for testing";
            await PostCategory(name);

            var response = await _client.GetFromJsonAsync<GetAllCategoriesResponse>(path);
            Assert.NotNull(response);
            Assert.NotEmpty(response.items);
            Assert.Equal(response.count, response.items.Count);
            Assert.Contains(response.items, c => c.Name == name);
        }

        [Fact]
        public async Task Add_Category_Returns_Created()
        {
            string name = "New Category for testing";
            AddCategoryCommand newCategory = new(name);
            var response = await _client.PostAsJsonAsync(path, newCategory);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            Assert.True(db.Categories.Any(p => p.Name == name));
        }

        [Fact]
        public async Task Add_Category_Returns_Guid()
        {
            string name = "New Category for testing 2";
            AddCategoryCommand newCategory = new(name);

            try
            {
                var response = await _client.PostAsJsonAsync(path, newCategory);
                var createdResponse = await response.Content.ReadFromJsonAsync<AddCategoryResponse>();

                Assert.NotNull(createdResponse);
                Assert.NotEqual(Guid.Empty, createdResponse!.categoryId);

                using var scope = _factory.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                Assert.True(db.Categories.Any(p => p.Id == createdResponse.categoryId && p.Name == name));
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception occurred while deserializing response: {ex.Message}");
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("ss")]
        public async Task Add_Category_Invalid_Name_Returns_BadRequest(string? name)
        {
            AddCategoryCommand newCategory = new(name);
            var response = await _client.PostAsJsonAsync(path, newCategory);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

}
