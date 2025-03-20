using L2Dn.Events;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Events.Impl.Playables;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Model.Enums;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.Actor.Stats;

public class PlayableStat: CreatureStat
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(PlayableStat));

	public PlayableStat(Playable player): base(player)
	{
	}

	public virtual bool addExp(long amount)
	{
		EventContainer charEvents = getActiveChar().Events;
		if (charEvents.HasSubscribers<OnPlayableExpChanged>())
		{
			OnPlayableExpChanged onPlayableExpChanged = new(getActiveChar(), getExp(), getExp() + amount);
			if (charEvents.Notify(onPlayableExpChanged) && onPlayableExpChanged.Terminate)
			{
				return false;
			}
		}

		if (getExp() + amount < 0 || (amount > 0 && getExp() == getExpForLevel(getMaxLevel()) - 1))
		{
			return true;
		}

		long value = amount;
		if (getExp() + value >= getExpForLevel(getMaxLevel()))
		{
			value = getExpForLevel(getMaxLevel()) - 1 - getExp();
		}

		int oldLevel = getLevel();
		setExp(getExp() + value);
		int minimumLevel = 1;
		if (getActiveChar().isPet())
		{
			// get minimum level from NpcTemplate
			minimumLevel = PetDataTable.getInstance().getPetMinLevel(((Pet) getActiveChar()).getTemplate().Id);
		}

		int level = minimumLevel; // minimum level
		for (int tmp = level; tmp <= getMaxLevel(); tmp++)
		{
			if (getExp() >= getExpForLevel(tmp))
			{
				continue;
			}
			level = --tmp;
			break;
		}

		if (level != getLevel() && level >= minimumLevel)
		{
			addLevel(level - getLevel());
		}

		int newLevel = getLevel();
        Player? player = getActiveChar().getActingPlayer();
		if (newLevel > oldLevel && getActiveChar().isPlayer() && player != null)
		{
			if (SkillTreeData.getInstance().hasAvailableSkills(player, player.getClassId()))
			{
				getActiveChar().sendPacket(ExNewSkillToLearnByLevelUpPacket.STATIC_PACKET);
			}

			// Check last rewarded level - prevent reputation farming via deleveling
			int lastPledgedLevel = player.getVariables().Get(PlayerVariables.LAST_PLEDGE_REPUTATION_LEVEL, 0);
			if (lastPledgedLevel < newLevel)
			{
				int leveledUpCount = newLevel - lastPledgedLevel;
				addReputationToClanBasedOnLevel(player, leveledUpCount);

				player.getVariables().Set(PlayerVariables.LAST_PLEDGE_REPUTATION_LEVEL, newLevel);
			}
		}

		return true;
	}

	public bool removeExp(long amount)
	{
		long value = amount;
		if (getExp() - value < getExpForLevel(getLevel()) && (!Config.Character.PLAYER_DELEVEL || (Config.Character.PLAYER_DELEVEL && getLevel() <= Config.Character.DELEVEL_MINIMUM)))
		{
			value = getExp() - getExpForLevel(getLevel());
		}

		if (getExp() - value < 0)
		{
			value = getExp() - 1;
		}

		setExp(getExp() - value);
		int minimumLevel = 1;
		if (getActiveChar().isPet())
		{
			// get minimum level from NpcTemplate
			minimumLevel = PetDataTable.getInstance().getPetMinLevel(((Pet) getActiveChar()).getTemplate().Id);
		}
		int level = minimumLevel;
		for (int tmp = level; tmp <= getMaxLevel(); tmp++)
		{
			if (getExp() >= getExpForLevel(tmp))
			{
				continue;
			}
			level = --tmp;
			break;
		}
		if (level != getLevel() && level >= minimumLevel)
		{
			addLevel(level - getLevel());
		}
		return true;
	}

	public virtual bool removeExpAndSp(long exp, long sp)
	{
		bool expRemoved = false;
		bool spRemoved = false;
		if (exp > 0)
		{
			expRemoved = removeExp(exp);
		}
		if (sp > 0)
		{
			spRemoved = removeSp(sp);
		}

		return expRemoved || spRemoved;
	}

	public virtual bool addLevel(int amount)
	{
		int value = amount;
		if (getLevel() + value > getMaxLevel() - 1)
		{
			if (getLevel() < getMaxLevel() - 1)
			{
				value = getMaxLevel() - 1 - getLevel();
			}
			else
			{
				return false;
			}
		}

		bool levelIncreased = getLevel() + value > getLevel();
		value += getLevel();
		setLevel(value);

		// Sync up exp with current level
		if (getExp() >= getExpForLevel(getLevel() + 1) || getExpForLevel(getLevel()) > getExp())
		{
			setExp(getExpForLevel(getLevel()));
		}

		if (!levelIncreased && getActiveChar() is Player player && !player.isGM() && Config.Character.DECREASE_SKILL_LEVEL)
		{
			player.checkPlayerSkills();
		}

		if (!levelIncreased)
		{
			return false;
		}

		getActiveChar().getStatus().setCurrentHp(getActiveChar().getStat().getMaxHp());
		getActiveChar().getStatus().setCurrentMp(getActiveChar().getStat().getMaxMp());

		return true;
	}

	public virtual bool addSp(long amount)
	{
		if (amount < 0)
		{
			LOGGER.Warn("wrong usage");
			return false;
		}

		long currentSp = getSp();
		if (currentSp >= Config.Character.MAX_SP)
		{
			return false;
		}

		long value = amount;
		if (currentSp > Config.Character.MAX_SP - value)
		{
			value = Config.Character.MAX_SP - currentSp;
		}

		setSp(currentSp + value);
		return true;
	}

	public bool removeSp(long amount)
	{
		long currentSp = getSp();
		if (currentSp < amount)
		{
			setSp(getSp() - currentSp);
			return true;
		}
		setSp(getSp() - amount);
		return true;
	}

	public virtual long getExpForLevel(int level)
	{
		return ExperienceData.getInstance().getExpForLevel(level);
	}

	public override Playable getActiveChar()
	{
		return (Playable)base.getActiveChar();
	}

	public virtual int getMaxLevel()
	{
		return ExperienceData.getInstance().getMaxLevel();
	}

	public override int getPhysicalAttackRadius()
	{
		Weapon? weapon = getActiveChar().getActiveWeaponItem();
		return weapon != null ? weapon.getBaseAttackRadius() : base.getPhysicalAttackRadius();
	}

	public override int getPhysicalAttackAngle()
	{
		Weapon? weapon = getActiveChar().getActiveWeaponItem();
		return weapon != null ? weapon.getBaseAttackAngle() + (int) getActiveChar().getStat().getValue(Stat.WEAPON_ATTACK_ANGLE_BONUS, 0) : base.getPhysicalAttackAngle();
	}

	private void addReputationToClanBasedOnLevel(Player player, int leveledUpCount)
	{
		Clan? clan = player.getClan();
		if (clan == null)
		{
			return;
		}

		if (clan.getLevel() < 3) // When a character from clan level 3 or above increases its level, CRP are added
		{
			return;
		}

		int reputation = 0;
		for (int i = 0; i < leveledUpCount; i++)
		{
			int level = player.getLevel() - i;
			if (level >= 20 && level <= 25)
			{
				reputation += Config.Feature.LVL_UP_20_AND_25_REP_SCORE;
			}
			else if (level >= 26 && level <= 30)
			{
				reputation += Config.Feature.LVL_UP_26_AND_30_REP_SCORE;
			}
			else if (level >= 31 && level <= 35)
			{
				reputation += Config.Feature.LVL_UP_31_AND_35_REP_SCORE;
			}
			else if (level >= 36 && level <= 40)
			{
				reputation += Config.Feature.LVL_UP_36_AND_40_REP_SCORE;
			}
			else if (level >= 41 && level <= 45)
			{
				reputation += Config.Feature.LVL_UP_41_AND_45_REP_SCORE;
			}
			else if (level >= 46 && level <= 50)
			{
				reputation += Config.Feature.LVL_UP_46_AND_50_REP_SCORE;
			}
			else if (level >= 51 && level <= 55)
			{
				reputation += Config.Feature.LVL_UP_51_AND_55_REP_SCORE;
			}
			else if (level >= 56 && level <= 60)
			{
				reputation += Config.Feature.LVL_UP_56_AND_60_REP_SCORE;
			}
			else if (level >= 61 && level <= 65)
			{
				reputation += Config.Feature.LVL_UP_61_AND_65_REP_SCORE;
			}
			else if (level >= 66 && level <= 70)
			{
				reputation += Config.Feature.LVL_UP_66_AND_70_REP_SCORE;
			}
			else if (level >= 71 && level <= 75)
			{
				reputation += Config.Feature.LVL_UP_71_AND_75_REP_SCORE;
			}
			else if (level >= 76 && level <= 80)
			{
				reputation += Config.Feature.LVL_UP_76_AND_80_REP_SCORE;
			}
			else if (level >= 81 && level <= 90)
			{
				reputation += Config.Feature.LVL_UP_81_AND_90_REP_SCORE;
			}
			else if (level >= 91 && level <= 120)
			{
				reputation += Config.Feature.LVL_UP_91_PLUS_REP_SCORE;
			}
		}

		if (reputation == 0)
		{
			return;
		}

		reputation = (int) Math.Ceiling(reputation * Config.Feature.LVL_OBTAINED_REP_SCORE_MULTIPLIER);

		clan.addReputationScore(reputation);

		foreach (ClanMember member in clan.getMembers())
		{
			if (member.isOnline())
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.CLAN_REPUTATION_POINTS_S1);
				sm.Params.addInt(reputation);
				member.getPlayer()?.sendPacket(sm);
			}
		}
	}
}