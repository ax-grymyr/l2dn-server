namespace L2Dn.Updating;

public class FileList
{
    public string BaseUrl { get; set; } = string.Empty;
    public List<FileListFile> Files { get; set; } = [];
}