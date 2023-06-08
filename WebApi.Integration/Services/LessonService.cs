using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebApi.Models;

namespace WebApi.Integration.Services;

public class LessonService
{
    private LessonApiClient _applicationHttpClient;
    public LessonService()
    {
        _applicationHttpClient = new LessonApiClient();
    }

    public async Task<LessonModel> GetLessonAsync(int id, string token = null)
    {
        var response = await GetLessonInternalAsync(id, token);
        return JsonConvert.DeserializeObject<LessonModel>(await response.Content.ReadAsStringAsync());
    }
    
    public async Task<int> AddLessonAsync(LessonModel lessonModel, string token = null)
    {
        var response = await AddLessonInternalAsync(lessonModel, token);
        return JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
    }
    
    public async Task<HttpResponseMessage> GetLessonInternalAsync(int id, string token = null)
    {
        return await _applicationHttpClient.GetLessonAsync(id, token);
    }
    
    public async Task<HttpResponseMessage> AddLessonInternalAsync(LessonModel lessonModel, string token = null)
    {
        return await _applicationHttpClient.AddLessonAsync(lessonModel, token);
    }
    
    public async Task<HttpResponseMessage> EditLessonInternalAsync(int id, LessonModel lessonModel, string token = null)
    {
        return await _applicationHttpClient.EditLessonAsync(id, lessonModel, token);
    }
}