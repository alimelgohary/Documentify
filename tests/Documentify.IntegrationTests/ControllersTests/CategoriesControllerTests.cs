using Documentify.ApplicationCore.Features;
using Documentify.ApplicationCore.Features.Categories;
using Documentify.ApplicationCore.Features.Categories.Add;
using Documentify.ApplicationCore.Features.Categories.GetAll;
using Documentify.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Documentify.IntegrationTests.ControllersTests
{
    public class CategoriesControllerTests : IClassFixture<NoAuthNoDbWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly NoAuthNoDbWebApplicationFactory<Program> _factory;
        private readonly string path = "/categories";
        public CategoriesControllerTests(NoAuthNoDbWebApplicationFactory<Program> factory)
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
            var res = await getResponse.Content.ReadFromJsonAsync<Result<CategoryDto>>();

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.NotNull(res);
            Assert.Equal(id, res.Data!.Id);
            Assert.Equal(name, res.Data.Name);
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
            var createdResponse = await response.Content.ReadFromJsonAsync<Result<AddCategoryResponse>>();
            if (createdResponse is null)
            {
                string res = await response.Content.ReadAsStringAsync();
                throw new Exception($"Problem in PostCategory method, deserialize failed: {res}");
            }
            return createdResponse!.Data!.categoryId;
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

            var response = await _client.GetFromJsonAsync<Result<GetAllCategoriesResponse>>(path);
            Assert.NotNull(response);
            Assert.NotEmpty(response.Data!.items);
            Assert.Equal(response.Data.count, response.Data.items.Count);
            Assert.Contains(response.Data.items, c => c.Name == name);
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
                var createdResponse = await response.Content.ReadFromJsonAsync<Result<AddCategoryResponse>>();

                Assert.NotNull(createdResponse!.Data);
                Assert.NotEqual(Guid.Empty, createdResponse!.Data.categoryId);

                using var scope = _factory.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                Assert.True(db.Categories.Any(p => p.Id == createdResponse.Data.categoryId && p.Name == name));
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
