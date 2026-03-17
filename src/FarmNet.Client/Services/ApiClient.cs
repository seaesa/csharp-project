using System.Net.Http.Headers;
using System.Net.Http.Json;
using FarmNet.Client.Auth;

namespace FarmNet.Client.Services;

public class ApiClient(HttpClient http, JwtAuthStateProvider authState)
{
    public async Task SetAuthHeaderAsync()
    {
        var token = await authState.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        await SetAuthHeaderAsync();
        var response = await http.GetAsync(url);
        if (!response.IsSuccessStatusCode) return default;
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
    {
        await SetAuthHeaderAsync();
        return await http.PostAsJsonAsync(url, data);
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string url, T data)
    {
        await SetAuthHeaderAsync();
        return await http.PutAsJsonAsync(url, data);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        await SetAuthHeaderAsync();
        return await http.DeleteAsync(url);
    }
}
