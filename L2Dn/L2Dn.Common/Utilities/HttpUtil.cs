namespace L2Dn.Utilities;

public static class HttpUtil
{
    public static async Task<byte[]> DownloadFileAsync(string url)
    {
        using HttpClientHandler clientHandler = new();
        using HttpClient client = new(clientHandler);
        return await client.GetByteArrayAsync(url).ConfigureAwait(false);
    }
}