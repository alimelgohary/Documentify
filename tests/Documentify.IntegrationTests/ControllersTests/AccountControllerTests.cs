using Documentify.ApplicationCore.Features.Auth.Login;
using Documentify.ApplicationCore.Features.Auth.Register;
using Documentify.ApplicationCore.Features.Categories.Add;
using Documentify.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Documentify.IntegrationTests.ControllersTests
{
    public class AccountControllerTests : IClassFixture<NoDbWebApplicationFactory<Program>>
    {
        private const string testUsername = "testuser";
        private const string testEmail = "user@gmail.com";
        private const string testPassword = "Test@1234";


        private readonly NoDbWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        public AccountControllerTests(NoDbWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Register_New_User_Returns_Success()
        {
            var result = await _client.PostAsJsonAsync<RegisterCommand>("/account/register", new
            (
                Username: testUsername,
                Email: testEmail,
                Password: testPassword
            ));
            result.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Login_Existing_User_Returns_Success()
        {
            string testUsername2 = "testuser2";
            string testEmail2 = "user2@gmail.com";
            var registerResult = await _client.PostAsJsonAsync<RegisterCommand>("/account/register", new
                (
                    Username: testUsername2,
                    Email: testEmail2,
                    Password: testPassword
                ));

            if (!registerResult.IsSuccessStatusCode)
                Assert.Fail($"User registration failed during login test setup. {registerResult.StatusCode}, {await registerResult.Content.ReadAsStringAsync()}");

            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByEmailAsync(testEmail2);
            string token = await userManager.GenerateEmailConfirmationTokenAsync(user!);
            var result = await userManager.ConfirmEmailAsync(user!, token);
            if (!result.Succeeded)
                Assert.Fail("Email confirmation failed during login test setup." + string.Join(",", result.Errors.Select(x => x.Description)));

            var loginResult = await _client.PostAsJsonAsync<LoginCommand>("/account/login", new
            (
                UsernameOrEmail: testUsername2,
                Password: testPassword
            ));
            string res = await loginResult.Content.ReadAsStringAsync();
            if (loginResult.StatusCode != HttpStatusCode.OK)
                Assert.Fail($"Test failed, statusCode: {loginResult.StatusCode}, {res}");
        }


        [Theory]
        [InlineData("", "", "")]
        [InlineData("dsld", testEmail, testPassword)] // username too short
        [InlineData("test-user#2123", testEmail, testPassword)] // invalid chars in username
        [InlineData(testUsername, "email@domain.com", testPassword)] // invalid domain
        [InlineData(testUsername, "emaildomain.com", testPassword)] // invalid email format
        [InlineData(testUsername, testEmail, "ABCDEFG")] // 7char
        [InlineData(testUsername, testEmail, "ABCDEFGH")] // no lowercase, no digit
        [InlineData(testUsername, testEmail, "AaBCDEFG")] // no digit
        [InlineData(testUsername, testEmail, "ACBCDEFG1")] // no lowercase
        [InlineData(testUsername, testEmail, "abcdefgh1")] // no uppercase
        public async Task Register_Using_Invalid_Data_Returns_BadRequest(string username, string email, string password)
        {
            var result = await _client.PostAsJsonAsync<RegisterCommand>("/account/register", new
            (
                Username: username,
                Email: email,
                Password: password
            ));
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Calling_Action_With_Authorize_Returns_Unauthorized()
        {
            var result = await _client.PostAsJsonAsync<AddCategoryCommand>("/categories", new
            (
                categoryName: "Sample Category"
            ));
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task Calling_Authorized_Action_With_Authorize_Header_Doesnt_Return_Unauthorized()
        {
            string testUsername3 = "testuser3";
            string testEmail3 = "user3@gmail.com";

            var registerResult = await _client.PostAsJsonAsync<RegisterCommand>("/account/register", new
                (
                    Username: testUsername3,
                    Email: testEmail3,
                    Password: testPassword
                ));

            if (!registerResult.IsSuccessStatusCode)
                Assert.Fail($"User registration failed during test setup. {registerResult.StatusCode}, {await registerResult.Content.ReadAsStringAsync()}");

            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByEmailAsync(testEmail3);
            string token = await userManager.GenerateEmailConfirmationTokenAsync(user!);
            var result = await userManager.ConfirmEmailAsync(user!, token);
            if (!result.Succeeded)
                Assert.Fail("Email confirmation failed during login test setup." + string.Join(",", result.Errors.Select(x => x.Description)));

            var loginResult = await _client.PostAsJsonAsync<LoginCommand>("/account/login", new
            (
                UsernameOrEmail: testUsername3,
                Password: testPassword
            ));
            string res = await loginResult.Content.ReadAsStringAsync();
            if (loginResult.StatusCode != HttpStatusCode.OK)
                Assert.Fail($"Login failed during test setup, statusCode: {loginResult.StatusCode}, {res}");

            AddCategoryCommand newCategory = new("New category");

            var request = new HttpRequestMessage(HttpMethod.Post, "/categories")
            {
                Content = JsonContent.Create(newCategory)
            };

            request.Headers.Add("Authorization", "Bearer " + JsonDocument.Parse(res).RootElement.GetProperty("token").GetString());
            var postResult = await _client.SendAsync(request);
            if (!postResult.IsSuccessStatusCode)
                Assert.Fail($"Authorized request failed: {postResult.StatusCode}, {await postResult.Content.ReadAsStringAsync()}");
        }
    }
}
