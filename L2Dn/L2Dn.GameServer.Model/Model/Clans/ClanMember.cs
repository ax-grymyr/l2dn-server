using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network;
using L2Dn.Model;
using L2Dn.Model.Enums;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.Model.Clans;

/**
 * This class holds the clan members data.
 */
public class ClanMember
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ClanMember));

	private readonly Clan _clan;
	private int _objectId;
	private string _name;
	private string _title;
	private int _powerGrade;
	private int _level;
	private CharacterClass _classId;
	private Sex _sex;
	private Race _race;
	private Player _player;
	private int _pledgeType;
	private int _apprentice;
	private int? _sponsor;
	private TimeSpan _onlineTime;

	/**
	 * Used to restore a clan member from the database.
	 * @param clan the clan where the clan member belongs.
	 * @param clanMember the clan member result set
	 * @throws SQLException if the columnLabel is not valid or a database error occurs
	 */
	public ClanMember(Clan clan, Character clanMember)
	{
		if (clan == null)
			throw new ArgumentNullException(nameof(clan),"Cannot create a Clan Member with a null clan.");

		_clan = clan;
		_name = clanMember.Name;
		_level = clanMember.Level;
		_classId = clanMember.Class;
		_objectId = clanMember.Id;
		_pledgeType = clanMember.SubPledge;
		_title = clanMember.Title ?? string.Empty;
		_powerGrade = clanMember.PowerGrade;
		_apprentice = clanMember.Apprentice;
		_sponsor = clanMember.SponsorId;
		_sex = clanMember.Sex;
		_race = clanMember.Class.GetRace();
	}

	/**
	 * Creates a clan member from a player instance.
	 * @param clan the clan where the player belongs
	 * @param player the player from which the clan member will be created
	 */
	public ClanMember(Clan clan, Player player)
	{
		if (clan == null)
		{
			throw new ArgumentNullException(nameof(clan),"Cannot create a Clan Member with a null clan.");
		}
		_player = player;
		_clan = clan;
		_name = player.getName();
		_level = player.getLevel();
		_classId = player.getClassId();
		_objectId = player.ObjectId;
		_pledgeType = player.getPledgeType();
		_powerGrade = player.getPowerGrade();
		_title = player.getTitle();
		_sponsor = 0;
		_apprentice = 0;
		_sex = player.getAppearance().getSex();
		_race = player.getRace();
	}

	/**
	 * Sets the player instance.
	 * @param player the new player instance
	 */
	public void setPlayer(Player player)
	{
		if (player == null && _player != null)
		{
			// this is here to keep the data when the player logs off
			_name = _player.getName();
			_level = _player.getLevel();
			_classId = _player.getClassId();
			_objectId = _player.ObjectId;
			_powerGrade = _player.getPowerGrade();
			_pledgeType = _player.getPledgeType();
			_title = _player.getTitle();
			_apprentice = _player.getApprentice();
			_sponsor = _player.getSponsor();
			_sex = _player.getAppearance().getSex();
			_race = _player.getRace();
		}

		if (player != null)
		{
			_clan.addSkillEffects(player);
			if (_clan.getLevel() > 3 && player.isClanLeader())
			{
				SiegeManager.getInstance().addSiegeSkills(player);
			}
			if (player.isClanLeader())
			{
				_clan.setLeader(this);
			}
		}

		_player = player;
	}

	/**
	 * Gets the player instance.
	 * @return the player instance
	 */
	public Player getPlayer()
	{
		return _player;
	}

	/**
	 * Verifies if the clan member is online.
	 * @return {@code true} if is online
	 */
	public bool isOnline()
	{
		if (_player == null || !_player.isOnline())
		{
			return false;
		}

        GameSession? client = _player.getClient();
		if (client == null || client.IsDetached)
		{
			return false;
		}
		return true;
	}

	/**
	 * Gets the class id.
	 * @return the classId
	 */
	public CharacterClass getClassId()
	{
		return _player != null ? _player.getClassId() : _classId;
	}

	/**
	 * Gets the level.
	 * @return the level
	 */
	public int getLevel()
	{
		return _player != null ? _player.getLevel() : _level;
	}

	/**
	 * Gets the name.
	 * @return the name
	 */
	public string getName()
	{
		return _player != null ? _player.getName() : _name;
	}

	/**
	 * Gets the object id.
	 * @return Returns the objectId.
	 */
	public int getObjectId()
	{
		return _player != null ? _player.ObjectId : _objectId;
	}

	/**
	 * Gets the title.
	 * @return the title
	 */
	public string getTitle()
	{
		return _player != null ? _player.getTitle() : _title;
	}

	/**
	 * Gets the pledge type.
	 * @return the pledge type
	 */
	public int getPledgeType()
	{
		return _player != null ? _player.getPledgeType() : _pledgeType;
	}

	/**
	 * Sets the pledge type.
	 * @param pledgeType the new pledge type
	 */
	public void setPledgeType(int pledgeType)
	{
		_pledgeType = pledgeType;
		if (_player != null)
		{
			_player.setPledgeType(pledgeType);
		}
		else
		{
			// db save if char not logged in
			updatePledgeType();
		}
	}

	/**
	 * Update pledge type.
	 */
	public void updatePledgeType()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = getObjectId();
			ctx.Characters.Where(c => c.Id == characterId)
				.ExecuteUpdate(s => s.SetProperty(c => c.SubPledge, _pledgeType));
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not update pledge type: " + e);
		}
	}

	/**
	 * Gets the power grade.
	 * @return the power grade
	 */
	public int getPowerGrade()
	{
		return _player != null ? _player.getPowerGrade() : _powerGrade;
	}

	/**
	 * Sets the power grade.
	 * @param powerGrade the new power grade
	 */
	public void setPowerGrade(int powerGrade)
	{
		_powerGrade = powerGrade;
		if (_player != null)
		{
			_player.setPowerGrade(powerGrade);
		}
		else
		{
			// db save if char not logged in
			updatePowerGrade();
		}
	}

	/**
	 * Update the characters table of the database with power grade.
	 */
	public void updatePowerGrade()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = getObjectId();
			ctx.Characters.Where(c => c.Id == characterId)
				.ExecuteUpdate(s => s.SetProperty(c => c.PowerGrade, _powerGrade));
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not update power _grade: " + e);
		}
	}

	/**
	 * Sets the apprentice and sponsor.
	 * @param apprenticeID the apprentice id
	 * @param sponsorID the sponsor id
	 */
	public void setApprenticeAndSponsor(int apprenticeID, int sponsorID)
	{
		_apprentice = apprenticeID;
		_sponsor = sponsorID;
	}

	/**
	 * Gets the player's race ordinal.
	 * @return the race ordinal
	 */
	public Race getRace()
	{
		return _player != null ? _player.getRace() : _race;
	}

	/**
	 * Gets the player's sex.
	 * @return the sex
	 */
	public Sex getSex()
	{
		return _player != null ? _player.getAppearance().getSex() : _sex;
	}

	/**
	 * Gets the sponsor.
	 * @return the sponsor
	 */
	public int? getSponsor()
	{
		return _player != null ? _player.getSponsor() : _sponsor;
	}

	/**
	 * Gets the apprentice.
	 * @return the apprentice
	 */
	public int getApprentice()
	{
		return _player != null ? _player.getApprentice() : _apprentice;
	}

	/**
	 * Gets the apprentice or sponsor name.
	 * @return the apprentice or sponsor name
	 */
	public string getApprenticeOrSponsorName()
	{
		if (_player != null)
		{
			_apprentice = _player.getApprentice();
			_sponsor = _player.getSponsor();
		}

		if (_apprentice != 0)
		{
			ClanMember? apprentice = _clan.getClanMember(_apprentice);
			if (apprentice != null)
			{
				return apprentice.getName();
			}
			return "Error";
		}

		if (_sponsor != null)
		{
			ClanMember? sponsor = _clan.getClanMember(_sponsor.Value);
			if (sponsor != null)
			{
				return sponsor.getName();
			}
			return "Error";
		}
		return "";
	}

	/**
	 * Gets the clan.
	 * @return the clan
	 */
	public Clan getClan()
	{
		return _clan;
	}

	/**
	 * Calculate pledge class.
	 * @param player the player
	 * @return the int
	 */
	public static SocialClass calculatePledgeClass(Player player)
	{
		SocialClass pledgeClass = 0;
		if (player == null)
		{
			return pledgeClass;
		}

		Clan? clan = player.getClan();
		if (clan != null)
		{
			switch (clan.getLevel())
			{
				case 4:
				{
					if (player.isClanLeader())
					{
						pledgeClass = SocialClass.HEIR;
					}
					break;
				}
				case 5:
				{
					if (player.isClanLeader())
					{
						pledgeClass = SocialClass.KNIGHT;
					}
					else
					{
						pledgeClass = SocialClass.APPRENTICE;
					}
					break;
				}
				case 6:
				{
					switch (player.getPledgeType())
					{
						case -1:
						{
							pledgeClass = SocialClass.VASSAL;
							break;
						}
						case 100:
						case 200:
						{
							pledgeClass = SocialClass.APPRENTICE;
							break;
						}
						case 0:
						{
							if (player.isClanLeader())
							{
								pledgeClass = SocialClass.ELDER;
							}
							else
							{
								switch (clan.getLeaderSubPledge(player.ObjectId))
								{
									case 100:
									case 200:
									{
										pledgeClass = SocialClass.KNIGHT;
										break;
									}
									case -1:
									default:
									{
										pledgeClass = SocialClass.HEIR;
										break;
									}
								}
							}
							break;
						}
					}
					break;
				}
				case 7:
				{
					switch (player.getPledgeType())
					{
						case -1:
						{
							pledgeClass = SocialClass.VASSAL;
							break;
						}
						case 100:
						case 200:
						{
							pledgeClass = SocialClass.HEIR;
							break;
						}
						case 1001:
						case 1002:
						case 2001:
						case 2002:
						{
							pledgeClass = SocialClass.APPRENTICE;
							break;
						}
						case 0:
						{
							if (player.isClanLeader())
							{
								pledgeClass = SocialClass.VISCOUNT;
							}
							else
							{
								switch (clan.getLeaderSubPledge(player.ObjectId))
								{
									case 100:
									case 200:
									{
										pledgeClass = SocialClass.BARON;
										break;
									}
									case 1001:
									case 1002:
									case 2001:
									case 2002:
									{
										pledgeClass = SocialClass.ELDER;
										break;
									}
									case -1:
									default:
									{
										pledgeClass = SocialClass.KNIGHT;
										break;
									}
								}
							}
							break;
						}
					}
					break;
				}
				case 8:
				{
					switch (player.getPledgeType())
					{
						case -1:
						{
							pledgeClass = SocialClass.VASSAL;
							break;
						}
						case 100:
						case 200:
						{
							pledgeClass = SocialClass.KNIGHT;
							break;
						}
						case 1001:
						case 1002:
						case 2001:
						case 2002:
						{
							pledgeClass = SocialClass.HEIR;
							break;
						}
						case 0:
						{
							if (player.isClanLeader())
							{
								pledgeClass = SocialClass.COUNT;
							}
							else
							{
								switch (clan.getLeaderSubPledge(player.ObjectId))
								{
									case 100:
									case 200:
									{
										pledgeClass = SocialClass.VISCOUNT;
										break;
									}
									case 1001:
									case 1002:
									case 2001:
									case 2002:
									{
										pledgeClass = SocialClass.BARON;
										break;
									}
									case -1:
									default:
									{
										pledgeClass = SocialClass.ELDER;
										break;
									}
								}
							}
							break;
						}
					}
					break;
				}
				case 9:
				{
					switch (player.getPledgeType())
					{
						case -1:
						{
							pledgeClass = SocialClass.VASSAL;
							break;
						}
						case 100:
						case 200:
						{
							pledgeClass = SocialClass.ELDER;
							break;
						}
						case 1001:
						case 1002:
						case 2001:
						case 2002:
						{
							pledgeClass = SocialClass.KNIGHT;
							break;
						}
						case 0:
						{
							if (player.isClanLeader())
							{
								pledgeClass = SocialClass.MARQUIS;
							}
							else
							{
								switch (clan.getLeaderSubPledge(player.ObjectId))
								{
									case 100:
									case 200:
									{
										pledgeClass = SocialClass.COUNT;
										break;
									}
									case 1001:
									case 1002:
									case 2001:
									case 2002:
									{
										pledgeClass = SocialClass.VISCOUNT;
										break;
									}
									case -1:
									default:
									{
										pledgeClass = SocialClass.BARON;
										break;
									}
								}
							}
							break;
						}
					}
					break;
				}
				case 10:
				{
					switch (player.getPledgeType())
					{
						case -1:
						{
							pledgeClass = SocialClass.VASSAL;
							break;
						}
						case 100:
						case 200:
						{
							pledgeClass = SocialClass.BARON;
							break;
						}
						case 1001:
						case 1002:
						case 2001:
						case 2002:
						{
							pledgeClass = SocialClass.ELDER;
							break;
						}
						case 0:
						{
							if (player.isClanLeader())
							{
								pledgeClass = SocialClass.DUKE;
							}
							else
							{
								switch (clan.getLeaderSubPledge(player.ObjectId))
								{
									case 100:
									case 200:
									{
										pledgeClass = SocialClass.MARQUIS;
										break;
									}
									case 1001:
									case 1002:
									case 2001:
									case 2002:
									{
										pledgeClass = SocialClass.COUNT;
										break;
									}
									case -1:
									default:
									{
										pledgeClass = SocialClass.VISCOUNT;
										break;
									}
								}
							}
							break;
						}
					}
					break;
				}
				case 11:
				{
					switch (player.getPledgeType())
					{
						case -1:
						{
							pledgeClass = SocialClass.VASSAL;
							break;
						}
						case 100:
						case 200:
						{
							pledgeClass = SocialClass.VISCOUNT;
							break;
						}
						case 1001:
						case 1002:
						case 2001:
						case 2002:
						{
							pledgeClass = SocialClass.BARON;
							break;
						}
						case 0:
						{
							if (player.isClanLeader())
							{
								pledgeClass = SocialClass.GRAND_DUKE;
							}
							else
							{
								switch (clan.getLeaderSubPledge(player.ObjectId))
								{
									case 100:
									case 200:
									{
										pledgeClass = SocialClass.DUKE;
										break;
									}
									case 1001:
									case 1002:
									case 2001:
									case 2002:
									{
										pledgeClass = SocialClass.MARQUIS;
										break;
									}
									case -1:
									default:
									{
										pledgeClass = SocialClass.COUNT;
										break;
									}
								}
							}
							break;
						}
					}
					break;
				}
				default:
				{
					pledgeClass = SocialClass.VASSAL;
					break;
				}
			}
		}

		if (player.isNoble() && pledgeClass < SocialClass.ELDER)
		{
			pledgeClass = SocialClass.ELDER;
		}

		if (player.isHero() && pledgeClass < SocialClass.COUNT)
		{
			pledgeClass = SocialClass.COUNT;
		}

		return pledgeClass;
	}

	/**
	 * Save apprentice and sponsor.
	 * @param apprentice the apprentice
	 * @param sponsor the sponsor
	 */
	public void saveApprenticeAndSponsor(int apprentice, int? sponsor)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = getObjectId();
			ctx.Characters.Where(c => c.Id == characterId)
				.ExecuteUpdate(s =>
					s.SetProperty(c => c.Apprentice, apprentice).SetProperty(c => c.SponsorId, sponsor));
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not save apprentice/sponsor: " + e);
		}
	}

	public TimeSpan getOnlineTime()
	{
		return _onlineTime;
	}

	public void setOnlineTime(TimeSpan onlineTime)
	{
		_onlineTime = onlineTime;
	}

	public void resetBonus()
	{
		_onlineTime = TimeSpan.Zero;
		PlayerVariables vars = getVariables();
		vars.set("CLAIMED_CLAN_REWARDS", (int)ClanRewardType.None);
		vars.storeMe();
	}

	public int getOnlineStatus()
	{
		return !isOnline() ? 0 : _onlineTime >= Config.ALT_CLAN_MEMBERS_TIME_FOR_BONUS ? 2 : 1;
	}

	public bool isRewardClaimed(ClanRewardType type)
	{
		PlayerVariables vars = getVariables();
		ClanRewardType claimedRewards = (ClanRewardType)vars.getInt("CLAIMED_CLAN_REWARDS", (int)ClanRewardType.All);
		return (claimedRewards & type) == type;
	}

	public void setRewardClaimed(ClanRewardType type)
	{
		PlayerVariables vars = getVariables();
		ClanRewardType claimedRewards = (ClanRewardType)vars.getInt("CLAIMED_CLAN_REWARDS", (int)ClanRewardType.All);
		claimedRewards |= type;
		vars.set("CLAIMED_CLAN_REWARDS", (int)claimedRewards);
		vars.storeMe();
	}

	private PlayerVariables getVariables()
	{
		return _player != null ? _player.getVariables() : new PlayerVariables(_objectId);
	}
}