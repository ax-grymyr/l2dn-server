using System.Security.Authentication;

namespace L2Dn.Utilities;

public static class HttpUtil
{
    public static async Task<byte[]> DownloadFileAsync(string url, int maxAttempts)
    {
        using HttpClientHandler clientHandler = new();
        clientHandler.SslProtocols = SslProtocols.Tls13;
        using HttpClient client = new(clientHandler);
        client.DefaultRequestHeaders.UserAgent.TryParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36");

        int attempt = 0;
        while (attempt < maxAttempts)
        {
            attempt++;
            try
            {
                return await client.GetByteArrayAsync(url).ConfigureAwait(false);
            }
            catch
            {
                if (attempt >= maxAttempts)
                    throw;
            }

            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
        }

        throw new HttpRequestException("Could not download file"); // must never happen, the exception is thrown above
    }
}