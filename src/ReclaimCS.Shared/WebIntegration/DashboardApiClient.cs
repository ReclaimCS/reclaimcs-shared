using System.Net.Http.Json;

namespace ReclaimCS.Shared.WebIntegration;

public sealed class DashboardApiClient : IDisposable
{
    private readonly DashboardClientOptions _options;
    private readonly HttpClient _httpClient;

    public DashboardApiClient(DashboardClientOptions options)
    {
        _options = options;
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(Math.Clamp(options.TimeoutSeconds, 1.0, 30.0))
        };
    }

    public async Task<T> GetJsonAsync<T>(string relativePath, CancellationToken cancellationToken = default)
    {
        using var response = await SendAsync(HttpMethod.Get, relativePath, content: null, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken)
            ?? throw new InvalidOperationException($"Empty dashboard response for GET {relativePath}.");
    }

    public async Task PostJsonAsync(string relativePath, object payload, CancellationToken cancellationToken = default)
    {
        using var response = await SendAsync(HttpMethod.Post, relativePath, JsonContent.Create(payload), cancellationToken);
    }

    public async Task<T> PostJsonAsync<T>(string relativePath, object payload, CancellationToken cancellationToken = default)
    {
        using var response = await SendAsync(HttpMethod.Post, relativePath, JsonContent.Create(payload), cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken)
            ?? throw new InvalidOperationException($"Empty dashboard response for POST {relativePath}.");
    }

    private async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string relativePath,
        HttpContent? content,
        CancellationToken cancellationToken)
    {
        if (!_options.HasCredentials)
            throw new InvalidOperationException("Dashboard client is not configured.");

        using var request = new HttpRequestMessage(method, BuildUri(relativePath));
        request.Content = content;
        request.Headers.TryAddWithoutValidation(_options.ApiKeyHeaderName, _options.ServerApiKey);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode)
            return response;

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        response.Dispose();
        throw new HttpRequestException(
            $"Dashboard request {method} {relativePath} failed with {(int)response.StatusCode}: {body}",
            null,
            response.StatusCode);
    }

    private Uri BuildUri(string relativePath)
    {
        var baseUrl = _options.ApiBaseUrl.TrimEnd('/') + "/";
        var path = relativePath.TrimStart('/');
        return new Uri(new Uri(baseUrl, UriKind.Absolute), path);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
