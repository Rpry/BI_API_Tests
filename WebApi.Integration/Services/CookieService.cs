using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApi.Integration.Services;

public class CookieService
{
    private CookieAuthApiClient _cookieAuthApiClient;

    public CookieService()
    {
        _cookieAuthApiClient = new CookieAuthApiClient();
    }
    
    public async Task<string> GetAdminCookieAsync()
    {
        return await GetCookieAsync("admin", "admin");
    }
    
    public async Task<string> GetCookieAsync(string name, string password)
    {
        var response = await GetCookieInternalAsync(name, password);
        return response.Headers.FirstOrDefault(h=> h.Key == "Set-Cookie").Value.ToList().First();
    }
     
	public async Task<(bool, string)> GetCookieWithResultAsync(string name, string password)
	{
        string cookie = null;
        var response = await GetCookieInternalAsync(name, password);
        if (response.IsSuccessStatusCode)
        {
            var firstHeader = response.Headers.FirstOrDefault(h => h.Key == "Set-Cookie").Value;
			cookie = firstHeader?.First();
		}
        return (cookie != null, cookie);
	}

	public async Task<HttpResponseMessage> GetCookieInternalAsync(string name, string password)
    {
        return await _cookieAuthApiClient.GetAuthCookieAsync(name, password);
    }
}
