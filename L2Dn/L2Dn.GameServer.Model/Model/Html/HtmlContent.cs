using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Html;

public sealed class HtmlContent
{
    public const string MissingHtml = "<html><body>Html is missing</body></html>";  
    private static readonly Logger _logger = LogManager.GetLogger(nameof(HtmlContent));
    private readonly string? _filePath;
    private readonly HtmlActionValidator? _validator;
    private string _html;
    private bool _disableValidation;
    private bool _fileLoaded;

    private HtmlContent(string? filePath, string html, HtmlActionValidator? validator, bool fileLoaded)
    {
        _filePath = filePath;
        _html = html;
        _validator = validator;
        _fileLoaded = fileLoaded;
    }

    public bool FileLoaded => _fileLoaded;
    
    public void DisableValidation()
    {
        _disableValidation = true;
    }
    
    public void Replace(string pattern, string? value)
    {
        string val = value?.replaceAll(@"\$", @"\\\$") ?? string.Empty; 
        _html = _html.replaceAll(pattern, val);
    }

    public void Replace<T>(string pattern, T value)
    {
        Replace(pattern, value?.ToString() ?? string.Empty);
    }

    public string BuildHtml(HtmlActionScope scope, int? originObjectId = null)
    {
        string html = _html;
        if (html.Length > 17200)
        {
            _logger.Warn($"Html is too long {_html.Length}! this will crash the client! Path: {_filePath}");
            
            html = html.Substring(0, 17200);
        }
        
        if (_validator != null)
        {
            _validator.ClearActions(scope);
            
            if (!_disableValidation)
                _validator.BuildActions(scope, html, originObjectId);
        }

        return html;
    }
    
    /// <summary>
    /// Loads HTML from data pack.
    /// </summary>
    /// <param name="path">Relative path.</param>
    /// <param name="player"></param>
    /// <returns></returns>
    public static HtmlContent LoadFromFile(string path, Player? player)
    {
        string? html = HtmCache.getInstance().getHtm(path, player?.getLang());
        bool fileLoaded = true;
        if (string.IsNullOrEmpty(html))
        {
            html = $"<html><body>Html file is missing:<br/>{path}</body></html>";
            _logger.Warn("Missing html page: " + path);
            fileLoaded = false;
        }

        if (Config.GM_DEBUG_HTML_PATHS && player != null && player.isGM())
            BuilderUtil.sendHtmlMessage(player, path);
        
        return new HtmlContent(path, WrapHtml(html), player?.getClient()?.HtmlActionValidator, fileLoaded);
    }

    /// <summary>
    /// Loads HTML from data pack.
    /// </summary>
    /// <param name="html">Html page.</param>
    /// <param name="player"></param>
    /// <returns></returns>
    public static HtmlContent LoadFromText(string html, Player? player)
    {
        return new HtmlContent(null, WrapHtml(html), player?.getClient()?.HtmlActionValidator, false);
    }
    
    private static string WrapHtml(string html)
    {
        if (!html.Contains("<html") && !html.StartsWith("..\\L2"))
            return "<html><body>" + html + "</body></html>";

        return html;
    }
}