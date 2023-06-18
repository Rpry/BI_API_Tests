using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;
using Xunit;

namespace WebApi.Integration.Tests
{
	//Helper methods
	public partial class CourseValidationTests_With_Cookie : IClassFixture<TestFixture>
	{
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
				result.AddRange(getResults.Item2.Where(t => !t.Deleted));
			}
			return result;
		}

		async Task<List<CourseModel[]>> GetCoursesPaged(int itemsPerPage)
		{
			List<CourseModel[]> result = new();
			int page = 1;
			while (true)
			{
				var getResults = await _courseService.GetCourseListWithResultAsync(page++, itemsPerPage, _cookie);
				if (getResults.Item1 == false || getResults.Item2.Count == 0)
					break;
				result.Add(getResults.Item2.Where(t => !t.Deleted).ToArray());
			}
			return result;
		}

		public class PageEqualityComparer : IEqualityComparer<CourseModel[]>
		{
			public bool Equals(CourseModel[] x, CourseModel[] y)
			{
				if (x.Length != y.Length)
					return false;

				for (int i = 0; i < x.Length; i++)
				{
					if (x[i].Name != y[i].Name || x[i].Price != y[i].Price)
						return false;
				}

				return true;
			}

			public int GetHashCode([DisallowNull] CourseModel[] obj)
			{
				throw new NotImplementedException();
			}
		}
	}
}
