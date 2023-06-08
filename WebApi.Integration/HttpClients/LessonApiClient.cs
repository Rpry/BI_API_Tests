using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WebApi.Models;

namespace WebApi.Integration.Services;

public class LessonApiClient
{
    private HttpClient _httpClient;
    private readonly string _baseUri;

    public LessonApiClient()
    {
        _httpClient = new HttpClient();
        var configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json").Build();
        _baseUri = configuration["BaseUri"];
    }
    
    public async Task<HttpResponseMessage> GetLessonAsync(int id, string token = null)
    {
        if (token != null)
        {
            AddToken(token);
        }
        return await _httpClient.GetAsync($"{_baseUri}/lesson/{id}");
    }
    
    public async Task<HttpResponseMessage> AddLessonAsync(LessonModel lessonModel, string token = null)
    {
        if (token != null)
        {
            AddToken(token);
        }
        return await _httpClient.PostAsJsonAsync($"{_baseUri}/lesson", lessonModel);
    }
    
    public async Task<HttpResponseMessage> EditLessonAsync(int id, LessonModel lessonModel, string token = null)
    {
        if (token != null)
        {
            AddToken(token);
        }
        return await _httpClient.PutAsJsonAsync($"{_baseUri}/lesson/{id}", lessonModel);
    }
    
    private void AddToken(string token)
    {
        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");    
        }
    }
}