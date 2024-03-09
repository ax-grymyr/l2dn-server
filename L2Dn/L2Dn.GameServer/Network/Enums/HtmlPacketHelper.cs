using System.Text;
using L2Dn.GameServer.Data;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network.Enums;

public class HtmlPacketHelper
{
    public const string MissingHtml = "<html><body>Html is missing</body></html>";  
    private static readonly Logger _logger = LogManager.GetLogger(nameof(HtmlPacketHelper));
    private string _html = MissingHtml;

    public HtmlPacketHelper()
    {
    }

    public HtmlPacketHelper(string html)
    {
        SetHtml(html);
    }

    public HtmlPacketHelper(DataFileLocation location, string relativeFilePath)
    {
        string filePath = DataReaderBase.GetFullPath(location, relativeFilePath);
        if (File.Exists(filePath))
            SetHtml(File.ReadAllText(filePath, Encoding.UTF8));
        else
            SetHtml($"<html><body>Html file is missing:<br/>{filePath.Replace("\\", "/")}</body></html>");
    }
    
    public void SetHtml(string html)
    {
        // TODO: html cache?
        
        if (!html.Contains("<html") && !html.StartsWith("..\\L2"))
            html = "<html><body>" + html + "</body></html>";

        if (html.Length > 17200)
        {
            _logger.Error($"Html is too long {html.Length}! this will crash the client!");
            html = html.Substring(0, 17200);
        }

        _html = html;
    }

    public void WriteHtml(PacketBitWriter writer)
    {
        writer.WriteString(_html);
    }

    public void Replace(string oldValue, string newValue)
    {
        SetHtml(_html.Replace(oldValue, newValue));
    }

    public string getHtml()
    {
        return _html;
    }
}