using Documentify.ApplicationCore.Features.Auth.Login;
using Documentify.ApplicationCore.Features.Auth.RefreshToken;
using Documentify.ApplicationCore.Features.Auth.Register;
using Documentify.ApplicationCore.Features.Categories.Add;
using Documentify.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using static Documentify.Infrastructure.InfrastructureServiceExtensions;


namespace Documentify.IntegrationTests.ControllersTests
{
    public class AccountControllerTests : IClassFixture<NoDbWebApplicationFactory<Program>>
    {
        private const string testUsername = "testuser";
        private const string testEmail = "user@gmail.com";
        private const string testPassword = "Test@1234";

        private const string path = "/Account";
        private const string pathRegister = path + "/Register";
        private const string pathLogin = path + "/Login";
        private const string pathRefresh = path + "/RefreshToken";

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
            var result = await _client.PostAsJsonAsync<RegisterCommand>(pathRegister, new
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
            var registerResult = await _client.PostAsJsonAsync<RegisterCommand>(pathRegister, new
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

            var loginResult = await _client.PostAsJsonAsync<LoginCommand>(pathLogin, new
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
            var result = await _client.PostAsJsonAsync<RegisterCommand>(pathRegister, new
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
            string res = await RegisterConfirmLoginUserAsync(testUsername3, testEmail3);

            AddCategoryCommand newCategory = new("New category");

            var request = new HttpRequestMessage(HttpMethod.Post, "/categories")
            {
                Content = JsonContent.Create(newCategory)
            };
            string? accessToken;
            accessToken = JsonDocument.Parse(res).RootElement.GetProperty(nameof(accessToken)).GetString();
            if (accessToken is null)
                Assert.Fail($"Login endpoint did not return access token (Looked for key: {nameof(accessToken)})");
            request.Headers.Add("Authorization", "Bearer " + accessToken);
            var postResult = await _client.SendAsync(request);
            if (!postResult.IsSuccessStatusCode)
                Assert.Fail($"Authorized request failed: {postResult.StatusCode}, {await postResult.Content.ReadAsStringAsync()}");
        }

        private async Task<string> RegisterConfirmLoginUserAsync(string testUsername, string testEmail)
        {
            var registerResult = await _client.PostAsJsonAsync<RegisterCommand>(pathRegister, new
                (
                    Username: testUsername,
                    Email: testEmail,
                    Password: testPassword
                ));

            if (!registerResult.IsSuccessStatusCode)
                throw new Exception($"User registration failed during test setup. {registerResult.StatusCode}, {await registerResult.Content.ReadAsStringAsync()}");

            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByEmailAsync(testEmail);
            string token = await userManager.GenerateEmailConfirmationTokenAsync(user!);
            var result = await userManager.ConfirmEmailAsync(user!, token);
            if (!result.Succeeded)
                throw new Exception("Email confirmation failed during login test setup." + string.Join(",", result.Errors.Select(x => x.Description)));

            var loginResult = await _client.PostAsJsonAsync<LoginCommand>(pathLogin, new
            (
                UsernameOrEmail: testUsername,
                Password: testPassword
            ));
            string res = await loginResult.Content.ReadAsStringAsync();
            if (loginResult.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Login failed during test setup, statusCode: {loginResult.StatusCode}, {res}");
            return res;
        }

        [Fact]
        public async Task Calling_Refresh_Token_Returns_New_Token()
        {
            string testUsername4 = "testuser4";
            string testEmail4 = "user4@gmail.com";
            string res = await RegisterConfirmLoginUserAsync(testUsername4, testEmail4);

            string? refreshToken;
            refreshToken = JsonDocument.Parse(res).RootElement.GetProperty(nameof(refreshToken)).GetString();
            if (refreshToken is null)
                Assert.Fail($"Login endpoint did not return a refresh token (Looked for key: {nameof(refreshToken)}");

            var httpResult = await _client.PostAsJsonAsync<RefreshTokenCommand>(pathRefresh, new(refreshToken));
            var stringResult = await httpResult.Content.ReadAsStringAsync();

            if (!httpResult.IsSuccessStatusCode)
                Assert.Fail($"Endpoint response not success: {httpResult.StatusCode}, result: {stringResult}");

            var objectResult = await httpResult.Content.ReadFromJsonAsync<RefreshTokenResponse>();
            if (objectResult is null)
                Assert.Fail($"Couldn't deserialize response object, result: {stringResult}");

            if (string.IsNullOrWhiteSpace(objectResult.AccessToken))
                Assert.Fail($"No access token: {stringResult}");

            if (string.IsNullOrWhiteSpace(objectResult.RefreshToken))
                Assert.Fail($"No refresh token: {stringResult}");

            if (objectResult.AccessTokenExpiry == default)
                Assert.Fail($"No access token expiry: {stringResult}");

            if (objectResult.RefreshTokenExpiry == default)
                Assert.Fail($"No refresh token expiry: {stringResult}");
        }

        [Fact]
        public async Task Calling_Refresh_Token_With_Revoked_Token()
        {
            string testUsername5 = "testuser5";
            string testEmail5 = "user5@gmail.com";
            string res = await RegisterConfirmLoginUserAsync(testUsername5, testEmail5);

            string? refreshToken;
            refreshToken = JsonDocument.Parse(res).RootElement.GetProperty(nameof(refreshToken)).GetString();
            if (refreshToken is null)
                Assert.Fail($"Login endpoint did not return a refresh token (Looked for key: {nameof(refreshToken)}");

            var httpResult = await _client.PostAsJsonAsync<RefreshTokenCommand>(pathRefresh, new(refreshToken));
            var httpResult2 = await _client.PostAsJsonAsync<RefreshTokenCommand>(pathRefresh, new(refreshToken));

            if (httpResult2.IsSuccessStatusCode)
                Assert.Fail($"Refresh token should be revoked after use, {httpResult2.StatusCode}, {await httpResult2.Content.ReadAsStringAsync()}");
        }

        [Fact]
        public async Task Calling_Refresh_Token_With_Wrong_Signature_Fails()
        {
            string testEmail4 = "user4@gmail.com";
            Guid userId = Guid.NewGuid();
            var wrongSecret = new StringBuilder(32);
            for (int i = 0; i < 32; i++)
            {
                wrongSecret.Append((char)Random.Shared.Next(65, 90));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(wrongSecret.ToString()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, testEmail4)
            };
            var scope = _factory.Services.CreateScope();
            var _conf = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            string refreshToken = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                claims: claims,
                signingCredentials: creds,
                expires: DateTime.UtcNow.AddDays(1),
                issuer: _conf[ConfigurationKeys.JwtIssuer]
            ));

            var httpResult = await _client.PostAsJsonAsync<RefreshTokenCommand>(pathRefresh, new(refreshToken));
            var stringResult = await httpResult.Content.ReadAsStringAsync();

            if (httpResult.IsSuccessStatusCode)
                Assert.Fail($"Request should fails because of wrong secret signing key, {httpResult.StatusCode}, {stringResult}");
        }
        [Fact]
        public async Task Calling_Refresh_Token_With_Wrong_Issuer_Fails()
        {
            string testEmail4 = "user4@gmail.com";
            Guid userId = Guid.NewGuid();
            var scope = _factory.Services.CreateScope();
            var _conf = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf[ConfigurationKeys.JwtRefreshSecret]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, testEmail4)
            };
            string refreshToken = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                claims: claims,
                signingCredentials: creds,
                expires: DateTime.UtcNow.AddDays(1),
                issuer: "WrongIssuer"
            ));

