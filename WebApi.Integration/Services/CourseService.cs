using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Newtonsoft.Json;
using WebApi.Models;

namespace WebApi.Integration.Services;

public class CourseService
{
	private CourseApiClient _applicationHttpClient;
	public CourseService()
	{
		_applicationHttpClient = new CourseApiClient();
	}


	public async Task<int> CreateRandomCourseAsync(string cookie = null)
	{
		var autoFixture = new Fixture();

		#region FSetup

		autoFixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => autoFixture.Behaviors.Remove(b));
		autoFixture.Behaviors.Add(new OmitOnRecursionBehavior());

		#endregion

		var courseModel = autoFixture.Create<AddCourseModel>();
		return await AddCourseAsync(courseModel, cookie);
	}

	public async Task<(bool, int, string)> AddCourseWithResultAsync(AddCourseModel courseModel, string cookie = null)
	{
		bool ok;
		int courseId = -1;
		string errMessage = "";

		var addCourseResponse = await AddCourseInternalAsync(courseModel, cookie);
		ok = addCourseResponse.IsSuccessStatusCode;
		var responseContent = await addCourseResponse.Content.ReadAsStringAsync();

		if (ok)
			courseId = JsonConvert.DeserializeObject<int>(responseContent);
		else
			errMessage = responseContent;
		return (ok, courseId, errMessage);
	}

	public async Task<int> AddCourseAsync(AddCourseModel courseModel, string cookie = null)
	{
		var addCourseResponse = await AddCourseInternalAsync(courseModel, cookie);
		return JsonConvert.DeserializeObject<int>(await addCourseResponse.Content.ReadAsStringAsync());
	}

	public async Task<HttpResponseMessage> AddCourseInternalAsync(AddCourseModel courseModel, string cookie = null)
	{
		return await _applicationHttpClient.CreateCourseAsync(courseModel, cookie);
	}
}