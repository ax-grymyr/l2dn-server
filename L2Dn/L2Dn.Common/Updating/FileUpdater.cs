using System.Security.Cryptography;
using System.Text;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.Updating;

public static class FileUpdater
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(FileUpdater));

    public static async Task UpdateFilesAsync(string fileListUrl, string path, string description)
    {
        try
        {
            await UpdateFilesPrivateAsync(fileListUrl, path, description);
        }
        catch (Exception exception)
        {
            _logger.Warn($"Failed to update files in {path}: " + exception.Message);
        }
    }

    private static async Task UpdateFilesPrivateAsync(string fileListUrl, string path, string description)
    {
        _logger.Info($"Downloading {description} file list {fileListUrl} ...");

        byte[] fileListBytes;
        try
        {
            fileListBytes = await HttpUtil.DownloadFileAsync(fileListUrl, 5);
        }
        catch (Exception exception)
        {
            _logger.Warn($"Error downloading {description} file {fileListUrl}: {exception}");
            return;
        }

        using MemoryStream memoryStream = new(fileListBytes);
        FileList fileList = JsonUtil.DeserializeStream<FileList>(memoryStream);

        string baseUrl = fileList.BaseUrl;
        if (!baseUrl.EndsWith('/'))
            baseUrl += '/';

        bool updated = false;
        bool error = false;
        foreach (FileListFile fileListFile in fileList.Files)
        {
            string destFilePath = Path.Combine(path, fileListFile.Name);
            string? hash = CalculateHash(destFilePath);
            if (string.Equals(hash, fileListFile.Hash))
                continue;

            string url = baseUrl + fileListFile.Name;
            _logger.Info($"Downloading {description} file {url} ...");

            byte[] data;
            try
            {
                data = await HttpUtil.DownloadFileAsync(url, 5).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _logger.Warn($"Error downloading {description} file {url}: {exception}");
                error = true;
                continue;
            }

            try
            {
                if (destFilePath.EndsWith(".gz"))
                {
                    string fn = destFilePath[..^3];
                    if (File.Exists(fn))
                        File.Delete(fn);
                }

                await File.WriteAllBytesAsync(destFilePath, data).ConfigureAwait(false);
                updated = true;
            }
            catch (Exception exception)
            {
                _logger.Warn($"Error saving {description} file {destFilePath}: {exception.Message}");
                error = true;
            }
        }

        if (!updated)
        {
            if (!error)
                _logger.Info($"{description} is up to date.");
        }
        else
        {
            _logger.Info($"{description} has been updated.");
        }
    }

    private static string? CalculateHash(string filePath)
    {
        if (!File.Exists(filePath))
            return null;

        using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using SHA256 sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(fileStream);
        return string.Concat(hash.Select(x => x.ToString("X2")));
    }
}