using System.Collections.Concurrent;
using System.Text;
using L2Dn.Extensions;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Cache;

public sealed class HtmCache
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(HtmCache));
	private static readonly ConcurrentDictionary<string, string> _cache = new(StringComparer.OrdinalIgnoreCase);

	private HtmCache()
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
			_logger.Info("Html cache start...");
			ParseDir(directoryPath);
			_logger.Info("Cache[HTML]: " + (getMemoryUsage() >> 20) + " megabytes on " + _cache.Count + " files loaded.");
		}
		else
		{
			_cache.Clear();
			_logger.Info("Cache[HTML]: Running lazy cache.");
		}
	}

	public void reloadPath(string dirPath)
	{
		ParseDir(dirPath);
		_logger.Info("Cache[HTML]: Reloaded specified path.");
	}

	public long getMemoryUsage()
	{
		return _cache.Sum(x => (long)x.Value.Length);
	}

	public int getLoadedFiles()
	{
		return _cache.Count;
	}

	private void ParseDir(string directoryPath)
	{
		Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories).ForEach(x => loadFile(x));
	}

	public string? loadFile(string filePath)
	{
		string extension = Path.GetExtension(filePath);
		if (!string.Equals(extension, ".htm", StringComparison.OrdinalIgnoreCase) &&
		    !string.Equals(extension, ".html", StringComparison.OrdinalIgnoreCase))
		{
			return null;
		}

		string? content = null;
		try
		{
			content = File.ReadAllText(filePath, Encoding.UTF8);
			content = content.replaceAll("(?s)<!--.*?-->", ""); // Remove html comments.
			content = content.replaceAll(@"[\t\n]", ""); // Remove tabs and new lines.

			// Automatic removal of -h parameter from specific bypasses.
			if (Config.HIDE_BYPASS_REMOVAL)
			{
				content = content.replaceAll("bypass -h npc_%objectId%_Chat ", "bypass npc_%objectId%_Chat ");
				content = content.replaceAll("bypass -h npc_%objectId%_Quest", "bypass npc_%objectId%_Quest");
			}

			filePath = Path.GetRelativePath(Config.DATAPACK_ROOT_PATH, filePath);
			if (Config.CHECK_HTML_ENCODING && !filePath.startsWith("lang") &&
			    content.Any(c => c >= 128))
			{
				_logger.Warn("HTML encoding check: File " + filePath + " contains non ASCII content.");
			}

			filePath = filePath.Replace("\\", "/");

			_cache[filePath] = content;
		}
		catch (Exception e)
		{
			_logger.Warn("Problem with htm file: " + e);
		}

		return content;
	}

	public string getHtm(string path, string? language = null)
	{
		string prefix = string.Empty;
		if (Config.MULTILANG_ENABLE && !string.IsNullOrEmpty(language))
		{
			string lang = language;
			if (!Config.MULTILANG_ALLOWED.Contains(lang))
				lang = Config.MULTILANG_DEFAULT;

			if (!string.Equals(lang, "en"))
				prefix = "lang/" + lang + "/"; // TODO: cache prefixes
		}

		string newPath = string.IsNullOrEmpty(prefix) ? path : prefix + path;
		if (!_cache.TryGetValue(newPath, out string? content))
		{
			if (!Config.HTM_CACHE)
			{
				content = loadFile(Path.Combine(Config.DATAPACK_ROOT_PATH, newPath));
				if (content == null)
					content = loadFile(Path.Combine(Config.SCRIPT_ROOT_PATH, newPath));
			}

			// In case localisation does not exist try the default path.
			if (string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(prefix))
			{
				if (!_cache.TryGetValue(path, out content))
				{
					if (!Config.HTM_CACHE)
					{
						content = loadFile(Path.Combine(Config.DATAPACK_ROOT_PATH, path));
						if (content == null)
							content = loadFile(Path.Combine(Config.SCRIPT_ROOT_PATH, path));
					}
				}
			}
		}

		return content ?? throw new ArgumentException("HTML resource not found: " + path);
	}

	public bool contains(string path)
	{
		return _cache.ContainsKey(path);
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