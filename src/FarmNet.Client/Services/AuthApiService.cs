using System.Net.Http.Json;
using FarmNet.Client.Auth;
using FarmNet.Client.Models;

namespace FarmNet.Client.Services;

public class AuthApiService(HttpClient http, JwtAuthStateProvider authState)
{
    public async Task<(bool Success, string? Error)> LoginAsync(LoginRequest request)
    {
        var response = await http.PostAsJsonAsync("api/auth/login", request);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            return (false, err?.Message ?? "Đăng nhập thất bại");
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (result == null) return (false, "Không nhận được dữ liệu");

        await authState.SetTokenAsync(result.Token);
        return (true, null);
    }

    public async Task LogoutAsync() => await authState.ClearTokenAsync();
}

public record ErrorResponse(string Message);
