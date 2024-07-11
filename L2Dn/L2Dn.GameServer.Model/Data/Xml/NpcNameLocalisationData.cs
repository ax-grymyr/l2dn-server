using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Mobius
 */
public class NpcNameLocalisationData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(NpcNameLocalisationData));
	
	private static readonly Map<string, Map<int, string[]>> NPC_NAME_LOCALISATIONS = new();
	private static string _lang;
	
	protected NpcNameLocalisationData()
	{
		load();
	}
	
	public void load()
	{
		NPC_NAME_LOCALISATIONS.clear();
		
		if (Config.MULTILANG_ENABLE)
		{
			foreach (string lang in Config.MULTILANG_ALLOWED)
			{
				string filePath = GetFullPath(DataFileLocation.Data, "lang/" + lang + "/NpcNameLocalisation.xml");
				if (!File.Exists(filePath))
					continue;
				
				NPC_NAME_LOCALISATIONS.put(lang, new());
				_lang = lang;

				XDocument document =
					LoadXmlDocument(DataFileLocation.Data, "lang/" + lang + "/NpcNameLocalisation.xml");

				document.Elements("list").Elements("localisation").ForEach(el =>
				{
					int id = el.GetAttributeValueAsInt32("id");
					string name = el.GetAttributeValueAsString("name");
					string title = el.GetAttributeValueAsString("title");
					NPC_NAME_LOCALISATIONS.get(_lang).put(id, [name, title]);
				});
				
				int count = NPC_NAME_LOCALISATIONS.get(lang).Count;
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
	
	/**
	 * @param lang
	 * @param id
	 * @return a String Array[] that contains NPC name and title or Null if is does not exist.
	 */
	public string[] getLocalisation(string lang, int id)
	{
		Map<int, string[]> localisations = NPC_NAME_LOCALISATIONS.get(lang);
		if (localisations != null)
		{
			return localisations.get(id);
		}
		return null;
	}
	
	public bool hasLocalisation(int id)
	{
		return NPC_NAME_LOCALISATIONS.Values.Any(data => data.ContainsKey(id));
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