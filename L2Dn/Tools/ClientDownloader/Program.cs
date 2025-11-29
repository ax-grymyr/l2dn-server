using System.Net;

const string targetDirectory = @"L2TorrentFiles";

const string baseUrl = "http://akumu.ru/lineage2/";

using HttpClientHandler handler = new();
handler.UseCookies = true;
handler.AllowAutoRedirect = true;
handler.AutomaticDecompression = DecompressionMethods.All;
handler.CookieContainer = new CookieContainer();
handler.MaxAutomaticRedirections = 5;

using HttpClient client = new(handler);

Queue<string> urlQueue = new();
urlQueue.Enqueue(baseUrl);

while (urlQueue.Count != 0)
{
    string url = urlQueue.Dequeue();
    HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
    //response. // TODO: not implemented
}

Directory.CreateDirectory(targetDirectory);