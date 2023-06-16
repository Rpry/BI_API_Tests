using FluentAssertions;
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

		[Fact]
		public async Task IfInitialParametersAreSetCorrectly_PutCourseShouldUpdateCourseCorrectly()
		{
			//Arrange
			string courseName = Guid.NewGuid().ToString();
			int coursePrice = 10;
			var courseModel = new AddCourseModel()
			{
				Name = courseName,
				Price = coursePrice
			};

			var addResults = await _courseService.AddCourseWithResultAsync(courseModel, _cookie);
			Assert.True(addResults.Item1);
			int courseId = addResults.Item2;

			//Act
			var updatedCourseModel = new AddCourseModel()
			{
				Name = "updated " + courseModel.Name,
				Price = 100 + courseModel.Price
			};
			var putResults = await _courseService.EditCourseWithResultAsync(courseId, updatedCourseModel, _cookie);
			var getResults = await _courseService.GetCourseWithResultAsync(courseId, _cookie);
			Assert.True(putResults.Item1);
			Assert.True(getResults.Item1);

			//Assert
			var updatedCourseFromServer = getResults.Item2;
			Assert.True(updatedCourseFromServer.Price == updatedCourseModel.Price);
			Assert.Equal(updatedCourseFromServer.Name, updatedCourseModel.Name);
		}

		[Fact]
		public async Task IfInitialParametersAreSetCorrectly_DeleteCourseShouldSetDeletedProperty()
		{
			//Arrange
			string courseName = Guid.NewGuid().ToString();
			int coursePrice = 10;
			var courseModel = new AddCourseModel()
			{
				Name = courseName,
				Price = coursePrice
			};

			//Act
			var addResults = await _courseService.AddCourseWithResultAsync(courseModel, _cookie);
			int courseId = addResults.Item2;
			var getResults = await _courseService.GetCourseWithResultAsync(courseId, _cookie);
			Assert.True(addResults.Item1);
			Assert.True(getResults.Item1);
			Assert.False(getResults.Item2.Deleted);

			var delResults = await _courseService.DeleteCourseWithResultAsync(courseId, _cookie);
			getResults = await _courseService.GetCourseWithResultAsync(courseId, _cookie);
			Assert.True(delResults.Item1);
			Assert.True(getResults.Item1);

			//Assert
			Assert.True(getResults.Item2.Deleted);
		}
	}
}
