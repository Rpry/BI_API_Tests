using System;
using System.Threading.Tasks;
using WebApi.Integration.Services;
using Xunit;

namespace WebApi.Integration.Tests
{
	public class CookieAuthTest
	{
		private readonly CookieService _cookieService = new();

		[Fact]
		public async Task IfLoginAndPasswordAreCorrect_CookieShouldBeReceived()
		{
			//Arrange
			string name = "admin";
			string password = "admin";

			//Act				
			var results = await _cookieService.GetCookieWithResultAsync(name, password);

			//Assert
			Assert.True(results.Item1);
			var cookie = results.Item2;
			Assert.NotNull(cookie);
		}

		[Fact]
		public async Task IfPasswordIsIncorrect_CookieShouldNotBeReceived()
		{
			//Arrange
			string name = "admin";
			string password = Guid.NewGuid().ToString();

			//Act
			var results = await _cookieService.GetCookieWithResultAsync(name, password);

			//Assert
			Assert.False(results.Item1);
			var cookie = results.Item2;
			Assert.Null(cookie);
		}
	}
}
