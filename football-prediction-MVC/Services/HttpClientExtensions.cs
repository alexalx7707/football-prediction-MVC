using Microsoft.Extensions.Configuration;

namespace football_prediction_MVC.Services;

public static class HttpClientExtensions
{
    public static void ConfigureApiClient(this HttpClient client, IConfiguration configuration)
    {
        var baseUrl = configuration["Api:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException("Api:BaseUrl is not configured.");
        }

        client.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
    }
}
