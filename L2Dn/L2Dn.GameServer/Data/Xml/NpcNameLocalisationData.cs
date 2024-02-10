using L2Dn.GameServer.Model;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Mobius
 */
public class NpcNameLocalisationData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(NpcNameLocalisationData));
	
	private static readonly Map<String, Map<int, String[]>> NPC_NAME_LOCALISATIONS = new();
	private static String _lang;
	
	protected NpcNameLocalisationData()
	{
		load();
	}
	
	public void load()
	{
		NPC_NAME_LOCALISATIONS.clear();
		
		if (Config.MULTILANG_ENABLE)
		{
			foreach (String lang in Config.MULTILANG_ALLOWED)
			{
				File file = new File("data/lang/" + lang + "/NpcNameLocalisation.xml");
				if (!file.isFile())
				{
					continue;
				}
				
				NPC_NAME_LOCALISATIONS.put(lang, new());
				_lang = lang;
				parseDatapackFile("data/lang/" + lang + "/NpcNameLocalisation.xml");
				int count = NPC_NAME_LOCALISATIONS.get(lang).values().size();
				if (count == 0)
				{
					NPC_NAME_LOCALISATIONS.remove(lang);
				}
				else
				{
					LOGGER.Info(GetType().Name + ": Loaded localisations for [" + lang + "].");
				}
			}
		}
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "localisation", localisationNode =>
		{
			StatSet set = new StatSet(parseAttributes(localisationNode));
			NPC_NAME_LOCALISATIONS.get(_lang).put(set.getInt("id"), new String[]
			{
				set.getString("name"),
				set.getString("title")
			});
		}));
	}
	
	/**
	 * @param lang
	 * @param id
	 * @return a String Array[] that contains NPC name and title or Null if is does not exist.
	 */
	public String[] getLocalisation(String lang, int id)
	{
		Map<int, String[]> localisations = NPC_NAME_LOCALISATIONS.get(lang);
		if (localisations != null)
		{
			return localisations.get(id);
		}
		return null;
	}
	
	public bool hasLocalisation(int id)
	{
		foreach (Map<int, String[]> data in NPC_NAME_LOCALISATIONS.values())
		{
			if (data.containsKey(id))
			{
				return true;
			}
		}
		return false;
	}
	
	public static NpcNameLocalisationData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly NpcNameLocalisationData INSTANCE = new();
	}
}