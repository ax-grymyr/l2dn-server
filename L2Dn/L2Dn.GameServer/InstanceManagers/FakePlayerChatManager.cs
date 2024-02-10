using System.Reflection.Metadata;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Geo;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;
using CollectionExtensions = L2Dn.GameServer.Utilities.CollectionExtensions;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Mobius
 */
public class FakePlayerChatManager: IXmlReader
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(FakePlayerChatManager));
	List<FakePlayerChatHolder> MESSAGES = new();
	private const int MIN_DELAY = 5000;
	private const int MAX_DELAY = 15000;
	
	protected FakePlayerChatManager()
	{
		load();
	}
	
	public void load()
	{
		if (Config.FAKE_PLAYERS_ENABLED && Config.FAKE_PLAYER_CHAT)
		{
			MESSAGES.Clear();
			parseDatapackFile("data/FakePlayerChatData.xml");
			LOGGER.Info(GetType().Name +": Loaded " + CollectionExtensions.size(MESSAGES) + " chat templates.");
		}
		else
		{
			LOGGER.Info(GetType().Name +": Disabled.");
		}
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "fakePlayerChat", fakePlayerChatNode =>
		{
			StatSet set = new StatSet(parseAttributes(fakePlayerChatNode));
			CollectionExtensions.add(MESSAGES, new FakePlayerChatHolder(set.getString("fpcName"), set.getString("searchMethod"), set.getString("searchText"), set.getString("answers")));
		}));
	}
	
	public void manageChat(Player player, String fpcName, String message)
	{
		ThreadPool.schedule(() => manageResponce(player, fpcName, message), Rnd.get(MIN_DELAY, MAX_DELAY));
	}
	
	public void manageChat(Player player, String fpcName, String message, int minDelay, int maxDelay)
	{
		ThreadPool.schedule(() => manageResponce(player, fpcName, message), Rnd.get(minDelay, maxDelay));
	}
	
	private void manageResponce(Player player, String fpcName, String message)
	{
		if (player == null)
		{
			return;
		}
		
		String text = message.ToLower();
		
		// tricky question
		if (text.Contains("can you see me"))
		{
			Spawn spawn = SpawnTable.getInstance().getAnySpawn(FakePlayerData.getInstance().getNpcIdByName(fpcName));
			if (spawn != null)
			{
				Npc npc = spawn.getLastSpawn();
				if (npc != null)
				{
					if (npc.calculateDistance2D(player) < 3000)
					{
						if (GeoEngine.getInstance().canSeeTarget(npc, player) && !player.isInvisible())
						{
							sendChat(player, fpcName, Rnd.nextBoolean() ? "i am not blind" : Rnd.nextBoolean() ? "of course i can" : "yes");
						}
						else
						{
							sendChat(player, fpcName, Rnd.nextBoolean() ? "i know you are around" : Rnd.nextBoolean() ? "not at the moment :P" : "no, where are you?");
						}
					}
					else
					{
						sendChat(player, fpcName, Rnd.nextBoolean() ? "nope, can't see you" : Rnd.nextBoolean() ? "nope" : "no");
					}
					return;
				}
			}
		}
		
		foreach (FakePlayerChatHolder chatHolder in MESSAGES)
		{
			if (!chatHolder.getFpcName().equals(fpcName) && !chatHolder.getFpcName().equals("ALL"))
			{
				continue;
			}
			
			switch (chatHolder.getSearchMethod())
			{
				case "EQUALS":
				{
					if (text.equals(chatHolder.getSearchText().get(0)))
					{
						sendChat(player, fpcName, chatHolder.getAnswers().get(Rnd.get(chatHolder.getAnswers().size())));
					}
					break;
				}
				case "STARTS_WITH":
				{
					if (text.startsWith(chatHolder.getSearchText().get(0)))
					{
						sendChat(player, fpcName, chatHolder.getAnswers().get(Rnd.get(chatHolder.getAnswers().size())));
					}
					break;
				}
				case "CONTAINS":
				{
					bool allFound = true;
					foreach (String word in chatHolder.getSearchText())
					{
						if (!text.Contains(word))
						{
							allFound = false;
						}
					}
					if (allFound)
					{
						sendChat(player, fpcName, chatHolder.getAnswers().get(Rnd.get(chatHolder.getAnswers().size())));
					}
					break;
				}
			}
		}
	}
	
	public void sendChat(Player player, String fpcName, String message)
	{
		Spawn spawn = SpawnTable.getInstance().getAnySpawn(FakePlayerData.getInstance().getNpcIdByName(fpcName));
		if (spawn != null)
		{
			Npc npc = spawn.getLastSpawn();
			if (npc != null)
			{
				player.sendPacket(new CreatureSay(npc, ChatType.WHISPER, fpcName, message));
			}
		}
	}
	
	public static FakePlayerChatManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly FakePlayerChatManager INSTANCE = new FakePlayerChatManager();
	}
}