namespace L2Dn.Utilities;

public static class HttpUtil
{
    public static byte[] DownloadFile(string url)
    {
        return DownloadFileAsync(url).GetAwaiter().GetResult();
    } 

    public static async Task<byte[]> DownloadFileAsync(string url)
    {
        using HttpClientHandler clientHandler = new();
        clientHandler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        using HttpClient client = new(clientHandler);
        return await client.GetByteArrayAsync(url).ConfigureAwait(false);
    } 
}