using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Mobius
 */
public class SendMessageLocalisationData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(SendMessageLocalisationData));
	
	private const string SPLIT_STRING = "XXX";
	private static readonly Map<string, Map<string[], string[]>> SEND_MESSAGE_LOCALISATIONS = new();
	private static string _lang;
	
	protected SendMessageLocalisationData()
	{
		load();
	}
	
	public void load()
	{
		SEND_MESSAGE_LOCALISATIONS.Clear();
		
		if (Config.MULTILANG_ENABLE)
		{
			foreach (string lang in Config.MULTILANG_ALLOWED)
			{
				string filePath = GetFullPath(DataFileLocation.Data, "lang/" + lang + "/SendMessageLocalisation.xml");
				if (!File.Exists(filePath))
					continue;
				
				SEND_MESSAGE_LOCALISATIONS.put(lang, new());
				_lang = lang;
				
				XDocument document =
					LoadXmlDocument(DataFileLocation.Data, "lang/" + lang + "/SendMessageLocalisation.xml");

				document.Elements("list").Elements("localisation").ForEach(el =>
				{
					string[] message = el.GetAttributeValueAsString("message").Split(SPLIT_STRING);
					string[] translation = el.GetAttributeValueAsString("translation").Split(SPLIT_STRING);
					SEND_MESSAGE_LOCALISATIONS.get(_lang).put(message, translation);
				});
				
				int count = SEND_MESSAGE_LOCALISATIONS.get(lang).Count;
				if (count == 0)
				{
					SEND_MESSAGE_LOCALISATIONS.remove(lang);
				}
				else
				{
					LOGGER.Info(GetType().Name + ": Loaded localisations for [" + lang + "].");
				}
			}
		}
	}
	
	public static string getLocalisation(Player player, string message)
	{
		if (Config.MULTILANG_ENABLE && (player != null))
		{
			Map<string[], string[]> localisations = SEND_MESSAGE_LOCALISATIONS.get(player.getLang());
			if (localisations != null)
			{
				// No pretty way of doing something like this.
				// Consider using proper SystemMessages where possible.
				string[] searchMessage;
				string[] replacementMessage;
				string localisation = message;
				bool found;
				foreach (var entry in localisations)
				{
					searchMessage = entry.Key;
					replacementMessage = entry.Value;
					
					// Exact match.
					if (searchMessage.Length == 1)
					{
						if (searchMessage[0].equals(localisation))
						{
							return replacementMessage[0];
						}
					}
					else // Split match.
					{
						found = true;
						foreach (string part in searchMessage)
						{
							if (!localisation.contains(part))
							{
								found = false;
								break;
							}
						}
						if (found)
						{
							for (int i = 0; i < searchMessage.Length; i++)
							{
								localisation = localisation.Replace(searchMessage[i], replacementMessage[i]);
							}
							break;
						}
					}
				}
				return localisation;
			}
		}
		return message;
	}
	
	public static SendMessageLocalisationData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly SendMessageLocalisationData INSTANCE = new();
	}
}