            var httpResult = await _client.PostAsJsonAsync<RefreshTokenCommand>(pathRefresh, new(refreshToken));
            var stringResult = await httpResult.Content.ReadAsStringAsync();

            if (httpResult.IsSuccessStatusCode)
                Assert.Fail($"Request should fails because of wrong issuer, {httpResult.StatusCode}, {stringResult}");
        }

        [Fact]
        public async Task Calling_Refresh_Token_With_Expired_Token_Fails()
        {
            string testEmail4 = "user4@gmail.com";
            Guid userId = Guid.NewGuid();
            var scope = _factory.Services.CreateScope();
            var _conf = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf[ConfigurationKeys.JwtRefreshSecret]!));
           
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, testEmail4)
            };
            string refreshToken = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                claims: claims,
                signingCredentials: creds,
                expires: DateTime.UtcNow.AddMinutes(-1),
                issuer: _conf[ConfigurationKeys.JwtIssuer]
            ));

            var httpResult = await _client.PostAsJsonAsync<RefreshTokenCommand>(pathRefresh, new(refreshToken));
            var stringResult = await httpResult.Content.ReadAsStringAsync();

            if (httpResult.IsSuccessStatusCode)
                Assert.Fail($"Request should fails because of expired token, {httpResult.StatusCode}, {stringResult}");
        }

    }
}
