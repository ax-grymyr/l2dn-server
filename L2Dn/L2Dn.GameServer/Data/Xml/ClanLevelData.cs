using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Mobius
 */
public class ClanLevelData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ClanLevelData));
	
	private const int EXPECTED_CLAN_LEVEL_DATA = 12; // Level 0 included.
	
	private int[] CLAN_EXP;
	private int MAX_CLAN_LEVEL = 0;
	private int MAX_CLAN_EXP = 0;
	
	protected ClanLevelData()
	{
		load();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)] 
	public void load()
	{
		CLAN_EXP = new int[EXPECTED_CLAN_LEVEL_DATA];
		MAX_CLAN_LEVEL = 0;
		MAX_CLAN_EXP = 0;
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "ClanLevelData.xml");
		document.Elements("list").Elements("clan").ForEach(element =>
		{
			int level = element.GetAttributeValueAsInt32("level");
			int exp = element.GetAttributeValueAsInt32("exp");
						
			if (MAX_CLAN_LEVEL < level)
			{
				MAX_CLAN_LEVEL = level;
			}
			if (MAX_CLAN_EXP < exp)
			{
				MAX_CLAN_EXP = exp;
			}
						
			CLAN_EXP[level] = exp;
		});
		
		LOGGER.Info(GetType().Name + ": Loaded " + (EXPECTED_CLAN_LEVEL_DATA - 1) /* level 0 excluded */ + " clan level data.");
	}
	
	public int getLevelExp(int clanLevel)
	{
		return CLAN_EXP[clanLevel];
	}
	
	public int getMaxLevel()
	{
		return MAX_CLAN_LEVEL;
	}
	
	public int getMaxExp()
	{
		return MAX_CLAN_EXP;
	}
	
	public static ClanLevelData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly ClanLevelData INSTANCE = new();
	}
}