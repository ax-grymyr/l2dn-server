using L2Dn.GameServer.Data;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;
using L2Dn.Model.Enums;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.Olympiads;

/**
 * @author DS
 */
public class OlympiadManager
{
	private readonly Set<int> _nonClassBasedRegisters = new();
	private readonly Map<CharacterClass, Set<int>> _classBasedRegisters = new();

	protected OlympiadManager()
	{
	}

	public static OlympiadManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	public Set<int> getRegisteredNonClassBased()
	{
		return _nonClassBasedRegisters;
	}

	public Map<CharacterClass, Set<int>> getRegisteredClassBased()
	{
		return _classBasedRegisters;
	}

	public List<Set<int>> hasEnoughRegisteredClassed()
	{
		List<Set<int>> result = [];
		foreach (var classList in _classBasedRegisters)
		{
			if (classList.Value != null && classList.Value.size() >= Config.Olympiad.ALT_OLY_CLASSED)
			{
				result.Add(classList.Value);
			}
		}

		return result;
	}

	public bool hasEnoughRegisteredNonClassed()
	{
		return _nonClassBasedRegisters.size() >= Config.Olympiad.ALT_OLY_NONCLASSED;
	}

	public void clearRegistered()
	{
		_nonClassBasedRegisters.clear();
		_classBasedRegisters.Clear();
		AntiFeedManager.getInstance().clear(AntiFeedManager.OLYMPIAD_ID);
	}

	public bool isRegistered(Player noble)
	{
		return isRegistered(noble, noble, false);
	}

	private bool isRegistered(Player noble, Player player, bool showMessage)
	{
		int objId = noble.ObjectId;
		if (_nonClassBasedRegisters.Contains(objId))
		{
			if (showMessage)
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_IS_ALREADY_REGISTERED_FOR_ALL_CLASS_BATTLES);
				sm.Params.addPcName(noble);
				player.sendPacket(sm);
			}

