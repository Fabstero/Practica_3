using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Practica_2.Application.Dtos;

namespace Practica_2.Api;

public abstract class ApiClientBase
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    protected ApiClientBase(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    protected HttpClient HttpClient { get; }

    protected async Task<T> GetRequiredAsync<T>(string requestUri, CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("La API devolvió una respuesta vacía.");
    }

    protected async Task<T?> GetOptionalAsync<T>(string requestUri, CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.GetAsync(requestUri, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
    }

    protected async Task<Respuesta<T>> SendAsync<T>(
        HttpMethod method,
        string requestUri,
        object? body = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(method, requestUri);
        if (body != null)
        {
            request.Content = JsonContent.Create(body, options: JsonOptions);
        }

        using var response = await HttpClient.SendAsync(request, cancellationToken);
        var respuesta = await response.Content.ReadFromJsonAsync<Respuesta<T>>(JsonOptions, cancellationToken);

        if (respuesta != null)
        {
            return respuesta;
        }

        return new Respuesta<T>
        {
            EsCorrecto = response.IsSuccessStatusCode,
            Codigo = (int)response.StatusCode,
            Mensaje = response.ReasonPhrase ?? "La API no devolvió una respuesta válida."
        };
    }
}
