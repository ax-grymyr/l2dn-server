using System.Runtime.CompilerServices;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Mobius
 */
public class ClanLevelData
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
		
		parseDatapackFile("data/ClanLevelData.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + (EXPECTED_CLAN_LEVEL_DATA - 1) /* level 0 excluded */ + " clan level data.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equals(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("clan".equals(d.getNodeName()))
					{
						NamedNodeMap attrs = d.getAttributes();
						
						int level = parseInteger(attrs, "level");
						int exp = parseInteger(attrs, "exp");
						
						if (MAX_CLAN_LEVEL < level)
						{
							MAX_CLAN_LEVEL = level;
						}
						if (MAX_CLAN_EXP < exp)
						{
							MAX_CLAN_EXP = exp;
						}
						
						CLAN_EXP[level] = exp;
					}
				}
			}
		}
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