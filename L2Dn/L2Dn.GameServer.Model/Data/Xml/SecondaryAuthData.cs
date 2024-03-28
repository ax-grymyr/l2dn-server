using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author NosBit
 */
public class SecondaryAuthData: DataReaderBase
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
		XDocument document = LoadXmlDocument(DataFileLocation.Config, "SecondaryAuth.xml");
		document.Elements("list").ForEach(parseElement);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _forbiddenPasswords.size() + " forbidden passwords.");
	}

	private void parseElement(XElement element)
	{
		_enabled = (bool)element.Elements("enabled").Single();
		_maxAttempts = (int)element.Elements("maxAttempts").Single();
		_banTime = (int)element.Elements("banTime").Single();
		_recoveryLink = (string)element.Elements("recoveryLink").Single();

		_forbiddenPasswords.addAll(element.Elements("forbiddenPasswords").Elements("password").Select(x => (string)x));
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