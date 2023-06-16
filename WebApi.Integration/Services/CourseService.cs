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

	public async Task<(bool, CourseModel, string)> GetCourseWithResultAsync(int courseId, string cookie = null)
	{
		bool ok = false;
		string errMessage = "";
		CourseModel result = null;
		string getCourseResponse = null;

		try
		{
			getCourseResponse = await GetCourseInternalAsync(courseId, cookie);
		}
		catch (HttpRequestException e)
		{
			errMessage = e.Message;
		}

		if (getCourseResponse != null)
		{
			result = JsonConvert.DeserializeObject<CourseModel>(getCourseResponse);
			ok = true;
		}

		return (ok, result, errMessage);
	}

	public async Task<(bool, string)> EditCourseWithResultAsync(int id, AddCourseModel courseModel, string cookie = null)
	{
		string errMessage = "";

		var addCourseResponse = await EditCourseInternalAsync(id, courseModel, cookie);
		bool ok = addCourseResponse.IsSuccessStatusCode;
		if (!ok)
			errMessage = await addCourseResponse.Content.ReadAsStringAsync();

		return (ok, errMessage);
	}

	public async Task<(bool, string)> DeleteCourseWithResultAsync(int id, string cookie = null)
	{
		string errMessage = "";

		var response = await DeleteCourseInternalAsync(id, cookie);
		bool ok = response.IsSuccessStatusCode;
		if (!ok)
			errMessage = await response.Content.ReadAsStringAsync();

		return (ok, errMessage);
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

	public async Task<string> GetCourseInternalAsync(int courseId, string cookie = null)
	{
		return await _applicationHttpClient.GetCourseAsync(courseId, cookie);
	}

	public async Task<HttpResponseMessage> EditCourseInternalAsync(int id, AddCourseModel courseModel, string cookie = null)
	{
		return await _applicationHttpClient.EditCourseAsync(id, courseModel, cookie);
	}

	public async Task<HttpResponseMessage> DeleteCourseInternalAsync(int id, string cookie = null)
	{
		return await _applicationHttpClient.DeleteCourseAsync(id, cookie);
	}
}
