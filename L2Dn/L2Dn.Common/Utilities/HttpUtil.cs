namespace L2Dn.Utilities;

public static class HttpUtil
{
    public static byte[] DownloadFile(string url)
    {
        return DownloadFileAsync(url).GetAwaiter().GetResult();
    } 

    public static async Task<byte[]> DownloadFileAsync(string url)
    {
        using HttpClient client = new();
        return await client.GetByteArrayAsync(url).ConfigureAwait(false);
    } 
}