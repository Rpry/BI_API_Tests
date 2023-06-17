using System;
using System.Collections.Generic;
using System.Linq;
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

		[Theory]
		[InlineData(20, 3)]
		public async Task IfInitialParametersAreSetCorrectly_PaginationShouldSortCoursesById(int testN, int itemsPerPage)
		{
			//Arrange
			var courses = CreateAddCourseModelsNumbered(testN);
			var nameIdIndex = courses.Select(async course =>
				{
					var addResults = await _courseService.AddCourseWithResultAsync(course, _cookie);
					Assert.True(addResults.Item1);
					return new { Id = addResults.Item2, Course = course };
				}
			).Select(c => c.Result).ToDictionary(t => t.Course.Name, t => t.Id);

			//Act
			var allCoursesFromServer = await GetCoursesFromPages(itemsPerPage);
			int previousId = -1;
			foreach (var course in allCoursesFromServer)
			{
				//Assert
				if (!course.Deleted && nameIdIndex.TryGetValue(course.Name, out int id))
				{
					Assert.True(previousId < id);
					previousId = id;
				}
			}

			//Cleanup
			nameIdIndex.ToList().ForEach(async t =>
			{
				var delResults = await _courseService.DeleteCourseWithResultAsync(t.Value, _cookie);
				Assert.True(delResults.Item1);
			});
		}

		List<AddCourseModel> CreateAddCourseModelsNumbered(int n)
		{
			string name = Guid.NewGuid().ToString();
			List<AddCourseModel> courses = new();

			for (int i = 0; i < n; i++)
				courses.Add(new AddCourseModel { Name = name + $"_{i}", Price = 1 });

			return courses;
		}

		async Task<List<CourseModel>> GetCoursesFromPages(int itemsPerPage)
		{
			List<CourseModel> result = new();
			int page = 1;
			while (true)
			{
				var getResults = await _courseService.GetCourseListWithResultAsync(page++, itemsPerPage, _cookie);
				if (getResults.Item1 == false || getResults.Item2.Count == 0)
					break;
				result.AddRange(getResults.Item2);
			}
			return result;
		}
	}
}
