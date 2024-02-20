using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripting;

public class ScriptEngineManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ScriptEngineManager));
	
	public static readonly string SCRIPT_FOLDER = Config.SCRIPT_ROOT_PATH;
	public static readonly string MASTER_HANDLER_FILE = Path.Combine(SCRIPT_FOLDER, "handlers", "MasterHandler.java");
	public static readonly string EFFECT_MASTER_HANDLER_FILE = Path.Combine(SCRIPT_FOLDER, "handlers", "EffectMasterHandler.java");
	public static readonly string SKILL_CONDITION_HANDLER_FILE = Path.Combine(SCRIPT_FOLDER, "handlers", "SkillConditionMasterHandler.java");
	public static readonly string CONDITION_HANDLER_FILE = Path.Combine(SCRIPT_FOLDER, "handlers", "ConditionMasterHandler.java");
	public static readonly string ONE_DAY_REWARD_MASTER_HANDLER = Path.Combine(SCRIPT_FOLDER, "handlers", "DailyMissionMasterHandler.java");
	
	protected static readonly List<String> _exclusions = new();
	
	protected ScriptEngineManager()
	{
		// Load Scripts.xml
		load();
	}
	
	public void load()
	{
		_exclusions.Clear();
		//parseDatapackFile("config/Scripts.xml");
		LOGGER.Info("Loaded " + _exclusions.Count + " files to exclude.");
	}
	//
	// public void parseDocument(Document doc, File f)
	// {
	// 	try
	// 	{
	// 		Map<String, List<String>> excludePaths = new();
	// 		forEach(doc, "list", listNode => forEach(listNode, "exclude", excludeNode =>
	// 		{
	// 			String excludeFile = parseString(excludeNode.getAttributes(), "file");
	// 			excludePaths.putIfAbsent(excludeFile, new());
	// 			
	// 			forEach(excludeNode, "include", includeNode => excludePaths.get(excludeFile).add(parseString(includeNode.getAttributes(), "file")));
	// 		}));
	// 		
	// 		int nameCount = SCRIPT_FOLDER.getNameCount();
	// 		Files.walkFileTree(SCRIPT_FOLDER, new SimpleFileVisitor<Path>()
	// 		{
	// 			public FileVisitResult visitFile(Path file, BasicFileAttributes attrs) throws IOException
	// 			{
	// 				String fileName = file.getFileName().toString();
	// 				if (fileName.endsWith(".java"))
	// 				{
	// 					Iterator<Path> relativePath = file.subpath(nameCount, file.getNameCount()).iterator();
	// 					while (relativePath.hasNext())
	// 					{
	// 						String nextPart = relativePath.next().toString();
	// 						if (excludePaths.containsKey(nextPart))
	// 						{
	// 							bool excludeScript = true;
	// 							
	// 							List<String> includePath = excludePaths.get(nextPart);
	// 							if (includePath != null)
	// 							{
	// 								while (relativePath.hasNext())
	// 								{
	// 									if (includePath.contains(relativePath.next().toString()))
	// 									{
	// 										excludeScript = false;
	// 										break;
	// 									}
	// 								}
	// 							}
	// 							if (excludeScript)
	// 							{
	// 								_exclusions.add(file.toUri().getPath());
	// 								break;
	// 							}
	// 						}
	// 					}
	// 				}
	// 				return base.visitFile(file, attrs);
	// 			}
	// 		});
	// 	}
	// 	catch (IOException e)
	// 	{
	// 		LOGGER.Warn("Couldn't load script exclusions: " + e);
	// 	}
	// }
	
	private void processDirectory(string dir, List<string> files)
	{
		foreach (string filePath in Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories))
		{
			if (filePath.endsWith(".java") && !_exclusions.Contains(filePath))
			{
				files.add(filePath);
			}
		}
	}
	
	public void executeScript(string sourceFiles)
	{
		throw new NotImplementedException($"ScriptEngine: '{sourceFiles}' failed execution!");
	}
	
	public void executeScriptList()
	{
		if (Config.ALT_DEV_NO_QUESTS)
		{
			return;
		}
		
		LOGGER.Warn("ScriptEngine: failed execution!");
	}
	
	public string getCurrentLoadingScript()
	{
		//return _javaExecutionContext.getCurrentExecutingScript();
		return string.Empty;
	}
	
	public static ScriptEngineManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly ScriptEngineManager INSTANCE = new ScriptEngineManager();
	}
}