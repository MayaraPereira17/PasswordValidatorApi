using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using PasswordValidatorApi.Models;
using Xunit;

namespace PasswordValidatorApi.Tests
{
    public class PasswordValidatorIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public PasswordValidatorIntegrationTests(WebApplicationFactory<Program> factory) => _factory = factory;

        [Fact]
        public async Task ValidPassword_Returns200AndTrue()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("validate-password", new PasswordRequest { Password = "AbTp9!fok" });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<bool>();
            Assert.True(result);
        }

        [Fact]
        public async Task InvalidPassword_WithWhitespace_Returns422AndFalse()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("validate-password", new PasswordRequest { Password = "AbTp9!fo k" });

            Assert.Equal((HttpStatusCode)422, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<bool>();
            Assert.False(result);
        }

        [Fact]
        public async Task EmptyPassword_Returns422AndFalse()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("validate-password", new PasswordRequest { Password = "" });

            Assert.Equal((HttpStatusCode)422, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<bool>();
            Assert.False(result);
        }

        [Fact]
        public async Task MissingPasswordPropertyReturns400BadRequest()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsync("validate-password", JsonContent.Create(new { }));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PasswordTooShort_Returns422AndFalse()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("validate-password", new PasswordRequest { Password = "Ab1!" });

            Assert.Equal((HttpStatusCode)422, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<bool>();
            Assert.False(result);
        }

        [Fact]
        public async Task PasswordWithoutUppercase_Returns422AndFalse()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("validate-password", new PasswordRequest { Password = "abtp9!fok" });

            Assert.Equal((HttpStatusCode)422, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<bool>();
            Assert.False(result);
        }

        [Fact]
        public async Task PasswordWithoutLowercase_Returns422AndFalse()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("validate-password", new PasswordRequest { Password = "ABTP9!FOK" });

            Assert.Equal((HttpStatusCode)422, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<bool>();
            Assert.False(result);
        }

        [Fact]
        public async Task PasswordWithoutDigit_Returns422AndFalse()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("validate-password", new PasswordRequest { Password = "AbTpX!fok" });

            Assert.Equal((HttpStatusCode)422, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<bool>();
            Assert.False(result);
        }

        [Fact]
        public async Task PasswordWithoutSpecialChar_Returns422AndFalse()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("validate-password", new PasswordRequest { Password = "AbTp9fokX" });

            Assert.Equal((HttpStatusCode)422, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<bool>();
            Assert.False(result);
        }

        [Fact]
        public async Task PasswordWithRepeatedCharacters_Returns422AndFalse()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("validate-password", new PasswordRequest { Password = "AbTp9!foA" });

            Assert.Equal((HttpStatusCode)422, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<bool>();
            Assert.False(result);
        }
    }
}