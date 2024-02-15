using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Mobius
 */
public class FakePlayerData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(FakePlayerData));
	
	private readonly Map<int, FakePlayerHolder> _fakePlayerInfos = new();
	private readonly Map<String, String> _fakePlayerNames = new();
	private readonly Map<String, int> _fakePlayerIds = new();
	private readonly Set<String> _talkableFakePlayerNames = new();
	
	protected FakePlayerData()
	{
		load();
	}
	
	public void load()
	{
		if (Config.FAKE_PLAYERS_ENABLED)
		{
			_fakePlayerInfos.clear();
			_fakePlayerNames.clear();
			_fakePlayerIds.clear();
			_talkableFakePlayerNames.clear();
			
			XDocument document = LoadXmlDocument(DataFileLocation.Data, "FakePlayerVisualData.xml");
			document.Elements("list").Elements("fakePlayer").ForEach(parseElement);

			LOGGER.Info(GetType().Name + ": Loaded " + _fakePlayerInfos.size() + " templates.");
		}
		else
		{
			LOGGER.Info(GetType().Name + ": Disabled.");
		}
	}
	
	private void parseElement(XElement element)
	{
		int npcId = element.Attribute("npcId").GetInt32();
		NpcTemplate template = NpcData.getInstance().getTemplate(npcId);
		String name = template.getName();
		if (CharInfoTable.getInstance().getIdByName(name) > 0)
		{
			LOGGER.Info(GetType().Name + ": Could not create fake player template " + npcId + ", player name already exists.");
		}
		else
		{
			_fakePlayerIds.put(name, npcId); // name - npcId
			_fakePlayerNames.put(name.toLowerCase(), name); // name to lowercase - name
			_fakePlayerInfos.put(npcId, new FakePlayerHolder(element));
			if (template.isFakePlayerTalkable())
			{
				_talkableFakePlayerNames.add(name.toLowerCase());
			}
		}
	}
	
	public int getNpcIdByName(String name)
	{
		return _fakePlayerIds.get(name);
	}
	
	public String getProperName(String name)
	{
		return _fakePlayerNames.get(name.ToLower());
	}
	
	public bool isTalkable(String name)
	{
		return _talkableFakePlayerNames.Contains(name.ToLower());
	}
	
	public FakePlayerHolder getInfo(int npcId)
	{
		return _fakePlayerInfos.get(npcId);
	}
	
	public static FakePlayerData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly FakePlayerData INSTANCE = new();
	}
}