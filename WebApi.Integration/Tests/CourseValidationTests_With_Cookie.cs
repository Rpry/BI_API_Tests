using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebApi.Integration.Services;
using WebApi.Models;
using Xunit;

namespace WebApi.Integration.Tests
{
	public class CourseValidationTests_With_Cookie : IClassFixture<TestFixture>
	{
		private readonly string _cookie;
		private readonly CourseService _courseService = new();

		public CourseValidationTests_With_Cookie(TestFixture testFixture)
		{
			_cookie = testFixture.AuthCookie;
		}

		[Fact]
		public async Task IfPriceIsZero_PostCourseShouldReturnError()
		{
			//Arrange 
			var courseModel = new AddCourseModel()
			{
				Name = Guid.NewGuid().ToString(),
				Price = 10
			};

			//Act			
			var results = await _courseService.AddCourseWithResultAsync(courseModel, _cookie);

			//Assert
			Assert.False(results.Item1);
			Assert.Equal(results.Item3, Errors.Поле_Price_должно_быть_больше_нуля);
		}

		[Fact]
		public async Task IfNameIsEmpty_PostCourseShouldReturnError()
		{
			//Arrange 
			var courseModel = new AddCourseModel()
			{
				Name = null,
				Price = 10
			};

			//Act			
			var results = await _courseService.AddCourseWithResultAsync(courseModel, _cookie);

			//Assert
			Assert.False(results.Item1);
			Assert.Equal(results.Item3, Errors.Поле_Name_не_должно_быть_пустым);
		}


		[Fact]
		public async Task IfInitialParametersAreSetCorrectly_PostCourseShouldCreateCourseSuccessfully()
		{
			//Arrange 
			var courseModel = new AddCourseModel()
			{
				Name = Guid.NewGuid().ToString(),
				Price = 10
			};

			//Act			
			var results = await _courseService.AddCourseWithResultAsync(courseModel, _cookie);

			//Assert
			Assert.True(results.Item1);
			Assert.InRange(results.Item2, 0, int.MaxValue);
		}
	}
}
