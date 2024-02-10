using System.Runtime.CompilerServices;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author NosBit
 */
public class SecondaryAuthData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(SecondaryAuthData));
	
	private readonly Set<String> _forbiddenPasswords = new();
	private bool _enabled = false;
	private int _maxAttempts = 5;
	private int _banTime = 480;
	private String _recoveryLink = "";
	
	protected SecondaryAuthData()
	{
		load();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)] 
	public void load()
	{
		_forbiddenPasswords.clear();
		parseFile(new File("config/SecondaryAuth.xml"));
		LOGGER.Info(GetType().Name + ": Loaded " + _forbiddenPasswords.size() + " forbidden passwords.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		try
		{
			for (Node node = doc.getFirstChild(); node != null; node = node.getNextSibling())
			{
				if ("list".equalsIgnoreCase(node.getNodeName()))
				{
					for (Node list_node = node.getFirstChild(); list_node != null; list_node = list_node.getNextSibling())
					{
						if ("enabled".equalsIgnoreCase(list_node.getNodeName()))
						{
							_enabled = Boolean.parseBoolean(list_node.getTextContent());
						}
						else if ("maxAttempts".equalsIgnoreCase(list_node.getNodeName()))
						{
							_maxAttempts = int.Parse(list_node.getTextContent());
						}
						else if ("banTime".equalsIgnoreCase(list_node.getNodeName()))
						{
							_banTime = int.Parse(list_node.getTextContent());
						}
						else if ("recoveryLink".equalsIgnoreCase(list_node.getNodeName()))
						{
							_recoveryLink = list_node.getTextContent();
						}
						else if ("forbiddenPasswords".equalsIgnoreCase(list_node.getNodeName()))
						{
							for (Node forbiddenPasswords_node = list_node.getFirstChild(); forbiddenPasswords_node != null; forbiddenPasswords_node = forbiddenPasswords_node.getNextSibling())
							{
								if ("password".equalsIgnoreCase(forbiddenPasswords_node.getNodeName()))
								{
									_forbiddenPasswords.add(forbiddenPasswords_node.getTextContent());
								}
							}
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Failed to load secondary auth data from xml.", e);
		}
	}
	
	public bool isEnabled()
	{
		return _enabled;
	}
	
	public int getMaxAttempts()
	{
		return _maxAttempts;
	}
	
	public int getBanTime()
	{
		return _banTime;
	}
	
	public String getRecoveryLink()
	{
		return _recoveryLink;
	}
	
	public Set<String> getForbiddenPasswords()
	{
		return _forbiddenPasswords;
	}
	
	public bool isForbiddenPassword(String password)
	{
		return _forbiddenPasswords.Contains(password);
	}
	
	public static SecondaryAuthData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly SecondaryAuthData INSTANCE = new();
	}
}