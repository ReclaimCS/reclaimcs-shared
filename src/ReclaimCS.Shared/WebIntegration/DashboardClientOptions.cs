namespace ReclaimCS.Shared.WebIntegration;

public sealed class DashboardClientOptions
{
    public bool Enabled { get; set; }
    public string ApiBaseUrl { get; set; } = "";
    public string ServerApiKey { get; set; } = "";
    public string ApiKeyHeaderName { get; set; } = "x-reclaimcs-server-key";
    public double TimeoutSeconds { get; set; } = 2.5;

    public bool HasCredentials =>
        Enabled
        && !string.IsNullOrWhiteSpace(ApiBaseUrl)
        && !string.IsNullOrWhiteSpace(ServerApiKey);
}
