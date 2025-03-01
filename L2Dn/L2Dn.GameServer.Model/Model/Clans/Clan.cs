using System.Runtime.CompilerServices;
using L2Dn.Extensions;
using L2Dn.GameServer.CommunityBbs.Managers;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Clans;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.PledgeBonus;
using L2Dn.GameServer.Network.OutgoingPackets.PledgeV3;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Packets;
using Microsoft.EntityFrameworkCore;
using NLog;
using Forum = L2Dn.GameServer.CommunityBbs.BB.Forum;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Clans;

public class Clan: IIdentifiable, INamable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Clan));

	// Ally Penalty Types
	/** Clan leaved ally */
	public const int PENALTY_TYPE_CLAN_LEAVED = 1;
	/** Clan was dismissed from ally */
	public const int PENALTY_TYPE_CLAN_DISMISSED = 2;
	/** Leader clan dismiss clan from ally */
	public const int PENALTY_TYPE_DISMISS_CLAN = 3;
	/** Leader clan dissolve ally */
	public const int PENALTY_TYPE_DISSOLVE_ALLY = 4;
	// Sub-unit types
	/** Clan subunit type of Academy */
	public const int SUBUNIT_ACADEMY = -1;
	/** Clan subunit type of Royal Guard A */
	public const int SUBUNIT_ROYAL1 = 100;
	/** Clan subunit type of Royal Guard B */
	public const int SUBUNIT_ROYAL2 = 200;
	/** Clan subunit type of Order of Knights A-1 */
	public const int SUBUNIT_KNIGHT1 = 1001;
	/** Clan subunit type of Order of Knights A-2 */
	public const int SUBUNIT_KNIGHT2 = 1002;
	/** Clan subunit type of Order of Knights B-1 */
	public const int SUBUNIT_KNIGHT3 = 2001;
	/** Clan subunit type of Order of Knights B-2 */
	public const int SUBUNIT_KNIGHT4 = 2002;

	private string _name = string.Empty;
	private int _clanId;
	private ClanMember _leader;
	private readonly Map<int, ClanMember> _members = new();

	private string? _allyName;
	private int? _allyId;
	private int _level;
	private int? _castleId;
	private int? _fortId;
	private int _hideoutId;
	private int _hiredGuards;
	private int? _crestId;
	private int? _crestLargeId;
	private int? _allyCrestId;
	private int _auctionBiddedAt;
	private DateTime? _allyPenaltyExpiryTime;
	private int _allyPenaltyType;
	private DateTime? _charPenaltyExpiryTime;
	private DateTime? _dissolvingExpiryTime;
	private int _bloodAllianceCount;
	private int _bloodOathCount;

	private readonly ItemContainer _warehouse;
	private readonly Map<int, ClanWar> _atWarWith = new();

	private Forum? _forum;

	private readonly Map<int, Skill> _skills = new();
	private readonly Map<int, RankPrivs> _privs = new();
	private readonly Map<int, SubPledge> _subPledges = new();
	private readonly Map<int, Skill> _subPledgeSkills = new();

	private int _reputationScore;
	private int _rank;
	private int _exp;

	private string _notice = string.Empty;
	private bool _noticeEnabled;
	private const int MAX_NOTICE_LENGTH = 8192;
	private int? _newLeaderId;

	private int _siegeKills; // atomic
	private int _siegeDeaths; // atomic

	private ClanRewardBonus? _lastMembersOnlineBonus;
	private ClanRewardBonus? _lastHuntingBonus;

	private ClanVariables? _vars;

	/**
	 * Called if a clan is referenced only by id. In this case all other data needs to be fetched from db
	 * @param clanId A valid clan Id to create and restore
	 */
	public Clan(int clanId)
	{
		_clanId = clanId;
		_warehouse = new ClanWarehouse(this);
		initializePrivs();
        _leader = null!; // TODO: hack
		restore(); // TODO: clan must be instantiated in 2 ways: new clan by player or clan restored from db
		_warehouse.restore();

		ClanRewardBonus? availableOnlineBonus = getAvailableBonus(ClanRewardType.MEMBERS_ONLINE);
		if (_lastMembersOnlineBonus == null && availableOnlineBonus != null)
		{
			_lastMembersOnlineBonus = availableOnlineBonus;
		}

		ClanRewardBonus? availableHuntingBonus = getAvailableBonus(ClanRewardType.HUNTING_MONSTERS);
		if (_lastHuntingBonus == null && availableHuntingBonus != null)
		{
			_lastHuntingBonus = availableHuntingBonus;
		}
	}

	/**
	 * Called only if a new clan is created
	 * @param clanId A valid clan Id to create
	 * @param clanName A valid clan name
	 */
	public Clan(int clanId, string clanName, Player leader)
	{
		_clanId = clanId;
		_warehouse = new ClanWarehouse(this);
		_name = clanName;
		initializePrivs();
        _leader = new ClanMember(this, leader);
	}

	/**
	 * @return Returns the clanId.
	 */
	public int getId()
	{
		return _clanId;
	}

	/**
	 * @param clanId The clanId to set.
	 */
	public void setClanId(int clanId)
	{
		_clanId = clanId;
	}

	/**
	 * @return Returns the leaderId.
	 */
	public int getLeaderId()
	{
		return _leader != null ? _leader.getObjectId() : 0;
	}

	/**
	 * @return PledgeMember of clan leader.
	 */
	public ClanMember getLeader()
	{
		return _leader;
	}

	/**
	 * @param leader the leader to set.
	 */
	public void setLeader(ClanMember leader)
	{
		_leader = leader;
		_members.put(leader.getObjectId(), leader);
	}

	public void setNewLeader(ClanMember member)
	{
		Player? newLeader = member.getPlayer();
		ClanMember exMember = _leader;
		Player? exLeader = exMember.getPlayer();

		// Notify to scripts
		if (GlobalEvents.Global.HasSubscribers<OnClanLeaderChange>())
		{
			GlobalEvents.Global.NotifyAsync(new OnClanLeaderChange(exMember, member, this));
		}

		if (exLeader != null)
		{
			if (exLeader.isFlying())
			{
				exLeader.dismount();
			}

			if (getLevel() >= SiegeManager.getInstance().getSiegeClanMinLevel())
			{
				SiegeManager.getInstance().removeSiegeSkills(exLeader);
			}

			exLeader.setClanPrivileges(ClanPrivilege.None);
			exLeader.broadcastUserInfo();
		}
		else
		{
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				int leaderId = getLeaderId();
				ctx.Characters.Where(c => c.Id == leaderId).ExecuteUpdate(s => s.SetProperty(c => c.ClanPrivileges, 0));
			}
			catch (Exception e)
			{
				LOGGER.Error("Couldn't update clan privs for old clan leader: " + e);
			}
		}

		setLeader(member);
		if (_newLeaderId != 0)
		{
			setNewLeaderId(0, true);
		}
		updateClanInDB();

		if (exLeader != null)
		{
			exLeader.setPledgeClass(ClanMember.calculatePledgeClass(exLeader));
			exLeader.broadcastUserInfo();
			exLeader.checkItemRestriction();
		}

		if (newLeader != null)
		{
			newLeader.setPledgeClass(ClanMember.calculatePledgeClass(newLeader));
			newLeader.setClanPrivileges(ClanPrivilege.All);

			if (getLevel() >= SiegeManager.getInstance().getSiegeClanMinLevel())
			{
				SiegeManager.getInstance().addSiegeSkills(newLeader);
			}
			newLeader.broadcastUserInfo();
		}
		else
		{
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				int leaderId = getLeaderId();
				ctx.Characters.Where(c => c.Id == leaderId)
					.ExecuteUpdate(s => s.SetProperty(c => c.ClanPrivileges, (int)ClanPrivilege.All));
			}
			catch (Exception e)
			{
				LOGGER.Warn("Couldn't update clan privs for new clan leader: " + e);
			}
		}

		broadcastClanStatus();
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.CLAN_LEADER_PRIVILEGES_HAVE_BEEN_TRANSFERRED_TO_C1);
		sm.Params.addString(member.getName());
		broadcastToOnlineMembers(sm);

		LOGGER.Info("Leader of Clan: " + getName() + " changed to: " + member.getName() + " ex leader: " + exMember.getName());
	}

	/**
	 * @return the clan leader's name.
	 */
	public string getLeaderName()
	{
		if (_leader == null)
		{
			LOGGER.Warn("Clan " + _name + " without clan leader!");
			return "";
		}
		return _leader.getName();
	}

	/**
	 * @return the clan name.
	 */
	public string getName()
	{
		return _name;
	}

	/**
	 * @param name The name to set.
	 */
	public void setName(string name)
	{
		_name = name;
	}

	/**
	 * Adds a clan member to the clan.
	 * @param member the clan member.
	 */
	private void addClanMember(ClanMember member)
	{
		_members.put(member.getObjectId(), member);
	}

	/**
	 * Adds a clan member to the clan.<br>
	 * Using a different constructor, to make it easier to read.
	 * @param player the clan member
	 */
	public void addClanMember(Player player)
	{
		ClanMember member = new ClanMember(this, player);
		// store in memory
		addClanMember(member);
		member.setPlayer(player);
		player.setClan(this);
		player.setPledgeClass(ClanMember.calculatePledgeClass(player));
		player.sendPacket(new PledgeShowMemberListUpdatePacket(player));
		player.sendPacket(new PledgeSkillListPacket(this));
		addSkillEffects(player);

		// Notify to scripts
		if (GlobalEvents.Global.HasSubscribers<OnClanJoin>())
		{
			GlobalEvents.Global.NotifyAsync(new OnClanJoin(member, this));
		}
	}

	/**
	 * Updates player status in clan.
	 * @param player the player to be updated.
	 */
	public void updateClanMember(Player player)
    {
        Clan? clan = player.getClan();
        if (clan is null)
            return;

		ClanMember member = new ClanMember(clan, player);
		if (player.isClanLeader())
		{
			setLeader(member);
		}

		addClanMember(member);
	}

	/**
	 * @param name the name of the required clan member.
	 * @return the clan member for a given name.
	 */
	public ClanMember? getClanMember(string name)
	{
		foreach (ClanMember temp in _members.Values)
		{
			if (temp.getName().equals(name))
			{
				return temp;
			}
		}
		return null;
	}

	/**
	 * @param objectId the required clan member object Id.
	 * @return the clan member for a given {@code objectId}.
	 */
	public ClanMember? getClanMember(int objectId)
	{
		return _members.get(objectId);
	}

	/**
	 * @param objectId the object Id of the member that will be removed.
	 * @param clanJoinExpiryTime time penalty to join a clan.
	 */
	public void removeClanMember(int objectId, DateTime? clanJoinExpiryTime)
	{
		ClanMember? exMember = _members.remove(objectId);
		if (exMember == null)
		{
			LOGGER.Warn("Member Object ID: " + objectId + " not found in clan while trying to remove");
			return;
		}

		int leadssubpledge = getLeaderSubPledge(objectId);
		if (leadssubpledge != 0)
		{
			// Subunit leader withdraws, position becomes vacant and leader
			// should appoint new via NPC
            SubPledge? subPledge = getSubPledge(leadssubpledge);
            if (subPledge != null)
            {
                subPledge.setLeaderId(0);
                updateSubPledgeInDB(leadssubpledge);
            }
        }

		if (exMember.getApprentice() != 0)
		{
			ClanMember? apprentice = getClanMember(exMember.getApprentice());
			if (apprentice != null)
            {
                Player? apprenticePlayer = apprentice.getPlayer();
				if (apprenticePlayer != null)
				{
                    apprenticePlayer.setSponsor(0);
				}
				else
				{
					apprentice.setApprenticeAndSponsor(0, 0);
				}

				apprentice.saveApprenticeAndSponsor(0, 0);
			}
		}

		int? sponsorId = exMember.getSponsor();
		if (sponsorId != null)
		{
			ClanMember? sponsor = getClanMember(sponsorId.Value);
			if (sponsor != null)
			{
                Player? sponsorPlayer = sponsor.getPlayer();
				if (sponsorPlayer != null)
				{
                    sponsorPlayer.setApprentice(0);
				}
				else
				{
					sponsor.setApprenticeAndSponsor(0, 0);
				}

				sponsor.saveApprenticeAndSponsor(0, 0);
			}
		}
		exMember.saveApprenticeAndSponsor(0, 0);

		if (Config.REMOVE_CASTLE_CIRCLETS && _castleId != null)
		{
			CastleManager.getInstance().removeCirclet(exMember, _castleId.Value);
		}

        Player? player = exMember.getPlayer();
		if (exMember.isOnline() && player != null)
		{
			if (!player.isNoble())
			{
				player.setTitle("");
			}
			player.setApprentice(0);
			player.setSponsor(0);

			if (player.isClanLeader())
			{
				SiegeManager.getInstance().removeSiegeSkills(player);
				player.setClanCreateExpiryTime(DateTime.UtcNow.AddDays(Config.ALT_CLAN_CREATE_DAYS));
			}

			// remove Clan skills from Player
			removeSkillEffects(player);
			player.getEffectList().stopSkillEffects(SkillFinishType.REMOVED, (int)CommonSkill.CLAN_ADVENT);

			// remove Residential skills
			if (getCastleId() > 0)
			{
				Castle? castle = CastleManager.getInstance().getCastleByOwner(this);
				if (castle != null)
				{
					castle.removeResidentialSkills(player);
				}
			}
			if (getFortId() > 0)
			{
				Fort? fort = FortManager.getInstance().getFortByOwner(this);
				if (fort != null)
				{
					fort.removeResidentialSkills(player);
				}
			}
			player.sendSkillList();
			player.setClan(null);

			// players leaving from clan academy have no penalty
			if (exMember.getPledgeType() != -1)
			{
				player.setClanJoinExpiryTime(clanJoinExpiryTime);
			}

			player.setPledgeClass(ClanMember.calculatePledgeClass(player));
			player.broadcastUserInfo();
			// disable clan tab
			player.sendPacket(PledgeShowMemberListDeleteAllPacket.STATIC_PACKET);
		}
		else
		{
			removeMemberInDatabase(exMember, clanJoinExpiryTime, getLeaderId() == objectId ?
				DateTime.UtcNow.AddDays(Config.ALT_CLAN_CREATE_DAYS) : DateTime.MinValue);
		}

		// Notify to scripts
		if (GlobalEvents.Global.HasSubscribers<OnClanLeft>())
		{
			GlobalEvents.Global.NotifyAsync(new OnClanLeft(exMember, this));
		}
	}

	public ICollection<ClanMember> getMembers()
	{
		return _members.Values;
	}

	public int getMembersCount()
	{
		return _members.Count;
	}

	public int getSubPledgeMembersCount(int subpl)
	{
		int result = 0;
		foreach (ClanMember temp in _members.Values)
		{
			if (temp.getPledgeType() == subpl)
			{
				result++;
			}
		}
		return result;
	}

	/**
	 * @param pledgeType the Id of the pledge type.
	 * @return the maximum number of members allowed for a given {@code pledgeType}.
	 */
	public int getMaxNrOfMembers(int pledgeType)
	{
		int limit = 0;

		switch (pledgeType)
		{
			case 0:
			{
				switch (_level)
				{
					case 3:
					{
						limit = 30;
						break;
					}
					case 2:
					{
						limit = 20;
						break;
					}
					case 1:
					{
						limit = 15;
						break;
					}
					case 0:
					{
						limit = 10;
						break;
					}
					default:
					{
						limit = 40;
						break;
					}
				}
				break;
			}
			case -1:
			{
				limit = 20;
				break;
			}
			case 100:
			case 200:
			{
				switch (_level)
				{
					case 11:
					{
						limit = 30;
						break;
					}
					default:
					{
						limit = 20;
						break;
					}
				}
				break;
			}
			case 1001:
			case 1002:
			case 2001:
			case 2002:
			{
				switch (_level)
				{
					case 9:
					case 10:
					case 11:
					{
						limit = 25;
						break;
					}
					default:
					{
						limit = 10;
						break;
					}
				}
				break;
			}
			default:
			{
				break;
			}
		}
		return limit;
	}

	/**
	 * @param exclude the object Id to exclude from list.
	 * @return all online members excluding the one with object id {code exclude}.
	 */
	public List<Player> getOnlineMembers(int exclude)
	{
		List<Player> result = new();
		foreach (ClanMember member in _members.Values)
        {
            Player? memberPlayer = member.getPlayer();
			if (member.getObjectId() != exclude && member.isOnline() && memberPlayer != null)
			{
				result.Add(memberPlayer);
			}
		}
		return result;
	}

	/**
	 * @return the online clan member count.
	 */
	public int getOnlineMembersCount()
	{
		int count = 0;
		foreach (ClanMember member in _members.Values)
		{
			if (member.isOnline())
			{
				count++;
			}
		}
		return count;
	}

	/**
	 * @return the alliance Id.
	 */
	public int? getAllyId()
	{
		return _allyId;
	}

	/**
	 * @return the alliance name.
	 */
	public string? getAllyName()
	{
		return _allyName;
	}

	/**
	 * @param allyCrestId the alliance crest Id to be set.
	 */
	public void setAllyCrestId(int? allyCrestId)
	{
		_allyCrestId = allyCrestId;
	}

	/**
	 * @return the alliance crest Id.
	 */
	public int? getAllyCrestId()
	{
		return _allyCrestId;
	}

	/**
	 * @return the clan level.
	 */
	public int getLevel()
	{
		return _level;
	}

	/**
	 * Sets the clan level and updates the clan forum if it's needed.
	 * @param level the clan level to be set.
	 */
	private void setLevel(int level)
	{
		_level = level;
		if (_level >= 2 && _forum == null && Config.ENABLE_COMMUNITY_BOARD)
		{
			Forum? forum = ForumsBBSManager.getInstance().getForumByName("ClanRoot");
			if (forum != null)
			{
				_forum = forum.getChildByName(_name);
				if (_forum == null)
				{
					_forum = ForumsBBSManager.getInstance().createNewForum(_name, forum, Forum.CLAN, Forum.CLANMEMBERONLY, getId());
				}
			}
		}
	}

	/**
	 * @return the castle Id for this clan if owns a castle, zero otherwise.
	 */
	public int? getCastleId()
	{
		return _castleId;
	}

	/**
	 * @return the fort Id for this clan if owns a fort, zero otherwise.
	 */
	public int? getFortId()
	{
		return _fortId;
	}

	/**
	 * @return the hideout Id for this clan if owns a hideout, zero otherwise.
	 */
	public int getHideoutId()
	{
		return _hideoutId;
	}

	/**
	 * @param crestId the Id of the clan crest to be set.
	 */
	public void setCrestId(int? crestId)
	{
		_crestId = crestId;
	}

	/**
	 * @return Returns the clanCrestId.
	 */
	public int? getCrestId()
	{
		return _crestId;
	}

	/**
	 * @param crestLargeId The id of pledge LargeCrest.
	 */
	public void setCrestLargeId(int? crestLargeId)
	{
		_crestLargeId = crestLargeId;
	}

	/**
	 * @return Returns the clan CrestLargeId
	 */
	public int? getCrestLargeId()
	{
		return _crestLargeId;
	}

	/**
	 * @param allyId The allyId to set.
	 */
	public void setAllyId(int? allyId)
	{
		_allyId = allyId;
	}

	/**
	 * @param allyName The allyName to set.
	 */
	public void setAllyName(string? allyName)
	{
		_allyName = allyName;
	}

	/**
	 * @param castleId the castle Id to set.
	 */
	public void setCastleId(int? castleId)
	{
		_castleId = castleId;
	}

	/**
	 * @param fortId the fort Id to set.
	 */
	public void setFortId(int? fortId)
	{
		_fortId = fortId;
	}

	/**
	 * @param hideoutId the hideout Id to set.
	 */
	public void setHideoutId(int hideoutId)
	{
		_hideoutId = hideoutId;
	}

	/**
	 * @param id the Id of the player to be verified.
	 * @return {code true} if the player belongs to the clan.
	 */
	public bool isMember(int id)
	{
		return id != 0 && _members.ContainsKey(id);
	}

	/**
	 * @return the Blood Alliance count for this clan
	 */
	public int getBloodAllianceCount()
	{
		return _bloodAllianceCount;
	}

	/**
	 * Increase Blood Alliance count by config predefined count and updates the database.
	 */
	public void increaseBloodAllianceCount()
	{
		_bloodAllianceCount += SiegeManager.getInstance().getBloodAllianceReward();
		updateBloodAllianceCountInDB();
	}

	/**
	 * Reset the Blood Alliance count to zero and updates the database.
	 */
	public void resetBloodAllianceCount()
	{
		_bloodAllianceCount = 0;
		updateBloodAllianceCountInDB();
	}

	/**
	 * Store current Bloood Alliances count in database.
	 */
	public void updateBloodAllianceCountInDB()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.Clans.Where(c => c.Id == _clanId)
				.ExecuteUpdate(s => s.SetProperty(c => c.BloodAllianceCount, _bloodAllianceCount));
		}
		catch (Exception e)
		{
			LOGGER.Error("Exception on updateBloodAllianceCountInDB(): " + e);
		}
	}

	/**
	 * @return the Blood Oath count for this clan
	 */
	public int getBloodOathCount()
	{
		return _bloodOathCount;
	}

	/**
	 * Increase Blood Oath count by config predefined count and updates the database.
	 */
	public void increaseBloodOathCount()
	{
		_bloodOathCount += Config.FS_BLOOD_OATH_COUNT;
		updateBloodOathCountInDB();
	}

	/**
	 * Reset the Blood Oath count to zero and updates the database.
	 */
	public void resetBloodOathCount()
	{
		_bloodOathCount = 0;
		updateBloodOathCountInDB();
	}

	/**
	 * Store current Bloood Alliances count in database.
	 */
	public void updateBloodOathCountInDB()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.Clans.Where(c => c.Id == _clanId)
				.ExecuteUpdate(s => s.SetProperty(c => c.BloodOathCount, _bloodOathCount));
		}
		catch (Exception e)
		{
			LOGGER.Error("Exception on updateBloodAllianceCountInDB(): " + e);
		}
	}

	/**
	 * Updates in database clan information:
	 * <ul>
	 * <li>Clan leader Id</li>
	 * <li>Alliance Id</li>
	 * <li>Alliance name</li>
	 * <li>Clan's reputation</li>
	 * <li>Alliance's penalty expiration time</li>
	 * <li>Alliance's penalty type</li>
	 * <li>Character's penalty expiration time</li>
	 * <li>Dissolving expiration time</li>
	 * <li>Clan's id</li>
	 * </ul>
	 */
	public void updateClanInDB()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			Db.Clan? clan = ctx.Clans.SingleOrDefault(c => c.Id == _clanId);
			if (clan is null)
			{
				clan = new Db.Clan();
				clan.Id = _clanId;
				ctx.Clans.Add(clan);
			}

			clan.LeaderId = getLeaderId();
			clan.AllyId = _allyId;
			clan.AllyName = _allyName;
			clan.Reputation = _reputationScore;
			clan.AllyPenaltyExpireTime = _allyPenaltyExpiryTime;
			clan.AllyPenaltyExpireType = (byte)_allyPenaltyType;
			clan.CharPenaltyExpireTime = _charPenaltyExpiryTime;
			clan.DissolvingExpireTime = _dissolvingExpiryTime;
			clan.NewLeaderId = _newLeaderId;
			clan.Exp = _exp;

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Error saving clan: " + e);
		}
	}

	/**
	 * Stores in database clan information:
	 * <ul>
	 * <li>Clan Id</li>
	 * <li>Clan name</li>
	 * <li>Clan level</li>
	 * <li>Has castle</li>
	 * <li>Alliance Id</li>
	 * <li>Alliance name</li>
	 * <li>Clan leader Id</li>
	 * <li>Clan crest Id</li>
	 * <li>Clan large crest Id</li>
	 * <li>Alliance crest Id</li>
	 * </ul>
	 */
	public void store()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.Clans.Add(new Db.Clan()
			{
				Id = _clanId,
				Name = _name,
				Level = (byte)_level,
				Castle = (short?)_castleId,
				BloodAllianceCount = (short)_bloodAllianceCount,
				BloodOathCount = (short)_bloodOathCount,
				AllyId = _allyId,
				LeaderId = getLeaderId(),
				CrestId = _crestId,
				LargeCrestId = _crestLargeId,
				NewLeaderId = _newLeaderId,
				Exp = _exp,
			});

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Error saving new clan: " + e);
		}
	}

	/**
	 * @param member the clan member to be removed.
	 * @param clanJoinExpiryTime
	 * @param clanCreateExpiryTime
	 */
	private void removeMemberInDatabase(ClanMember member, DateTime? clanJoinExpiryTime, DateTime? clanCreateExpiryTime)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = member.getObjectId();
			Character? record = ctx.Characters.SingleOrDefault(c => c.Id == characterId);
			if (record is not null)
			{
				record.ClanId = null;
				record.Title = null;
				record.ClanJoinExpiryTime = clanJoinExpiryTime;
				record.ClanCreateExpiryTime = clanCreateExpiryTime;
				record.ClanPrivileges = 0;
				record.WantsPeace = false;
				record.SubPledge = 0;
				record.LevelJoinedAcademy = 0;
				record.SubPledge = 0;
				record.Apprentice = 0;
				record.SponsorId = null;

				ctx.SaveChanges();
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Error removing clan member: " + e);
		}
	}

	private void restore()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			Db.Clan? record = ctx.Clans.SingleOrDefault(c => c.Id == _clanId);
			if (record is not null)
			{
				setName(record.Name);
				setLevel(record.Level);
				setCastleId(record.Castle ?? 0);
				_bloodAllianceCount = record.BloodAllianceCount;
				_bloodOathCount = record.BloodOathCount;
				setAllyId(record.AllyId);
				setAllyName(record.AllyName);
				setAllyPenaltyExpiryTime(record.AllyPenaltyExpireTime, record.AllyPenaltyExpireType);
				if (_allyPenaltyExpiryTime < DateTime.UtcNow)
				{
					setAllyPenaltyExpiryTime(null, 0);
				}
				setCharPenaltyExpiryTime(record.CharPenaltyExpireTime);
				if (_charPenaltyExpiryTime + TimeSpan.FromMinutes(Config.ALT_CLAN_JOIN_MINS) < DateTime.UtcNow) // 24*60*60*1000 = 60000
				{
					setCharPenaltyExpiryTime(null);
				}
				setDissolvingExpiryTime(record.DissolvingExpireTime);

				setCrestId(record.CrestId);
				setCrestLargeId(record.LargeCrestId);
				setAllyCrestId(record.AllyCrestId);

				_exp = record.Exp;
				setReputationScore(record.Reputation);
				setAuctionBiddedAt(record.AuctionBidAt, false);
				setNewLeaderId(record.NewLeaderId, false);

				int leaderId = record.LeaderId;

				foreach (Character character in ctx.Characters.Where(c => c.ClanId == _clanId))
				{
					ClanMember member = new ClanMember(this, character);
					if (member.getObjectId() == leaderId)
					{
						setLeader(member);
					}
					else
					{
						addClanMember(member);
					}
				}
			}

			restoreSubPledges();
			restoreRankPrivs();
			restoreSkills();
			restoreNotice();
		}
		catch (Exception e)
		{
			LOGGER.Error("Error restoring clan data: " + e);
		}
	}

	private void restoreNotice()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ClanNotice? notice = ctx.ClanNotices.SingleOrDefault(c => c.ClanId == _clanId);
			if (notice is not null)
			{
				_noticeEnabled = notice.Enabled;
				_notice = notice.Notice;
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Error restoring clan notice: " + e);
		}
	}

	private void storeNotice(string noticeValue, bool enabled)
	{
		string notice = noticeValue;
		if (notice == null)
		{
			notice = "";
		}

		if (notice.Length > MAX_NOTICE_LENGTH)
		{
			notice = notice.Substring(0, MAX_NOTICE_LENGTH - 1);
		}

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ClanNotice? record = ctx.ClanNotices.SingleOrDefault(c => c.ClanId == _clanId);
			if (record is null)
			{
				record = new ClanNotice();
				record.ClanId = _clanId;
				ctx.ClanNotices.Add(record);
			}

			record.Notice = notice;
			record.Enabled = enabled;

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Error could not store clan notice: " + e);
		}

		_notice = notice;
		_noticeEnabled = enabled;
	}

	public void setNoticeEnabled(bool enabled)
	{
		storeNotice(getNotice(), enabled);
	}

	public void setNotice(string notice)
	{
		storeNotice(notice, _noticeEnabled);
	}

	public bool isNoticeEnabled()
	{
		return _noticeEnabled;
	}

	public string getNotice()
	{
		if (_notice == null)
		{
			return "";
		}

		// Bypass exploit check.
		string text = _notice.toLowerCase();
		if (text.contains("action") && text.contains("bypass"))
		{
			return "";
		}

		// Returns text without tags.
		return _notice.replaceAll("<.*?>", "");
	}

	private void restoreSkills()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (ClanSkill clanSkill in ctx.ClanSkills.Where(c => c.ClanId == _clanId))
			{
				int id = clanSkill.SkillId;
				int level = clanSkill.SkillLevel;

                // Create a Skill object for each record
                Skill skill = SkillData.getInstance().getSkill(id, level) ??
                    throw new InvalidOperationException("Clan Skill not found for id: " + id); // TODO: null checking hack

                // Add the Skill object to the Clan _skills
				int subType = clanSkill.SubPledgeId;
				if (subType == -2)
				{
					_skills.put(skill.getId(), skill);
				}
				else if (subType == 0)
				{
					_subPledgeSkills.put(skill.getId(), skill);
				}
				else
				{
					SubPledge? subunit = _subPledges.get(subType);
					if (subunit != null)
					{
						subunit.addNewSkill(skill);
					}
					else
					{
						LOGGER.Info("Missing subpledge " + subType + " for clan " + this + ", skill skipped.");
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Error restoring clan skills: " + e);
		}
	}

	/**
	 * @return all the clan skills.
	 */
	public ICollection<Skill> getAllSkills()
	{
		if (_skills == null)
		{
			return new List<Skill>();
		}

		return _skills.Values;
	}

	/**
	 * @return the map containing this clan skills.
	 */
	public Map<int, Skill> getSkills()
	{
		return _skills;
	}

	/**
	 * Used to add a skill to skill list of this Pledge
	 * @param newSkill
	 * @return
	 */
	public Skill? addSkill(Skill newSkill)
	{
		Skill? oldSkill = null;
		if (newSkill != null)
		{
			// Replace oldSkill by newSkill or Add the newSkill
			oldSkill = _skills.put(newSkill.getId(), newSkill);
		}
		return oldSkill;
	}

	public Skill? addNewSkill(Skill newSkill)
	{
		return addNewSkill(newSkill, -2);
	}

	/**
	 * Used to add a new skill to the list, send a packet to all online clan members, update their stats and store it in db
	 * @param newSkill
	 * @param subType
	 * @return
	 */
	public Skill? addNewSkill(Skill newSkill, int subType)
	{
		Skill? oldSkill = null;
		if (newSkill != null)
		{
			if (subType == -2)
			{
				oldSkill = _skills.put(newSkill.getId(), newSkill);
			}
			else if (subType == 0)
			{
				oldSkill = _subPledgeSkills.put(newSkill.getId(), newSkill);
			}
			else
			{
				SubPledge? subunit = getSubPledge(subType);
				if (subunit != null)
				{
					oldSkill = subunit.addNewSkill(newSkill);
				}
				else
				{
					LOGGER.Warn("Subpledge " + subType + " does not exist for clan " + this);
					return oldSkill;
				}
			}

			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				if (oldSkill != null)
				{
					int skillId = oldSkill.getId();
					int skillLevel = newSkill.getLevel();
					ctx.ClanSkills.Where(r => r.SkillId == skillId && r.ClanId == _clanId)
						.ExecuteUpdate(s => s.SetProperty(r => r.SkillLevel, skillLevel));
				}
				else
				{
					ctx.ClanSkills.Add(new ClanSkill()
					{
						ClanId = _clanId,
						SkillId = newSkill.getId(),
						SkillLevel = (short)newSkill.getLevel(),
						SkillName = newSkill.getName(),
						SubPledgeId = subType
					});

					ctx.SaveChanges();
				}
			}
			catch (Exception e)
			{
				LOGGER.Error("Error could not store clan skills: " + e);
			}

			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_CLAN_SKILL_S1_HAS_BEEN_ADDED);
			sm.Params.addSkillName(newSkill.getId());

			foreach (ClanMember temp in _members.Values)
            {
                Player? tempPlayer = temp.getPlayer();
				if (temp != null && tempPlayer != null && temp.isOnline())
				{
					if (subType == -2)
					{
						if (newSkill.getMinPledgeClass() <= tempPlayer.getPledgeClass())
						{
                            tempPlayer.addSkill(newSkill, false); // Skill is not saved to player DB
                            tempPlayer.sendPacket(new PledgeSkillListAddPacket(newSkill.getId(), newSkill.getLevel()));
                            tempPlayer.sendPacket(sm);
                            tempPlayer.sendSkillList();
						}
					}
					else if (temp.getPledgeType() == subType)
					{
                        tempPlayer.addSkill(newSkill, false); // Skill is not saved to player DB
                        tempPlayer.sendPacket(new ExSubPledgeSkillAddPacket(subType, newSkill.getId(), newSkill.getLevel()));
                        tempPlayer.sendPacket(sm);
                        tempPlayer.sendSkillList();
					}
				}
			}
		}

		return oldSkill;
	}

	public void addSkillEffects()
	{
		foreach (Skill skill in _skills.Values)
		{
			foreach (ClanMember temp in _members.Values)
			{
				try
                {
                    Player? tempPlayer = temp.getPlayer();
					if (temp != null && tempPlayer != null && temp.isOnline() && skill.getMinPledgeClass() <= tempPlayer.getPledgeClass())
					{
						tempPlayer.addSkill(skill, false); // Skill is not saved to player DB
					}
				}
				catch (Exception e)
				{
					LOGGER.Warn(e);
				}
			}
		}
	}

	public void addSkillEffects(Player player)
	{
		if (player == null)
		{
			return;
		}

		SocialClass playerSocialClass = (SocialClass)player.getPledgeClass() + 1;
		foreach (Skill skill in _skills.Values)
		{
			SkillLearn? skillLearn = SkillTreeData.getInstance().getPledgeSkill(skill.getId(), skill.getLevel());
			if (skillLearn == null || skillLearn.getSocialClass() == null || playerSocialClass >= skillLearn.getSocialClass())
			{
				player.addSkill(skill, false); // Skill is not saved to player DB
			}
		}
		if (player.getPledgeType() == 0)
		{
			foreach (Skill skill in _subPledgeSkills.Values)
			{
				SkillLearn? skillLearn = SkillTreeData.getInstance().getSubPledgeSkill(skill.getId(), skill.getLevel());
				if (skillLearn == null || skillLearn.getSocialClass() == null || playerSocialClass >= skillLearn.getSocialClass())
				{
					player.addSkill(skill, false); // Skill is not saved to player DB
				}
			}
		}
		else
		{
			SubPledge? subunit = getSubPledge(player.getPledgeType());
			if (subunit == null)
			{
				return;
			}
			foreach (Skill skill in subunit.getSkills())
			{
				player.addSkill(skill, false); // Skill is not saved to player DB
			}
		}

		if (_reputationScore < 0)
		{
			skillsStatus(player, true);
		}
	}

	public void removeSkillEffects(Player player)
	{
		if (player == null)
		{
			return;
		}

		foreach (Skill skill in _skills.Values)
		{
			player.removeSkill(skill, false); // Skill is not saved to player DB
		}

		if (player.getPledgeType() == 0)
		{
			foreach (Skill skill in _subPledgeSkills.Values)
			{
				player.removeSkill(skill, false); // Skill is not saved to player DB
			}
		}
		else
		{
			SubPledge? subunit = getSubPledge(player.getPledgeType());
			if (subunit == null)
			{
				return;
			}
			foreach (Skill skill in subunit.getSkills())
			{
				player.removeSkill(skill, false); // Skill is not saved to player DB
			}
		}
	}

	public void skillsStatus(Player player, bool disable)
	{
		if (player == null)
		{
			return;
		}

		foreach (Skill skill in _skills.Values)
		{
			if (disable)
			{
				player.disableSkill(skill, TimeSpan.Zero);
			}
			else
			{
				player.enableSkill(skill, false);
			}
		}

		if (player.getPledgeType() == 0)
		{
			foreach (Skill skill in _subPledgeSkills.Values)
			{
				if (disable)
				{
					player.disableSkill(skill, TimeSpan.Zero);
				}
				else
				{
					player.enableSkill(skill, false);
				}
			}
		}
		else
		{
			SubPledge? subunit = getSubPledge(player.getPledgeType());
			if (subunit != null)
			{
				foreach (Skill skill in subunit.getSkills())
				{
					if (disable)
					{
						player.disableSkill(skill, TimeSpan.Zero);
					}
					else
					{
						player.enableSkill(skill, false);
					}
				}
			}
		}
	}

	public void broadcastToOnlineAllyMembers<TPacket>(TPacket packet)
		where TPacket: struct, IOutgoingPacket
	{
		if (_allyId is not null)
		{
			foreach (Clan clan in ClanTable.getInstance().getClanAllies(_allyId.Value))
			{
				clan.broadcastToOnlineMembers(packet);
			}
		}
	}

	public void broadcastToOnlineMembers<TPacket>(TPacket packet)
		where TPacket: struct, IOutgoingPacket
	{
		foreach (ClanMember member in _members.Values)
		{
            Player? memberPlayer = member.getPlayer();
			if (member != null && member.isOnline() && memberPlayer != null)
			{
				memberPlayer.sendPacket(packet);
			}
		}
	}

	public void broadcastCSToOnlineMembers(CreatureSayPacket packet, Player broadcaster)
	{
		foreach (ClanMember member in _members.Values)
		{
            Player? memberPlayer = member.getPlayer();
			if (member != null && memberPlayer != null && member.isOnline() && !BlockList.isBlocked(memberPlayer, broadcaster))
			{
                memberPlayer.sendPacket(packet);
			}
		}
	}

	public void broadcastToOtherOnlineMembers<TPacket>(TPacket packet, Player player)
		where TPacket: struct, IOutgoingPacket
	{
		foreach (ClanMember member in _members.Values)
        {
            Player? memberPlayer = member.getPlayer();
			if (member != null && member.isOnline() && memberPlayer != player && memberPlayer != null)
			{
                memberPlayer.sendPacket(packet);
			}
		}
	}

	public override string ToString()
	{
		return _name + "[" + _clanId + "]";
	}

	public ItemContainer getWarehouse()
	{
		return _warehouse;
	}

	public bool isAtWarWith(int clanId)
	{
		return _atWarWith.ContainsKey(clanId);
	}

	public bool isAtWarWith(Clan clan)
	{
		if (clan == null)
		{
			return false;
		}
		return _atWarWith.ContainsKey(clan.getId());
	}

	public int getHiredGuards()
	{
		return _hiredGuards;
	}

	public void incrementHiredGuards()
	{
		_hiredGuards++;
	}

	public bool isAtWar()
	{
		return _atWarWith.Count != 0;
	}

	public Map<int, ClanWar> getWarList()
	{
		return _atWarWith;
	}

	public void broadcastClanStatus()
	{
		foreach (Player member in getOnlineMembers(0))
		{
			member.sendPacket(PledgeShowMemberListDeleteAllPacket.STATIC_PACKET);
			PledgeShowMemberListAllPacket.sendAllTo(member);
		}
	}

	public class SubPledge
	{
		private readonly int _id;
		private string _subPledgeName;
		private int _leaderId;
		private readonly Map<int, Skill> _subPledgeSkills = new();

		public SubPledge(int id, string name, int leaderId)
		{
			_id = id;
			_subPledgeName = name;
			_leaderId = leaderId;
		}

		public int getId()
		{
			return _id;
		}

		public string getName()
		{
			return _subPledgeName;
		}

		public void setName(string name)
		{
			_subPledgeName = name;
		}

		public int getLeaderId()
		{
			return _leaderId;
		}

		public void setLeaderId(int leaderId)
		{
			_leaderId = leaderId;
		}

		public Skill? addNewSkill(Skill skill)
		{
			return _subPledgeSkills.put(skill.getId(), skill);
		}

		public ICollection<Skill> getSkills()
		{
			return _subPledgeSkills.Values;
		}

		public Skill? getSkill(int id)
		{
			return _subPledgeSkills.get(id);
		}
	}

	public class RankPrivs
	{
		private readonly int _rankId;
		private readonly int _party; // TODO find out what this stuff means and implement it
		private ClanPrivilege _rankPrivs;

		public RankPrivs(int rank, int party, ClanPrivilege privs)
		{
			_rankId = rank;
			_party = party;
			_rankPrivs = privs;
		}

		public int getRank()
		{
			return _rankId;
		}

		public int getParty()
		{
			return _party;
		}

		public ClanPrivilege getPrivs()
		{
			return _rankPrivs;
		}

		public void setPrivs(ClanPrivilege privs)
		{
			_rankPrivs = privs;
		}
	}

	private void restoreSubPledges()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (ClanSubPledge record in ctx.ClanSubPledges.Where(c => c.ClanId == _clanId))
			{
				int id = record.SubPledgeId;
				string name = record.Name;
				int leaderId = record.LeaderId;
				// Create a SubPledge object for each record
				SubPledge pledge = new SubPledge(id, name, leaderId);
				_subPledges.put(id, pledge);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not restore clan sub-units: " + e);
		}
	}

	/**
	 * used to retrieve subPledge by type
	 * @param pledgeType
	 * @return
	 */
	public SubPledge? getSubPledge(int pledgeType)
	{
		return _subPledges.GetValueOrDefault(pledgeType);
	}

	/**
	 * Used to retrieve subPledge by type
	 * @param pledgeName
	 * @return
	 */
	public SubPledge? getSubPledge(string pledgeName)
	{
		foreach (SubPledge sp in _subPledges.Values)
		{
			if (sp.getName().equalsIgnoreCase(pledgeName))
				return sp;
		}

		return null;
	}

	/**
	 * Used to retrieve all subPledges
	 * @return
	 */
	public ICollection<SubPledge> getAllSubPledges()
	{
		if (_subPledges == null)
		{
			return new List<SubPledge>();
		}
		return _subPledges.Values;
	}

	public SubPledge? createSubPledge(Player player, int pledgeTypeValue, int leaderId, string subPledgeName)
	{
		int pledgeType = getAvailablePledgeTypes(pledgeTypeValue);
		if (pledgeType == 0)
		{
			if (pledgeType == SUBUNIT_ACADEMY)
			{
				player.sendPacket(SystemMessageId.YOUR_CLAN_HAS_ALREADY_ESTABLISHED_A_CLAN_ACADEMY);
			}
			else
			{
				player.sendMessage("You can't create any more sub-units of this type");
			}
			return null;
		}

        if (_leader.getObjectId() == leaderId)
		{
			player.sendMessage("Leader is not correct");
			return null;
		}

		// Royal Guard 5000 points per each
		// Order of Knights 10000 points per each
		if (pledgeType != -1 && ((_reputationScore < Config.ROYAL_GUARD_COST && pledgeType < SUBUNIT_KNIGHT1) || (_reputationScore < Config.KNIGHT_UNIT_COST && pledgeType > SUBUNIT_ROYAL2)))
		{
			player.sendPacket(SystemMessageId.THE_CLAN_REPUTATION_IS_TOO_LOW);
			return null;
		}

        SubPledge subPledge;
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.ClanSubPledges.Add(new ClanSubPledge()
			{
				ClanId = _clanId,
				SubPledgeId = pledgeType,
				Name = subPledgeName,
				LeaderId = pledgeType != -1 ? leaderId : 0
			});

			ctx.SaveChanges();

			subPledge = new SubPledge(pledgeType, subPledgeName, leaderId);
			_subPledges.put(pledgeType, subPledge);

			if (pledgeType != -1)
			{
				// Royal Guard 5000 points per each
				// Order of Knights 10000 points per each
				if (pledgeType < SUBUNIT_KNIGHT1)
				{
					setReputationScore(_reputationScore - Config.ROYAL_GUARD_COST);
				}
				else
				{
					setReputationScore(_reputationScore - Config.KNIGHT_UNIT_COST);
					// TODO: clan lvl9 or more can reinforce knights cheaper if first knight unit already created, use Config.KNIGHT_REINFORCE_COST
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Error saving sub clan data: " + e);
            return null; // TODO: just throw
        }

		broadcastToOnlineMembers(new PledgeShowInfoUpdatePacket(_leader.getClan()));
		broadcastToOnlineMembers(new PledgeReceiveSubPledgeCreatedPacket(subPledge, _leader.getClan()));
		return subPledge;
	}

	public int getAvailablePledgeTypes(int pledgeType)
	{
		if (_subPledges.get(pledgeType) != null)
		{
			// LOGGER.warning("found sub-unit with id: "+pledgeType);
			switch (pledgeType)
			{
				case SUBUNIT_ACADEMY:
				{
					return 0;
				}
				case SUBUNIT_ROYAL1:
				{
					return getAvailablePledgeTypes(SUBUNIT_ROYAL2);
				}
				case SUBUNIT_ROYAL2:
				{
					return 0;
				}
				case SUBUNIT_KNIGHT1:
				{
					return getAvailablePledgeTypes(SUBUNIT_KNIGHT2);
				}
				case SUBUNIT_KNIGHT2:
				{
					return getAvailablePledgeTypes(SUBUNIT_KNIGHT3);
				}
				case SUBUNIT_KNIGHT3:
				{
					return getAvailablePledgeTypes(SUBUNIT_KNIGHT4);
				}
				case SUBUNIT_KNIGHT4:
				{
					return 0;
				}
			}
		}
		return pledgeType;
	}

	public void updateSubPledgeInDB(int pledgeType)
	{
		try
        {
            SubPledge? subPledge = getSubPledge(pledgeType);
            if (subPledge == null)
            {
                LOGGER.Warn($"Requested to update non-existing sub-pledge type={pledgeType}");
                return;
            }

			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int leaderId = subPledge.getLeaderId();
			string name = subPledge.getName();
			ctx.ClanSubPledges.Where(r => r.ClanId == _clanId && r.SubPledgeId == pledgeType)
				.ExecuteUpdate(s => s.SetProperty(r => r.LeaderId, leaderId).SetProperty(r => r.Name, name));
		}
		catch (Exception e)
		{
			LOGGER.Error("Error updating subpledge: " + e);
		}
	}

	private void restoreRankPrivs()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (var record in ctx.ClanPrivileges.Where(c => c.ClanId == _clanId))
			{
				int rank = record.Rank;
				// int party = rset.getInt("party");
				int privileges = record.Privileges;
				// Create a SubPledge object for each record
				if (rank == -1)
				{
					continue;
				}

                _privs.GetOrAdd(rank, r => new RankPrivs(r, 0, ClanPrivilege.None)).
                    setPrivs((ClanPrivilege)privileges);
            }
		}
		catch (Exception e)
		{
			LOGGER.Error("Error restoring clan privs by rank: " + e);
		}
	}

	public void initializePrivs()
	{
		for (int i = 1; i < 10; i++)
		{
			_privs.put(i, new RankPrivs(i, 0, ClanPrivilege.None));
		}
	}

	public ClanPrivilege getRankPrivs(int rank)
	{
		return _privs.get(rank)?.getPrivs() ?? ClanPrivilege.None;
	}

	public void setRankPrivs(int rank, ClanPrivilege privs)
    {
        RankPrivs? rankPrivs = _privs.get(rank);
		if (rankPrivs != null)
		{
            rankPrivs.setPrivs(privs);

			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ClanPrivileges? record =
					ctx.ClanPrivileges.SingleOrDefault(r => r.ClanId == _clanId && r.Rank == rank && r.Party == 0);

				if (record is null)
				{
					record = new ClanPrivileges()
					{
						ClanId = _clanId,
						Party = 0,
						Rank = rank
					};

					ctx.ClanPrivileges.Add(record);
				}

				record.Privileges = (int)privs;
				ctx.SaveChanges();
			}
			catch (Exception e)
			{
				LOGGER.Error("Could not store clan privs for rank: " + e);
			}

			foreach (ClanMember cm in _members.Values)
            {
                Player? cmPlayer = cm.getPlayer();
				if (cm.isOnline() && cm.getPowerGrade() == rank && cmPlayer != null)
				{
                    cmPlayer.setClanPrivileges(privs);
                    cmPlayer.updateUserInfo();
				}
			}
			broadcastClanStatus();
		}
		else
		{
			_privs.put(rank, new RankPrivs(rank, 0, privs));

			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ClanPrivileges? record =
					ctx.ClanPrivileges.SingleOrDefault(r => r.ClanId == _clanId && r.Rank == rank && r.Party == 0);

				if (record is null)
				{
					record = new ClanPrivileges()
					{
						ClanId = _clanId,
						Party = 0,
						Rank = rank
					};

					ctx.ClanPrivileges.Add(record);
				}

				record.Privileges = (int)privs;
				ctx.SaveChanges();
			}
			catch (Exception e)
			{
				LOGGER.Error("Could not create new rank and store clan privs for rank: " + e);
			}
		}
	}

	/**
	 * @return all RankPrivs.
	 */
	public ICollection<RankPrivs> getAllRankPrivs()
	{
		return _privs == null ? new List<RankPrivs>() : _privs.Values;
	}

	public int getLeaderSubPledge(int leaderId)
	{
		int id = 0;
		foreach (SubPledge sp in _subPledges.Values)
		{
			if (sp.getLeaderId() == 0)
			{
				continue;
			}
			if (sp.getLeaderId() == leaderId)
			{
				id = sp.getId();
			}
		}
		return id;
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void addReputationScore(int value)
	{
		setReputationScore(_reputationScore + value);
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void takeReputationScore(int value)
	{
		setReputationScore(_reputationScore - value);
	}

	private void setReputationScore(int value)
	{
		if (_reputationScore >= 0 && value < 0)
		{
			broadcastToOnlineMembers(new SystemMessagePacket(SystemMessageId.SINCE_THE_CLAN_REPUTATION_HAS_DROPPED_BELOW_0_YOUR_CLAN_SKILL_S_WILL_BE_DE_ACTIVATED));
			foreach (ClanMember member in _members.Values)
			{
                Player? memberPlayer = member.getPlayer();
				if (member.isOnline() && memberPlayer != null)
				{
					skillsStatus(memberPlayer, true);
				}
			}
		}
		else if (_reputationScore < 0 && value >= 0)
		{
			broadcastToOnlineMembers(new SystemMessagePacket(SystemMessageId.CLAN_SKILLS_WILL_NOW_BE_ACTIVATED_SINCE_THE_CLAN_REPUTATION_IS_1_OR_HIGHER));
			foreach (ClanMember member in _members.Values)
            {
                Player? memberPlayer = member.getPlayer();
				if (member.isOnline() && memberPlayer != null)
				{
					skillsStatus(memberPlayer, false);
				}
			}
		}

		_reputationScore = value;
		if (_reputationScore > 100000000)
		{
			_reputationScore = 100000000;
		}
		if (_reputationScore < -100000000)
		{
			_reputationScore = -100000000;
		}

		broadcastToOnlineMembers(new PledgeShowInfoUpdatePacket(this));
	}

	public int getReputationScore()
	{
		return _reputationScore;
	}

	public void setRank(int rank)
	{
		_rank = rank;
	}

	public int getRank()
	{
		return _rank;
	}

	public int getAuctionBiddedAt()
	{
		return _auctionBiddedAt;
	}

	public void setAuctionBiddedAt(int id, bool storeInDb)
	{
		_auctionBiddedAt = id;
		if (storeInDb)
		{
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.Clans.Where(c => c.Id == _clanId).ExecuteUpdate(s => s.SetProperty(c => c.AuctionBidAt, id));
			}
			catch (Exception e)
			{
				LOGGER.Warn("Could not store auction for clan: " + e);
			}
		}
	}

	/**
	 * @param player the clan inviting player.
	 * @param target the invited player.
	 * @param pledgeType the pledge type to join.
	 * @return {core true} if player and target meet various conditions to join a clan.
	 */
	public bool checkClanJoinCondition(Player player, Player target, int pledgeType)
	{
		if (player == null)
		{
			return false;
		}
		if (!player.hasClanPrivilege(ClanPrivilege.CL_JOIN_CLAN))
		{
			player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
			return false;
		}
		if (target == null)
		{
			player.sendPacket(SystemMessageId.THE_TARGET_CANNOT_BE_INVITED);
			return false;
		}
		if (player.ObjectId == target.ObjectId)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_ASK_YOURSELF_TO_APPLY_TO_A_CLAN);
			return false;
		}
		if (_charPenaltyExpiryTime > DateTime.UtcNow)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_ACCEPT_A_NEW_CLAN_MEMBER_FOR_24_H_AFTER_DISMISSING_SOMEONE);
			return false;
		}
		if (target.getClanId() != 0)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_IS_ALREADY_A_MEMBER_OF_ANOTHER_CLAN);
			sm.Params.addString(target.getName());
			player.sendPacket(sm);
			return false;
		}
		if (target.getClanJoinExpiryTime() > DateTime.UtcNow)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_WILL_BE_ABLE_TO_JOIN_YOUR_CLAN_IN_S2_MIN_AFTER_LEAVING_THE_PREVIOUS_ONE);
			sm.Params.addString(target.getName());
			sm.Params.addInt(Config.ALT_CLAN_JOIN_MINS);
			player.sendPacket(sm);
			return false;
		}
		if ((target.getLevel() > 40 || target.getClassId().GetLevel() >= 2) && pledgeType == -1)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_DOES_NOT_MEET_THE_REQUIREMENTS_TO_JOIN_A_CLAN_ACADEMY);
			sm.Params.addString(target.getName());
			player.sendPacket(sm);
			player.sendPacket(SystemMessageId.IN_ORDER_TO_JOIN_THE_CLAN_ACADEMY_YOU_MUST_BE_UNAFFILIATED_WITH_A_CLAN_AND_BE_AN_UNAWAKENED_CHARACTER_LV_84_OR_BELOW_FOR_BOTH_MAIN_AND_SUBCLASS);
			return false;
		}
		if (getSubPledgeMembersCount(pledgeType) >= getMaxNrOfMembers(pledgeType))
		{
			if (pledgeType == 0)
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_IS_FULL_AND_CANNOT_ACCEPT_ADDITIONAL_CLAN_MEMBERS_AT_THIS_TIME);
				sm.Params.addString(_name);
				player.sendPacket(sm);
			}
			else
			{
				player.sendPacket(SystemMessageId.THE_CLAN_IS_FULL);
			}
			return false;
		}
		return true;
	}

	/**
	 * @param player the clan inviting player.
	 * @param target the invited player.
	 * @return {core true} if player and target meet various conditions to join a clan.
	 */
	public bool checkAllyJoinCondition(Player player, Player target)
	{
		if (player == null)
		{
			return false;
		}

		int? playerAllyId = player.getAllyId();
        Clan? leaderClan = player.getClan();
		if (playerAllyId is null || !player.isClanLeader() || leaderClan == null || player.getClanId() != playerAllyId)
		{
			player.sendPacket(SystemMessageId.ACCESS_ONLY_FOR_THE_CHANNEL_FOUNDER);
			return false;
		}

		if (leaderClan.getAllyPenaltyExpiryTime() > DateTime.UtcNow && leaderClan.getAllyPenaltyType() == PENALTY_TYPE_DISMISS_CLAN)
		{
			player.sendPacket(SystemMessageId.YOU_CAN_ACCEPT_A_NEW_CLAN_IN_THE_ALLIANCE_IN_24_H_AFTER_DISMISSING_ANOTHER_ONE);
			return false;
		}

		if (target == null)
		{
			player.sendPacket(SystemMessageId.THE_TARGET_CANNOT_BE_INVITED);
			return false;
		}

		if (player.ObjectId == target.ObjectId)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_ASK_YOURSELF_TO_APPLY_TO_A_CLAN);
			return false;
		}

        Clan? targetClan = target.getClan();
		if (targetClan == null)
		{
			player.sendPacket(SystemMessageId.THE_TARGET_MUST_BE_A_CLAN_MEMBER);
			return false;
		}

		if (!target.isClanLeader())
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_IS_NOT_A_CLAN_LEADER);
			sm.Params.addString(target.getName());
			player.sendPacket(sm);
			return false;
		}

		if (target.getAllyId() is not null)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_CLAN_IS_ALREADY_A_MEMBER_OF_S2_ALLIANCE);
			sm.Params.addString(targetClan.getName());
			sm.Params.addString(targetClan.getAllyName() ?? string.Empty);
			player.sendPacket(sm);
			return false;
		}

		if (targetClan.getAllyPenaltyExpiryTime() > DateTime.UtcNow)
		{
			if (targetClan.getAllyPenaltyType() == PENALTY_TYPE_CLAN_LEAVED)
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_CLAN_CANNOT_JOIN_THE_ALLIANCE_BECAUSE_ONE_DAY_HAS_NOT_YET_PASSED_SINCE_THEY_LEFT_ANOTHER_ALLIANCE);
				sm.Params.addString(targetClan.getName());
				sm.Params.addString(targetClan.getAllyName() ?? string.Empty);
				player.sendPacket(sm);
				return false;
			}

			if (targetClan.getAllyPenaltyType() == PENALTY_TYPE_CLAN_DISMISSED)
			{
				player.sendPacket(SystemMessageId.A_CLAN_CAN_JOIN_ANOTHER_ALLIANCE_IN_24_H_AFTER_LEAVING_THE_PREVIOUS_ONE);
				return false;
			}
		}

		if (player.isInsideZone(ZoneId.SIEGE) && target.isInsideZone(ZoneId.SIEGE))
		{
			player.sendPacket(SystemMessageId.THE_OPPOSING_CLAN_IS_PARTICIPATING_IN_A_SIEGE_BATTLE);
			return false;
		}

		if (leaderClan.isAtWarWith(targetClan.getId()))
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_MAKE_AN_ALLIANCE_WITH_A_CLAN_YOU_ARE_IN_WAR_WITH);
			return false;
		}

		if (ClanTable.getInstance().getClanAllies(playerAllyId.Value).Count >= Config.ALT_MAX_NUM_OF_CLANS_IN_ALLY)
		{
			player.sendPacket(SystemMessageId.YOU_HAVE_EXCEEDED_THE_LIMIT);
			return false;
		}

		return true;
	}

	public DateTime? getAllyPenaltyExpiryTime()
	{
		return _allyPenaltyExpiryTime;
	}

	public int getAllyPenaltyType()
	{
		return _allyPenaltyType;
	}

	public void setAllyPenaltyExpiryTime(DateTime? expiryTime, int penaltyType)
	{
		_allyPenaltyExpiryTime = expiryTime;
		_allyPenaltyType = penaltyType;
	}

	public DateTime? getCharPenaltyExpiryTime()
	{
		return _charPenaltyExpiryTime;
	}

	public void setCharPenaltyExpiryTime(DateTime? time)
	{
		_charPenaltyExpiryTime = time;
	}

	public DateTime? getDissolvingExpiryTime()
	{
		return _dissolvingExpiryTime;
	}

	public void setDissolvingExpiryTime(DateTime? time)
	{
		_dissolvingExpiryTime = time;
	}

	public void createAlly(Player player, string allyName)
	{
		if (null == player)
		{
			return;
		}

		if (!player.isClanLeader())
		{
			player.sendPacket(SystemMessageId.ONLY_CLAN_LEADERS_MAY_CREATE_ALLIANCES);
			return;
		}
		if (_allyId != 0)
		{
			player.sendPacket(SystemMessageId.YOU_ALREADY_BELONG_TO_ANOTHER_ALLIANCE);
			return;
		}
		if (_level < 5)
		{
			player.sendPacket(SystemMessageId.TO_CREATE_AN_ALLIANCE_YOUR_CLAN_MUST_BE_LV_5_OR_HIGHER);
			return;
		}
		if (_allyPenaltyExpiryTime > DateTime.UtcNow && _allyPenaltyType == PENALTY_TYPE_DISSOLVE_ALLY)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_CREATE_A_NEW_ALLIANCE_WITHIN_1_DAY_OF_DISSOLUTION);
			return;
		}
		if (_dissolvingExpiryTime > DateTime.UtcNow)
		{
			player.sendPacket(SystemMessageId.AS_YOU_ARE_CURRENTLY_SCHEDULE_FOR_CLAN_DISSOLUTION_NO_ALLIANCE_CAN_BE_CREATED);
			return;
		}
		if (string.IsNullOrEmpty(allyName) || !allyName.ContainsAlphaNumericOnly())
		{
			player.sendPacket(SystemMessageId.INCORRECT_ALLIANCE_NAME_PLEASE_TRY_AGAIN);
			return;
		}
		if (allyName.Length > 16 || allyName.Length < 2)
		{
			player.sendPacket(SystemMessageId.INCORRECT_LENGTH_FOR_AN_ALLIANCE_NAME);
			return;
		}
		if (ClanTable.getInstance().isAllyExists(allyName))
		{
			player.sendPacket(SystemMessageId.THAT_ALLIANCE_NAME_ALREADY_EXISTS);
			return;
		}

		setAllyId(_clanId);
		setAllyName(allyName.Trim());
		setAllyPenaltyExpiryTime(null, 0);
		updateClanInDB();

		player.updateUserInfo();

		// TODO: Need correct message id
		player.sendMessage("Alliance " + allyName + " has been created.");
	}

	public void dissolveAlly(Player player)
	{
		if (_allyId is null)
		{
			player.sendPacket(SystemMessageId.YOU_ARE_NOT_IN_AN_ALLIANCE);
			return;
		}
		if (!player.isClanLeader() || _clanId != _allyId)
		{
			player.sendPacket(SystemMessageId.ACCESS_ONLY_FOR_THE_CHANNEL_FOUNDER);
			return;
		}
		if (player.isInsideZone(ZoneId.SIEGE))
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_DISSOLVE_AN_ALLIANCE_WHILE_AN_AFFILIATED_CLAN_IS_PARTICIPATING_IN_A_SIEGE_BATTLE);
			return;
		}

		broadcastToOnlineAllyMembers(new SystemMessagePacket(SystemMessageId.THE_ALLIANCE_IS_DISBANDED));

		DateTime currentTime = DateTime.UtcNow;
		foreach (Clan clan in ClanTable.getInstance().getClanAllies(_allyId.Value))
		{
			if (clan.getId() != getId())
			{
				clan.setAllyId(0);
				clan.setAllyName(null);
				clan.setAllyPenaltyExpiryTime(null, 0);
				clan.updateClanInDB();
			}
		}

		setAllyId(0);
		setAllyName(null);
		changeAllyCrest(0, false);
		setAllyPenaltyExpiryTime(currentTime + TimeSpan.FromDays(Config.ALT_CREATE_ALLY_DAYS_WHEN_DISSOLVED),
			PENALTY_TYPE_DISSOLVE_ALLY);

		updateClanInDB();
	}

	// Unused?
	public bool levelUpClan(Player player)
	{
		if (!player.isClanLeader())
		{
			player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
			return false;
		}
		if (DateTime.UtcNow < _dissolvingExpiryTime)
		{
			player.sendPacket(SystemMessageId.AS_YOU_ARE_CURRENTLY_SCHEDULE_FOR_CLAN_DISSOLUTION_YOUR_CLAN_LEVEL_CANNOT_BE_INCREASED);
			return false;
		}

		bool increaseClanLevel = false;

		// Such as https://l2wiki.com/classic/Clans_%E2%80%93_Clan_Level
		switch (_level)
		{
			case 0:
			{
				// Upgrade to 1
				if (player.getSp() >= 1000 && player.getAdena() >= 150000 && _members.Count >= 1 && player.reduceAdena("ClanLvl", 150000, player.getTarget(), true))
				{
					player.setSp(player.getSp() - 1000);
					SystemMessagePacket sp = new SystemMessagePacket(SystemMessageId.YOUR_SP_HAS_DECREASED_BY_S1);
					sp.Params.addInt(1000);
					player.sendPacket(sp);
					increaseClanLevel = true;
				}
				break;
			}
			case 1:
			{
				// Upgrade to 2
				if (player.getSp() >= 15000 && player.getAdena() >= 300000 && _members.Count >= 1 && player.reduceAdena("ClanLvl", 300000, player.getTarget(), true))
				{
					player.setSp(player.getSp() - 15000);
					SystemMessagePacket sp = new SystemMessagePacket(SystemMessageId.YOUR_SP_HAS_DECREASED_BY_S1);
					sp.Params.addInt(15000);
					player.sendPacket(sp);
					increaseClanLevel = true;
				}
				break;
			}
			case 2:
			{
				// Upgrade to 3 (itemId 1419 == Blood Mark)
				if (player.getSp() >= 100000 && player.getInventory().getItemByItemId(1419) != null && _members.Count >= 1 && player.destroyItemByItemId("ClanLvl", 1419, 100, player.getTarget(), true))
				{
					player.setSp(player.getSp() - 100000);
					SystemMessagePacket sp = new SystemMessagePacket(SystemMessageId.YOUR_SP_HAS_DECREASED_BY_S1);
					sp.Params.addInt(100000);
					player.sendPacket(sp);
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_DISAPPEARED);
					sm.Params.addItemName(1419);
					player.sendPacket(sm);
					increaseClanLevel = true;
				}
				break;
			}
			case 3:
			{
				// Upgrade to 4 (itemId 1419 == Blood Mark)
				if (player.getSp() >= 1000000 && player.getInventory().getItemByItemId(1419) != null && _members.Count >= 1 && player.destroyItemByItemId("ClanLvl", 1419, 5000, player.getTarget(), true))
				{
					player.setSp(player.getSp() - 1000000);
					SystemMessagePacket sp = new SystemMessagePacket(SystemMessageId.YOUR_SP_HAS_DECREASED_BY_S1);
					sp.Params.addInt(1000000);
					player.sendPacket(sp);
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_DISAPPEARED);
					sm.Params.addItemName(1419);
					player.sendPacket(sm);
					increaseClanLevel = true;
				}
				break;
			}
			case 4:
			{
				// Upgrade to 5 (itemId 1419 == Blood Mark)
				if (player.getSp() >= 5000000 && player.getInventory().getItemByItemId(1419) != null && _members.Count >= 1 && player.destroyItemByItemId("ClanLvl", 1419, 10000, player.getTarget(), true))
				{
					player.setSp(player.getSp() - 5000000);
					SystemMessagePacket sp = new SystemMessagePacket(SystemMessageId.YOUR_SP_HAS_DECREASED_BY_S1);
					sp.Params.addInt(5000000);
					player.sendPacket(sp);
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_DISAPPEARED);
					sm.Params.addItemName(1419);
					player.sendPacket(sm);
					increaseClanLevel = true;
				}
				break;
			}
			default:
			{
				return false;
			}
		}

		if (!increaseClanLevel)
		{
			player.sendPacket(SystemMessageId.THE_CONDITIONS_NECESSARY_TO_INCREASE_THE_CLAN_S_LEVEL_HAVE_NOT_BEEN_MET);
			return false;
		}

		// the player should know that he has less sp now :p
        if (!player.isSubclassLocked())
        {
            UserInfoPacket ui = new UserInfoPacket(player, false);
            ui.AddComponentType(UserInfoType.CURRENT_HPMPCP_EXP_SP);
            player.sendPacket(ui);
        }

        player.sendItemList();

		changeLevel(_level + 1);

		// Notify to scripts
		if (GlobalEvents.Global.HasSubscribers<OnClanLvlUp>())
		{
			GlobalEvents.Global.NotifyAsync(new OnClanLvlUp(player, this));
		}

		return true;
	}

	public void changeLevel(int level)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.Clans.Where(c => c.Id == _clanId).ExecuteUpdate(s => s.SetProperty(c => c.Level, level));
		}
		catch (Exception e)
		{
			LOGGER.Warn("could not increase clan level: " + e);
		}

		setLevel(level);

        Player? leader = _leader.getPlayer();
		if (leader != null && _leader.isOnline())
		{
			if (level > 4)
			{
				SiegeManager.getInstance().addSiegeSkills(leader);
				leader.sendPacket(SystemMessageId.NOW_THAT_YOUR_CLAN_LEVEL_IS_ABOVE_LEVEL_5_IT_CAN_ACCUMULATE_CLAN_REPUTATION);
			}
			else if (level < 5) // TODO: invalid condition??
			{
				SiegeManager.getInstance().removeSiegeSkills(leader);
			}
		}

		// notify all the members about it
		broadcastToOnlineMembers(new ExPledgeLevelUpPacket(level));
		broadcastToOnlineMembers(new SystemMessagePacket(SystemMessageId.YOUR_CLAN_S_LEVEL_HAS_INCREASED));
		broadcastToOnlineMembers(new PledgeShowInfoUpdatePacket(this));
	}

	/**
	 * Change the clan crest. If crest id is 0, crest is removed. New crest id is saved to database.
	 * @param crestId if 0, crest is removed, else new crest id is set and saved to database
	 */
	public void changeClanCrest(int? crestId)
	{
		if (_crestId is not null)
		{
			CrestTable.getInstance().removeCrest(_crestId.Value);
		}

		setCrestId(crestId);

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.Clans.Where(c => c.Id == _clanId).ExecuteUpdate(s => s.SetProperty(c => c.CrestId, crestId));
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not update crest for clan " + _name + " [" + _clanId + "] : " + e);
		}

		foreach (Player member in getOnlineMembers(0))
		{
			member.broadcastUserInfo();
		}
	}

	/**
	 * Change the ally crest. If crest id is 0, crest is removed. New crest id is saved to database.
	 * @param crestId if 0, crest is removed, else new crest id is set and saved to database
	 * @param onlyThisClan
	 */
	public void changeAllyCrest(int? crestId, bool onlyThisClan)
	{
		if (!onlyThisClan && _allyCrestId != null)
		{
			CrestTable.getInstance().removeCrest(_allyCrestId.Value);
		}

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var query = onlyThisClan ? ctx.Clans.Where(c => c.Id == _clanId) : ctx.Clans.Where(c => c.AllyId == _allyId);
			query.ExecuteUpdate(s => s.SetProperty(c => c.AllyCrestId, crestId));
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not update ally crest for ally/clan: " + e);
		}

		if (onlyThisClan)
		{
			setAllyCrestId(crestId);
			foreach (Player member in getOnlineMembers(0))
			{
				member.broadcastUserInfo();
			}
		}
		else
		{
			foreach (Clan clan in ClanTable.getInstance().getClanAllies(getAllyId() ?? 0))
			{
				clan.setAllyCrestId(crestId);
				foreach (Player member in clan.getOnlineMembers(0))
				{
					member.broadcastUserInfo();
				}
			}
		}
	}

	/**
	 * Change the large crest. If crest id is 0, crest is removed. New crest id is saved to database.
	 * @param crestId if 0, crest is removed, else new crest id is set and saved to database
	 */
	public void changeLargeCrest(int? crestId)
	{
		if (_crestLargeId != null)
		{
			CrestTable.getInstance().removeCrest(_crestLargeId.Value);
		}

		setCrestLargeId(crestId);

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.Clans.Where(c => c.Id == _clanId).ExecuteUpdate(s => s.SetProperty(c => c.LargeCrestId, crestId));
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not update large crest for clan " + _name + " [" + _clanId + "] : " + e);
		}

		foreach (Player member in getOnlineMembers(0))
		{
			member.broadcastUserInfo();
		}
	}

	/**
	 * Check if this clan can learn the skill for the given skill ID, level.
	 * @param skillId
	 * @param skillLevel
	 * @return {@code true} if skill can be learned.
	 */
	public bool isLearnableSubSkill(int skillId, int skillLevel)
	{
		Skill? current = _subPledgeSkills.get(skillId);
		// is next level?
		if (current != null && current.getLevel() + 1 == skillLevel)
		{
			return true;
		}
		// is first level?
		if (current == null && skillLevel == 1)
		{
			return true;
		}
		// other sub-pledges
		foreach (SubPledge subunit in _subPledges.Values)
		{
			// disable academy
			if (subunit.getId() == -1)
			{
				continue;
			}
			current = subunit.getSkill(skillId);
			// is next level?
			if (current != null && current.getLevel() + 1 == skillLevel)
			{
				return true;
			}
			// is first level?
			if (current == null && skillLevel == 1)
			{
				return true;
			}
		}
		return false;
	}

	public bool isLearnableSubPledgeSkill(Skill skill, int subType)
	{
		// academy
		if (subType == -1)
		{
			return false;
		}

		int id = skill.getId();
		Skill? current;
		if (subType == 0)
		{
			current = _subPledgeSkills.get(id);
		}
		else
		{
			current = _subPledges.get(subType)?.getSkill(id);
		}
		// is next level?
		if (current != null && current.getLevel() + 1 == skill.getLevel())
		{
			return true;
		}
		// is first level?
		if (current == null && skill.getLevel() == 1)
		{
			return true;
		}

		return false;
	}

	public List<PledgeSkillListPacket.SubPledgeSkill> getAllSubSkills()
	{
		List<PledgeSkillListPacket.SubPledgeSkill> list = new();
		foreach (Skill skill in _subPledgeSkills.Values)
		{
			list.Add(new PledgeSkillListPacket.SubPledgeSkill(0, skill.getId(), skill.getLevel()));
		}
		foreach (SubPledge subunit in _subPledges.Values)
		{
			foreach (Skill skill in subunit.getSkills())
			{
				list.Add(new PledgeSkillListPacket.SubPledgeSkill(subunit.getId(), skill.getId(), skill.getLevel()));
			}
		}
		return list;
	}

	public void setNewLeaderId(int? objectId, bool storeInDb)
	{
		_newLeaderId = objectId;
		if (storeInDb)
		{
			updateClanInDB();
		}
	}

	public int? getNewLeaderId()
	{
		return _newLeaderId;
	}

	public Player? getNewLeader()
	{
		if (_newLeaderId is null)
			return null;
		return World.getInstance().getPlayer(_newLeaderId.Value);
	}

	public string? getNewLeaderName()
	{
		if (_newLeaderId is null)
			return null;
		return CharInfoTable.getInstance().getNameById(_newLeaderId.Value);
	}

	public int getSiegeKills()
	{
		return _siegeKills;
	}

	public int getSiegeDeaths()
	{
		return _siegeDeaths;
	}

	public int addSiegeKill()
	{
		return Interlocked.Increment(ref _siegeKills);
	}

	public int addSiegeDeath()
	{
		return Interlocked.Increment(ref _siegeDeaths);
	}

	public void clearSiegeKills()
	{
		_siegeKills=0;
	}

	public void clearSiegeDeaths()
	{
		_siegeDeaths=0;
	}

	public int getWarCount()
	{
		return _atWarWith.Count;
	}

	public void addWar(int clanId, ClanWar war)
	{
		_atWarWith.put(clanId, war);
	}

	public void deleteWar(int clanId)
	{
		_atWarWith.remove(clanId);
	}

	public ClanWar? getWarWith(int? clanId)
	{
		if (clanId is null)
			return null;

		return _atWarWith.get(clanId.Value);
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void addMemberOnlineTime(Player player)
	{
		ClanMember? clanMember = getClanMember(player.ObjectId);
		if (clanMember != null)
		{
			clanMember.setOnlineTime(clanMember.getOnlineTime() + TimeSpan.FromMinutes(1));
			if (clanMember.getOnlineTime() == TimeSpan.FromMinutes(30))
			{
				broadcastToOnlineMembers(new PledgeShowMemberListUpdatePacket(clanMember));
			}
		}

		ClanRewardBonus? availableBonus = getAvailableBonus(ClanRewardType.MEMBERS_ONLINE);
		if (availableBonus != null)
		{
			if (_lastMembersOnlineBonus == null)
			{
				_lastMembersOnlineBonus = availableBonus;
				var sm = new SystemMessagePacket(SystemMessageId.YOUR_CLAN_HAS_ACHIEVED_LOGIN_BONUS_LV_S1);
				sm.Params.addByte(availableBonus.getLevel());
				broadcastToOnlineMembers(sm);
			}
			else if (_lastMembersOnlineBonus.getLevel() < availableBonus.getLevel())
			{
				_lastMembersOnlineBonus = availableBonus;
				var sm = new SystemMessagePacket(SystemMessageId.YOUR_CLAN_HAS_ACHIEVED_LOGIN_BONUS_LV_S1);
				sm.Params.addByte(availableBonus.getLevel());
				broadcastToOnlineMembers(sm);
			}
		}

		int currentMaxOnline = 0;
		foreach (ClanMember member in _members.Values)
		{
			if (member.getOnlineTime() > Config.ALT_CLAN_MEMBERS_TIME_FOR_BONUS)
			{
				currentMaxOnline++;
			}
		}
		if (getMaxOnlineMembers() < currentMaxOnline)
		{
			getVariables().set("MAX_ONLINE_MEMBERS", currentMaxOnline);
		}
	}

	/**
	 * @param player
	 * @param target
	 * @param value
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void addHuntingPoints(Player player, Npc target, double value)
	{
		// TODO: Figure out the retail formula
		int points = (int) value / 2960; // Reduced / 10 for Classic.
		if (points > 0)
		{
			getVariables().set("HUNTING_POINTS", getHuntingPoints() + points);
			ClanRewardBonus? availableBonus = getAvailableBonus(ClanRewardType.HUNTING_MONSTERS);
			if (availableBonus != null)
			{
				if (_lastHuntingBonus == null)
				{
					_lastHuntingBonus = availableBonus;
					var sm = new SystemMessagePacket(SystemMessageId.YOUR_CLAN_HAS_ACHIEVED_HUNTING_BONUS_LV_S1);
					sm.Params.addByte(availableBonus.getLevel());
					broadcastToOnlineMembers(sm);
				}
				else if (_lastHuntingBonus.getLevel() < availableBonus.getLevel())
				{
					_lastHuntingBonus = availableBonus;
					var sm = new SystemMessagePacket(SystemMessageId.YOUR_CLAN_HAS_ACHIEVED_HUNTING_BONUS_LV_S1);
					sm.Params.addByte(availableBonus.getLevel());
					broadcastToOnlineMembers(sm);
				}
			}
		}
	}

	public int getMaxOnlineMembers()
	{
		return getVariables().getInt("MAX_ONLINE_MEMBERS", 0);
	}

	public int getHuntingPoints()
	{
		return getVariables().getInt("HUNTING_POINTS", 0);
	}

	public int getPreviousMaxOnlinePlayers()
	{
		return getVariables().getInt("PREVIOUS_MAX_ONLINE_PLAYERS", 0);
	}

	public int getPreviousHuntingPoints()
	{
		return getVariables().getInt("PREVIOUS_HUNTING_POINTS", 0);
	}

	public bool canClaimBonusReward(Player player, ClanRewardType type)
	{
		ClanMember? clanMember = getClanMember(player.ObjectId);
		return clanMember != null && getAvailableBonus(type) != null && !clanMember.isRewardClaimed(type);
	}

	public void resetClanBonus()
	{
		// Save current state
		getVariables().set("PREVIOUS_MAX_ONLINE_PLAYERS", getMaxOnlineMembers());
		getVariables().set("PREVIOUS_HUNTING_POINTS", getHuntingPoints());

		// Reset
		_members.Values.ForEach(x => x.resetBonus());
		getVariables().remove("HUNTING_POINTS");

		// force store
		getVariables().storeMe();

		// Send Packet
		broadcastToOnlineMembers(ExPledgeBonusMarkResetPacket.STATIC_PACKET);
	}

	public ClanVariables getVariables()
	{
		if (_vars == null)
		{
			lock (this)
			{
				if (_vars == null)
				{
					_vars = new ClanVariables(_clanId);
					if (Config.CLAN_VARIABLES_STORE_INTERVAL > 0)
					{
						ThreadPool.scheduleAtFixedRate(storeVariables, Config.CLAN_VARIABLES_STORE_INTERVAL, Config.CLAN_VARIABLES_STORE_INTERVAL);
					}
				}
			}
		}
		return _vars;
	}

	private void storeVariables()
	{
		ClanVariables? vars = _vars;
		if (vars != null)
			vars.storeMe();
	}

	public int getClanContribution(int objId)
	{
		return getVariables().getInt(ClanVariables.CONTRIBUTION + objId, 0);
	}

	public void setClanContribution(int objId, int exp)
	{
		getVariables().set(ClanVariables.CONTRIBUTION + objId, exp);
	}

	public int getClanContributionWeekly(int objId)
	{
		return getVariables().getInt(ClanVariables.CONTRIBUTION_WEEKLY + objId, 0);
	}

	public List<ClanMember> getContributionList()
	{
		return getMembers().Where(it => it.getClan().getClanContribution(it.getObjectId()) != 0).ToList();
	}

	public void setClanContributionWeekly(int objId, int exp)
	{
		getVariables().set(ClanVariables.CONTRIBUTION_WEEKLY + objId, exp);
	}

	public int getExp()
	{
		return _exp;
	}

	public void addExp(int objId, int value)
	{
		if (_exp + value <= ClanLevelData.getInstance().getMaxExp())
		{
			_exp += value;
			broadcastToOnlineMembers(new ExPledgeV3InfoPacket(_exp, getRank(), getNotice(), isNoticeEnabled()));
		}

		int nextLevel = _level + 1;
		if (nextLevel <= ClanLevelData.getInstance().getMaxLevel() && ClanLevelData.getInstance().getLevelExp(nextLevel) <= _exp)
		{
			changeLevel(nextLevel);
		}

		int contribution = getClanContribution(objId);
		setClanContribution(objId, contribution + value);
		setClanContributionWeekly(objId, contribution + value);
	}

	public void setExp(int objId, int value)
	{
		_exp = value;
		broadcastToOnlineMembers(new ExPledgeV3InfoPacket(_exp, getRank(), getNotice(), isNoticeEnabled()));

		int contribution = getClanContribution(objId);
		setClanContribution(objId, contribution + value);
		setClanContributionWeekly(objId, contribution + value);

		updateClanInDB();
	}

	private ClanRewardBonus? getAvailableBonus(ClanRewardType rewardType)
	{
		int currentAmount = rewardType switch
		{
			ClanRewardType.MEMBERS_ONLINE => getPreviousMaxOnlinePlayers(),
			ClanRewardType.HUNTING_MONSTERS => getPreviousHuntingPoints(),
			_ => int.MaxValue
		};

		ClanRewardBonus? availableBonus = null;
		foreach (ClanRewardBonus bonus in ClanRewardData.getInstance().getClanRewardBonuses(rewardType))
		{
			if (bonus.getRequiredAmount() <= currentAmount)
			{
				if (availableBonus == null || availableBonus.getLevel() < bonus.getLevel())
				{
					availableBonus = bonus;
				}
			}
		}

		return availableBonus;
	}
}