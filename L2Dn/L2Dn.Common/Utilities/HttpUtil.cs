using System.Security.Authentication;

namespace L2Dn.Utilities;

public static class HttpUtil
{
    public static async Task<byte[]> DownloadFileAsync(string url, int maxAttempts)
    {
        using HttpClientHandler clientHandler = new();
        clientHandler.SslProtocols = SslProtocols.Tls13;
        using HttpClient client = new(clientHandler);

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
        }

        throw new HttpRequestException("Could not download file"); // must never happen, the exception is thrown above
    }
}