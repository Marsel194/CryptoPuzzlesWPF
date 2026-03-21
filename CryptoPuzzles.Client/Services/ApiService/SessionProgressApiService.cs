using CryptoPuzzles.Client.Services.ApiService.Base;
using CryptoPuzzles.Shared;
using System.Net.Http;

public class SessionProgressApiService : BaseEntityApiService<ASessionProgress, ASessionProgressCreate, ASessionProgressUpdate>
{
    public SessionProgressApiService(HttpClient httpClient) : base(httpClient, "api/sessionprogress") { }

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

        return await SendAsync<List<ASessionProgress>>(() => _httpClient.GetAsync(query));
    }

    public async Task<List<ASessionProgress>> GetBySessionIdAsync(int sessionId)
    {
        return await SendAsync<List<ASessionProgress>>(() =>
            _httpClient.GetAsync($"{_endpoint}?sessionId={sessionId}"));
    }
}