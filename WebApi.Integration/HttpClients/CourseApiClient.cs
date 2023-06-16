using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WebApi.Models;

namespace WebApi.Integration.Services;

public class CourseApiClient
{
	private HttpClient _httpClient;
	private readonly string _baseUri;

	public CourseApiClient()
	{
		_httpClient = new HttpClient();
		var configuration = new ConfigurationBuilder()
			.AddJsonFile($"appsettings.json").Build();
		_baseUri = configuration["BaseUri"];
	}

	public async Task<HttpResponseMessage> CreateCourseAsync(AddCourseModel course, string cookie = null)
	{
		if (cookie != null)
		{
			AddAuthCookie(cookie);
		}
		return await _httpClient.PostAsJsonAsync($"{_baseUri}/course", course);
	}

	public async Task<string> GetCourseAsync(int courseId, string cookie = null)
	{
		if (cookie != null)
		{
			AddAuthCookie(cookie);
		}
		return await _httpClient.GetStringAsync($"{_baseUri}/course/{courseId}");
	}

	public async Task<HttpResponseMessage> EditCourseAsync(int id, AddCourseModel courseModel, string cookie = null)
	{
		if (cookie != null)
		{
			AddAuthCookie(cookie);
		}
		return await _httpClient.PutAsJsonAsync($"{_baseUri}/course/{id}", courseModel);
	}

	public async Task<HttpResponseMessage> DeleteCourseAsync(int id, string cookie = null)
	{
		if (cookie != null)
		{
			AddAuthCookie(cookie);
		}
		return await _httpClient.DeleteAsync($"{_baseUri}/course/{id}");
	}

	private void AddAuthCookie(string cookie)
	{
		_httpClient.DefaultRequestHeaders.Add("cookie", cookie);
	}
}
