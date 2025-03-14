using System.Collections.Immutable;
using L2Dn.CustomAttributes;
using L2Dn.Extensions;
using L2Dn.Utilities;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.Enums;

public static class SystemMessageIdUtil
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(SystemMessageIdUtil));
	private static readonly ImmutableDictionary<SystemMessageId, SystemMessageInfo> _infos;

	private static ImmutableDictionary<string, ImmutableDictionary<SystemMessageId, SystemMessageInfo>>
		_localizations = ImmutableDictionary<string, ImmutableDictionary<SystemMessageId, SystemMessageInfo>>
			.Empty;

	static SystemMessageIdUtil()
	{
		_infos = EnumUtil.GetValues<SystemMessageId>().Select(id =>
		{
			string? text = id.GetCustomAttribute<SystemMessageId, TextAttribute>()?.Text;
			int paramCount = ParseMessageParameters(id.ToString());
			return new SystemMessageInfo(id, text ?? string.Empty, paramCount);
		}).ToImmutableDictionary(info => info.MessageId, info => info);
	}

	public static SystemMessageInfo GetInfo(this SystemMessageId messageId) => _infos[messageId];
	public static string GetName(this SystemMessageId messageId) => _infos[messageId].Text;
	public static int GetParamCount(this SystemMessageId messageId) => _infos[messageId].ParamCount;

	private static int ParseMessageParameters(string name)
	{
		int paramCount = 0;
		for (int i = 0, count = name.Length - 1; i < count; i++)
		{
			char c1 = name[i];
			if (c1 is 'C' or 'S')
			{
				char c2 = name[i + 1];
				if (char.IsDigit(c2))
				{
					paramCount = Math.Max(paramCount, c2 - '0');
					i++;
				}
			}
		}

		return paramCount;
	}

	public static void loadLocalisations()
	{
		_localizations = ImmutableDictionary<string, ImmutableDictionary<SystemMessageId, SystemMessageInfo>>.Empty;
		if (!Config.MultilingualSupport.MULTILANG_ENABLE)
		{
			_logger.Info("SystemMessageId: MultiLanguage disabled.");
			return;
		}

		// ImmutableArray<string> languages = Config.MULTILANG_ALLOWED;
		// DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
		// factory.setValidating(false);
		// factory.setIgnoringComments(true);
		//
		// File file;
		// Node node;
		// Document doc;
		// NamedNodeMap nnmb;
		// SystemMessageId smId;
		// String text;
		// for (String lang : languages)
		// {
		// 	file = new File("data/lang/" + lang + "/SystemMessageLocalisation.xml");
		// 	if (!file.isFile())
		// 	{
		// 		continue;
		// 	}
		//
		// 	try
		// 	{
		// 		doc = factory.newDocumentBuilder().parse(file);
		// 		for (Node na = doc.getFirstChild(); na != null; na = na.getNextSibling())
		// 		{
		// 			if ("list".equals(na.getNodeName()))
		// 			{
		// 				for (Node nb = na.getFirstChild(); nb != null; nb = nb.getNextSibling())
		// 				{
		// 					if ("localisation".equals(nb.getNodeName()))
		// 					{
		// 						nnmb = nb.getAttributes();
		// 						node = nnmb.getNamedItem("id");
		// 						if (node != null)
		// 						{
		// 							smId = getSystemMessageId(Integer.parseInt(node.getNodeValue()));
		// 							if (smId == null)
		// 							{
		// 								LOGGER.log(Level.WARNING, "SystemMessageId: Unknown SMID '" + node.getNodeValue() + "', lang '" + lang + "'.");
		// 								continue;
		// 							}
		//
		// 							node = nnmb.getNamedItem("translation");
		// 							if (node == null)
		// 							{
		// 								LOGGER.log(Level.WARNING, "SystemMessageId: No text defined for SMID '" + smId + "', lang '" + lang + "'.");
		// 								continue;
		// 							}
		//
		// 							text = node.getNodeValue();
		// 							if (text.isEmpty() || (text.length() > 255))
		// 							{
		// 								LOGGER.log(Level.WARNING, "SystemMessageId: Invalid text defined for SMID '" + smId + "' (to long or empty), lang '" + lang + "'.");
		// 								continue;
		// 							}
		//
		// 							smId.attachLocalizedText(lang, text);
		// 						}
		// 					}
		// 				}
		// 			}
		// 		}
		// 	}
		// 	catch (Exception e)
		// 	{
		// 		_logger.Error("SystemMessageId: Failed loading '" + file + "': " + e);
		// 	}
		//
		// 	_logger.Info("SystemMessageId: Loaded localisations for [" + lang + "].");
		// }
	}

	// public SMLocalisation getLocalisation(String lang)
	// {
	// 	if (_localisations == null)
	// 	{
	// 		return null;
	// 	}
	//
	// 	SMLocalisation sml;
	// 	for (int i = _localisations.length; i-- > 0;)
	// 	{
	// 		sml = _localisations[i];
	// 		if (sml.getLanguage().hashCode() == lang.hashCode())
	// 		{
	// 			return sml;
	// 		}
	// 	}
	//
	// 	return null;
	// }
}