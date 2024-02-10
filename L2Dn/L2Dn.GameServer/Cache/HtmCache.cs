using System.Text;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Cache;

/**
 * @author Layane
 * @author Zoey76
 */
public class HtmCache
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(HtmCache));
	
	private static readonly Map<String, String> HTML_CACHE = Config.HTM_CACHE ? new() : new(); // concurrent if false
	
	private int _loadedFiles;
	private long _bytesBuffLen;
	
	protected HtmCache()
	{
		reload();
	}
	
	public void reload()
	{
		reload(Config.DATAPACK_ROOT_PATH);
	}
	
	public void reload(string directoryPath)
	{
		if (Config.HTM_CACHE)
		{
			LOGGER.Info("Html cache start...");
			parseDir(directoryPath);
			LOGGER.Info("Cache[HTML]: " + getMemoryUsage() + " megabytes on " + _loadedFiles + " files loaded.");
		}
		else
		{
			HTML_CACHE.clear();
			_loadedFiles = 0;
			_bytesBuffLen = 0;
			LOGGER.Info("Cache[HTML]: Running lazy cache.");
		}
	}
	
	public void reloadPath(string dirPath)
	{
		parseDir(dirPath);
		LOGGER.Info("Cache[HTML]: Reloaded specified path.");
	}
	
	public double getMemoryUsage()
	{
		return (float) _bytesBuffLen / 1048576;
	}
	
	public int getLoadedFiles()
	{
		return _loadedFiles;
	}
	
	private void parseDir(string directoryPath)
	{
		Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories).forEach(x => loadFile(x));
	}
	
	public String loadFile(string filePath)
	{
		string extension = Path.GetExtension(filePath); 
		if (!string.Equals(extension, ".htm", StringComparison.OrdinalIgnoreCase) && 
		    !string.Equals(extension, ".html", StringComparison.OrdinalIgnoreCase))
		{
			return null;
		}
		
		String content = null;
		try
		{
			byte[] raw = File.ReadAllBytes(filePath);
			content = Encoding.UTF8.GetString(raw);
			content = content.replaceAll("(?s)<!--.*?-->", ""); // Remove html comments.
			content = content.replaceAll("[\\t\\n]", ""); // Remove tabs and new lines.
			
			filePath = Path.GetRelativePath(Config.DATAPACK_ROOT_PATH, filePath);
			if (Config.CHECK_HTML_ENCODING && !filePath.startsWith("data/lang") &&
			    content.All(c => c <= 127))
			{
				LOGGER.Warn("HTML encoding check: File " + filePath + " contains non ASCII content.");
			}
			
			String oldContent = HTML_CACHE.put(filePath, content);
			if (oldContent == null)
			{
				_bytesBuffLen += raw.Length;
				_loadedFiles++;
			}
			else
			{
				_bytesBuffLen = (_bytesBuffLen - oldContent.Length) + raw.Length;
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Problem with htm file:", e);
		}
		return content;
	}
	
	public String getHtm(Player player, String path)
	{
		String prefix = player != null ? player.getHtmlPrefix() : "";
		String newPath = prefix + path;
		String content = HTML_CACHE.get(newPath);
		if (!Config.HTM_CACHE && (content == null))
		{
			content = loadFile(Path.Combine(Config.DATAPACK_ROOT_PATH, newPath));
			if (content == null)
			{
				content = loadFile(Path.Combine(Config.SCRIPT_ROOT_PATH, newPath));
			}
		}
		
		// In case localisation does not exist try the default path.
		if ((content == null) && !string.IsNullOrEmpty(prefix))
		{
			content = HTML_CACHE.get(path);
			newPath = path;
		}
		
		if ((player != null) && player.isGM() && Config.GM_DEBUG_HTML_PATHS)
		{
			BuilderUtil.sendHtmlMessage(player, newPath.Substring(5));
		}
		
		return content;
	}
	
	public bool contains(String path)
	{
		return HTML_CACHE.containsKey(path);
	}
	
	/**
	 * @param path The path to the HTM
	 * @return {@code true} if the path targets a HTM or HTML file, {@code false} otherwise.
	 */
	public bool isLoadable(string path)
	{
		string extension = Path.GetExtension(path);
		return string.Equals(extension, ".htm", StringComparison.OrdinalIgnoreCase) ||
		       string.Equals(extension, ".html", StringComparison.OrdinalIgnoreCase);
	}
	
	public static HtmCache getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly HtmCache INSTANCE = new HtmCache();
	}
}
