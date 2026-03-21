using CryptoPuzzles.Client.Services;
using CryptoPuzzles.Client.Services.ApiService.Base;
using CryptoPuzzles.Shared;
using System.Net.Http;
using System.Text.Json;

public class SessionProgressApiService : BaseEntityApiService<ASessionProgress, ASessionProgressCreate, ASessionProgressUpdate>
{
    public SessionProgressApiService(HttpClient httpClient, UserSessionService userSessionService)
        : base(httpClient, userSessionService, "api/sessionprogress") { }

    public async Task<List<ASessionProgress>> GetAllAsync(int? userId = null, int? sessionId = null, bool? solved = null)
    {
        var query = _endpoint;
        var parameters = new List<string>();

        if (userId.HasValue)
            parameters.Add($"userId={userId.Value}");
        if (sessionId.HasValue)
            parameters.Add($"sessionId={sessionId.Value}");
        if (solved.HasValue)
            parameters.Add($"solved={solved.Value}");

        if (parameters.Count != 0)
            query += "?" + string.Join("&", parameters);

        var request = new HttpRequestMessage(HttpMethod.Get, query);
        return await SendAsync<List<ASessionProgress>>(request);
    }

    public async Task<List<ASessionProgress>> GetBySessionIdAsync(int sessionId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_endpoint}?sessionId={sessionId}");
        return await SendAsync<List<ASessionProgress>>(request);
    }

    public async Task<ASessionProgress> CreateAsync(ASessionProgressCreate dto)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _endpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json")
        };
        return await SendAsync<ASessionProgress>(request);
    }

    public async Task UpdateAsync(int id, ASessionProgressUpdate dto)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_endpoint}/{id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json")
        };
        await SendAsync<object>(request);
    }

    public async Task DeleteAsync(int id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_endpoint}/{id}");
        await SendAsync<object>(request);
    }
}