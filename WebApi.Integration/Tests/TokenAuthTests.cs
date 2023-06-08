using System;
using System.Net;
using System.Threading.Tasks;
using Demo.Authentication.Dto;
using Newtonsoft.Json;
using WebApi.Integration.Services;
using Xunit;

namespace WebApi.Integration.Tests
{
    public class TokenAuthTests
    {
        private readonly TokenService _tokenService;

        public TokenAuthTests()
        {
            _tokenService = new TokenService();
        }
        
        [Fact]
        public async Task IfLoginAndPasswordAreCorrect_TokenShouldBeReceived()
        {
            //Arrange
            string name = "admin";
            string password = "admin";

            //Act
            var httpResponseMessage = await _tokenService.GetTokenInternalAsync(name, password);
            
            //Assert
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            var responseMessage = await httpResponseMessage.Content.ReadAsStringAsync();
            var tokenDto = JsonConvert.DeserializeObject<TokenResultDto>(responseMessage);
            Assert.NotNull(tokenDto);
            Assert.NotNull(tokenDto.IdToken);
        }
        
        [Fact]
        public async Task IfPasswordIsIncorrect_TokenShouldNotBeReceived()
        {
            //Arrange
            string name = "admin";
            string password = Guid.NewGuid().ToString();

            //Act
            var httpResponseMessage = await _tokenService.GetTokenInternalAsync(name, password);
            
            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
            var responseMessage = await httpResponseMessage.Content.ReadAsStringAsync();
            Assert.Equal("Некорректные логин/пароль", responseMessage);
        }
    }
}