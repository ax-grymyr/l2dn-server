using System.Reflection.Metadata;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Mobius
 */
public class FakePlayerChatManager: DataReaderBase
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

			XDocument document = LoadXmlDocument(DataFileLocation.Data, "FakePlayerChatData.xml");
			document.Elements("list").Elements("fakePlayerChat").ForEach(parseElement);

			LOGGER.Info(GetType().Name +": Loaded " + MESSAGES.Count + " chat templates.");
		}
		else
		{
			LOGGER.Info(GetType().Name +": Disabled.");
		}
	}

	private void parseElement(XElement element)
	{
		StatSet set = new StatSet(element);
		MESSAGES.Add(new FakePlayerChatHolder(set.getString("fpcName"), set.getString("searchMethod"),
				set.getString("searchText"), set.getString("answers")));
	}

	public void manageChat(Player player, string fpcName, string message)
	{
		ThreadPool.schedule(() => manageResponce(player, fpcName, message), Rnd.get(MIN_DELAY, MAX_DELAY));
	}

	public void manageChat(Player player, string fpcName, string message, int minDelay, int maxDelay)
	{
		ThreadPool.schedule(() => manageResponce(player, fpcName, message), Rnd.get(minDelay, maxDelay));
	}

	private void manageResponce(Player player, string fpcName, string message)
	{
		if (player == null)
		{
			return;
		}

		string text = message.ToLower();

		// tricky question
		if (text.Contains("can you see me"))
		{
			Spawn? spawn = SpawnTable.getInstance().getAnySpawn(FakePlayerData.getInstance().getNpcIdByName(fpcName));
			if (spawn != null)
			{
				Npc? npc = spawn.getLastSpawn();
				if (npc != null)
				{
					if (npc.Distance2D(player) < 3000)
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
					if (text.equals(chatHolder.getSearchText()[0]))
					{
						sendChat(player, fpcName, chatHolder.getAnswers().GetRandomElement());
					}
					break;
				}
				case "STARTS_WITH":
				{
					if (text.startsWith(chatHolder.getSearchText()[0]))
					{
						sendChat(player, fpcName, chatHolder.getAnswers().GetRandomElement());
					}
					break;
				}
				case "CONTAINS":
				{
					bool allFound = true;
					foreach (string word in chatHolder.getSearchText())
					{
						if (!text.Contains(word))
						{
							allFound = false;
						}
					}
					if (allFound)
					{
						sendChat(player, fpcName, chatHolder.getAnswers().GetRandomElement());
					}
					break;
				}
			}
		}
	}

	public void sendChat(Player player, string fpcName, string message)
	{
		Spawn? spawn = SpawnTable.getInstance().getAnySpawn(FakePlayerData.getInstance().getNpcIdByName(fpcName));
		if (spawn != null)
		{
			Npc? npc = spawn.getLastSpawn();
			if (npc != null)
			{
				player.sendPacket(new CreatureSayPacket(npc, ChatType.WHISPER, fpcName, message));
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