			return true;
		}

		Set<int>? classed = _classBasedRegisters.get(getClassGroup(noble));
		if (classed != null && classed.Contains(objId))
		{
			if (showMessage)
			{
				SystemMessagePacket sm =
					new SystemMessagePacket(SystemMessageId.C1_IS_ALREADY_REGISTERED_ON_THE_CLASS_MATCH_WAITING_LIST);
				sm.Params.addPcName(noble);
				player.sendPacket(sm);
			}

			return true;
		}

		return false;
	}

	public bool isRegisteredInComp(Player noble)
	{
		return isRegistered(noble, noble, false) || isInCompetition(noble, noble, false);
	}

	private bool isInCompetition(Player noble, Player player, bool showMessage)
	{
		if (!Olympiad._inCompPeriod)
		{
			return false;
		}

		AbstractOlympiadGame? game;
		for (int i = OlympiadGameManager.getInstance().getNumberOfStadiums(); --i >= 0;)
		{
			game = OlympiadGameManager.getInstance().getOlympiadTask(i)?.getGame();
			if (game == null)
			{
				continue;
			}

			if (game.containsParticipant(noble.ObjectId))
			{
				if (!showMessage)
				{
					return true;
				}

				switch (game.getType())
				{
					case CompetitionType.CLASSED:
					{
						SystemMessagePacket sm =
							new SystemMessagePacket(SystemMessageId.C1_IS_ALREADY_REGISTERED_ON_THE_CLASS_MATCH_WAITING_LIST);
						sm.Params.addPcName(noble);
						player.sendPacket(sm);
						break;
					}
					case CompetitionType.NON_CLASSED:
					{
						SystemMessagePacket sm =
							new SystemMessagePacket(SystemMessageId.C1_IS_ALREADY_REGISTERED_FOR_ALL_CLASS_BATTLES);
						sm.Params.addPcName(noble);
						player.sendPacket(sm);
						break;
					}
				}

				return true;
			}
		}

		return false;
	}

	public bool registerNoble(Player player, CompetitionType type)
	{
		if (!Olympiad._inCompPeriod)
		{
			player.sendPacket(SystemMessageId.THE_OLYMPIAD_IS_NOT_HELD_RIGHT_NOW);
			return false;
		}

		if (Olympiad.getInstance().getMillisToCompEnd() < TimeSpan.FromMilliseconds(1200000))
		{
			player.sendPacket(SystemMessageId
				.GAME_PARTICIPATION_REQUEST_MUST_BE_FILED_NOT_EARLIER_THAN_10_MIN_AFTER_THE_GAME_ENDS);
			return false;
		}

		int charId = player.ObjectId;
		if (Olympiad.getInstance().getRemainingWeeklyMatches(charId) < 1)
		{
			player.sendPacket(SystemMessageId.THE_MAXIMUM_NUMBER_OF_MATCHES_YOU_CAN_PARTICIPATE_IN_1_WEEK_IS_25);
			return false;
		}

		if (isRegistered(player, player, true) || isInCompetition(player, player, true))
		{
			return false;
		}

		NobleData? statDat = Olympiad.getNobleStats(charId);
		if (statDat == null)
		{
			statDat = new NobleData
			{
				Class = player.getBaseClass(),
				CharacterName = player.getName(),
				OlympiadPoints = Olympiad.DEFAULT_POINTS
			};

			Olympiad.addNobleStats(charId, statDat);
		}

		switch (type)
		{
			case CompetitionType.CLASSED:
			{
				if (player.isRegisteredOnEvent())
				{
					player.sendMessage("You can't join olympiad while participating on an event.");
					return false;
				}

				if (Config.DualboxCheck.DUALBOX_CHECK_MAX_OLYMPIAD_PARTICIPANTS_PER_IP > 0 && !AntiFeedManager.getInstance()
					    .tryAddPlayer(AntiFeedManager.OLYMPIAD_ID, player,
						    Config.DualboxCheck.DUALBOX_CHECK_MAX_OLYMPIAD_PARTICIPANTS_PER_IP))
				{
					HtmlContent htmlContent = HtmlContent.LoadFromFile("html/mods/OlympiadIPRestriction.htm", player);
					htmlContent.Replace("%max%", AntiFeedManager.getInstance()
						.getLimit(player, Config.DualboxCheck.DUALBOX_CHECK_MAX_OLYMPIAD_PARTICIPANTS_PER_IP).ToString());

					NpcHtmlMessagePacket message =
						new NpcHtmlMessagePacket(player.getClient()?.HtmlActionValidator.OriginObjectId, 0, htmlContent);

					player.sendPacket(message);
					return false;
				}

				_classBasedRegisters.GetOrAdd(getClassGroup(player), _=> []).add(charId);
				player.sendPacket(SystemMessageId.YOU_VE_BEEN_REGISTERED_FOR_THE_OLYMPIAD_CLASS_MATCHES);
				break;
			}
			case CompetitionType.NON_CLASSED:
			{
				if (player.isRegisteredOnEvent())
				{
					player.sendMessage("You can't join olympiad while participating on an event.");
					return false;
				}

				if (Config.DualboxCheck.DUALBOX_CHECK_MAX_OLYMPIAD_PARTICIPANTS_PER_IP > 0 && !AntiFeedManager.getInstance()
					    .tryAddPlayer(AntiFeedManager.OLYMPIAD_ID, player,
						    Config.DualboxCheck.DUALBOX_CHECK_MAX_OLYMPIAD_PARTICIPANTS_PER_IP))
				{
					HtmlContent htmlContent = HtmlContent.LoadFromFile("html/mods/OlympiadIPRestriction.htm", player);
					htmlContent.Replace("%max%",
						AntiFeedManager.getInstance()
							.getLimit(player, Config.DualboxCheck.DUALBOX_CHECK_MAX_OLYMPIAD_PARTICIPANTS_PER_IP).ToString());

					NpcHtmlMessagePacket message =
						new NpcHtmlMessagePacket(player.getClient()?.HtmlActionValidator.OriginObjectId, 0, htmlContent);

					player.sendPacket(message);
					return false;
				}

				_nonClassBasedRegisters.add(charId);
				player.sendPacket(SystemMessageId.YOU_HAVE_REGISTERED_IN_THE_WORLD_OLYMPIAD);
				break;
			}
		}

		return true;
	}

	public bool unRegisterNoble(Player noble)
	{
		if (!Olympiad._inCompPeriod)
		{
			noble.sendPacket(SystemMessageId.THE_OLYMPIAD_IS_NOT_HELD_RIGHT_NOW);
			return false;
		}

		if ((!noble.isInCategory(CategoryType.THIRD_CLASS_GROUP) &&
		     !noble.isInCategory(CategoryType.FOURTH_CLASS_GROUP)) ||
		    noble.getLevel() < 55) // Classic noble equivalent check.
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId
				.CHARACTER_C1_DOES_NOT_MEET_THE_CONDITIONS_ONLY_CHARACTERS_WHO_HAVE_CHANGED_TWO_OR_MORE_CLASSES_CAN_PARTICIPATE_IN_OLYMPIAD);
			sm.Params.addString(noble.getName());
			noble.sendPacket(sm);
			return false;
		}

		if (!isRegistered(noble, noble, false))
		{
			noble.sendPacket(SystemMessageId.YOU_ARE_NOT_CURRENTLY_REGISTERED_FOR_THE_OLYMPIAD);
			return false;
		}

		if (isInCompetition(noble, noble, false))
		{
			return false;
		}

		int objId = noble.ObjectId;
		if (_nonClassBasedRegisters.remove(objId))
		{
			if (Config.DualboxCheck.DUALBOX_CHECK_MAX_OLYMPIAD_PARTICIPANTS_PER_IP > 0)
			{
				AntiFeedManager.getInstance().removePlayer(AntiFeedManager.OLYMPIAD_ID, noble);
			}

			noble.sendPacket(SystemMessageId.YOU_HAVE_BEEN_REMOVED_FROM_THE_OLYMPIAD_WAITING_LIST);
			return true;
		}

		Set<int>? classed = _classBasedRegisters.get(getClassGroup(noble));
		if (classed != null && classed.remove(objId))
		{
			if (Config.DualboxCheck.DUALBOX_CHECK_MAX_OLYMPIAD_PARTICIPANTS_PER_IP > 0)
			{
				AntiFeedManager.getInstance().removePlayer(AntiFeedManager.OLYMPIAD_ID, noble);
			}

			noble.sendPacket(SystemMessageId.YOU_HAVE_BEEN_REMOVED_FROM_THE_OLYMPIAD_WAITING_LIST);
			return true;
		}

		return false;
	}

	public void removeDisconnectedCompetitor(Player player)
	{
		OlympiadGameTask? task = OlympiadGameManager.getInstance().getOlympiadTask(player.getOlympiadGameId());
		if (task != null && task.isGameStarted())
		{
			task.getGame()?.handleDisconnect(player);
		}

		int objId = player.ObjectId;
		if (_nonClassBasedRegisters.remove(objId))
		{
			return;
		}

		_classBasedRegisters.GetValueOrDefault(getClassGroup(player))?.remove(objId);
	}

	public int getCountOpponents()
	{
		return _nonClassBasedRegisters.size() + _classBasedRegisters.Count;
	}

	private static class SingletonHolder
	{
		public static OlympiadManager INSTANCE = new OlympiadManager();
	}

	private CharacterClass getClassGroup(Player player)
	{
		return player.getBaseClass();
	}
}