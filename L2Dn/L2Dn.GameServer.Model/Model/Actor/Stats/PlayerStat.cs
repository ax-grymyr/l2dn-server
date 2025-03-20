using System.Runtime.CompilerServices;
using L2Dn.Extensions;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Transforms;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.DailyMissions;
using L2Dn.GameServer.Network.OutgoingPackets.Friends;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.Actor.Stats;

public class PlayerStat: PlayableStat
{
	private long _startingXp;
	private int _talismanSlots;
	private bool _cloakSlot;
	private int _vitalityPoints;

	public const int MAX_VITALITY_POINTS = 3500000;
	public const int MIN_VITALITY_POINTS = 0;

	private const int FANCY_FISHING_ROD_SKILL = 21484;

	public PlayerStat(Player player): base(player)
	{
	}

	public override bool addExp(long value)
	{
		Player player = getActiveChar();

		// Allowed to gain exp?
		if (!player.getAccessLevel().CanGainExp)
		{
			return false;
		}

		if (!base.addExp(value))
		{
			return false;
		}

		// Set new karma
		if (!player.isCursedWeaponEquipped() && player.getReputation() < 0 && (player.isGM() || !player.isInsideZone(ZoneId.PVP)))
		{
			int karmaLost = Formulas.calculateKarmaLost(player, value);
			if (karmaLost > 0)
			{
				player.setReputation(Math.Min(player.getReputation() + karmaLost, 0));
			}
		}

		// EXP status update currently not used in retail
		player.updateUserInfo();
		return true;
	}

	public void addExpAndSp(double addToExpValue, double addToSpValue, bool useBonuses)
	{
		Player player = getActiveChar();

		// Allowed to gain exp/sp?
		if (!player.getAccessLevel().CanGainExp)
		{
			return;
		}

		double addToExp = addToExpValue;
		double addToSp = addToSpValue;

		double baseExp = addToExp;
		double baseSp = addToSp;
		double bonusExp = 1;
		double bonusSp = 1;
		if (useBonuses)
		{
			if (player.isFishing())
			{
				// rod fishing skills
				Item? rod = player.getActiveWeaponInstance();
				if (rod != null && rod.getItemType() == WeaponType.FISHINGROD && rod.getTemplate().getAllSkills() != null)
				{
					foreach (ItemSkillHolder s in rod.getTemplate().getAllSkills())
					{
						if (s.getSkill().Id == FANCY_FISHING_ROD_SKILL)
						{
							bonusExp *= 1.5;
							bonusSp *= 1.5;
						}
					}
				}
			}
			else
			{
				bonusExp = getExpBonusMultiplier();
				bonusSp = getSpBonusMultiplier();
			}
		}

		addToExp *= bonusExp;
		addToSp *= bonusSp;
		double ratioTakenByPlayer = 0;

		// if this player has a pet and it is in his range he takes from the owner's Exp, give the pet Exp now
		Summon? sPet = player.getPet();
		if (sPet != null && Util.checkIfInShortRange(Config.Character.ALT_PARTY_RANGE, player, sPet, false))
		{
			Pet pet = (Pet) sPet;
			ratioTakenByPlayer = pet.getPetLevelData().getOwnerExpTaken() / 100f;

			// only give exp/sp to the pet by taking from the owner if the pet has a non-zero, positive ratio
			// allow possible customizations that would have the pet earning more than 100% of the owner's exp/sp
			if (ratioTakenByPlayer > 1)
			{
				ratioTakenByPlayer = 1;
			}

			if (!pet.isDead())
			{
				pet.addExpAndSp(addToExp * (1 - ratioTakenByPlayer), addToSp * (1 - ratioTakenByPlayer));
			}

			// now adjust the max ratio to avoid the owner earning negative exp/sp
			addToExp *= ratioTakenByPlayer;
			addToSp *= ratioTakenByPlayer;
		}

		long finalExp = (long)addToExp;
		long finalSp = (long)addToSp;
		bool expAdded = addExp(finalExp);
		bool spAdded = addSp(finalSp);
		if (!expAdded && spAdded)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_ACQUIRED_S1_SP);
			sm.Params.addLong(finalSp);
			player.sendPacket(sm);
		}
		else if (expAdded && !spAdded)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_ACQUIRED_S1_XP);
			sm.Params.addLong(finalExp);
			player.sendPacket(sm);
		}
		else if (finalExp > 0 || finalSp > 0)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_ACQUIRED_S1_XP_BONUS_S2_AND_S3_SP_BONUS_S4);
			sm.Params.addLong(finalExp);
			sm.Params.addLong((long)(addToExp - baseExp));
			sm.Params.addLong(finalSp);
			sm.Params.addLong((long)(addToSp - baseSp));
			player.sendPacket(sm);
		}
	}

	public override bool removeExpAndSp(long addToExp, long addToSp)
	{
		return removeExpAndSp(addToExp, addToSp, true);
	}

	public bool removeExpAndSp(long addToExp, long addToSp, bool sendMessage)
	{
		int level = getLevel();
		if (!base.removeExpAndSp(addToExp, addToSp))
		{
			return false;
		}

		if (sendMessage)
		{
			// Send a Server->Client System Message to the Player
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOUR_XP_HAS_DECREASED_BY_S1);
			sm.Params.addLong(addToExp);
			getActiveChar().sendPacket(sm);
			sm = new SystemMessagePacket(SystemMessageId.YOUR_SP_HAS_DECREASED_BY_S1);
			sm.Params.addLong(addToSp);
			getActiveChar().sendPacket(sm);
			if (getLevel() < level)
			{
				getActiveChar().broadcastStatusUpdate();
			}
		}
		return true;
	}

	public override bool addLevel(int value)
	{
		if (getLevel() + value > ExperienceData.getInstance().getMaxLevel() - 1)
		{
			return false;
		}

		bool levelIncreased = base.addLevel(value);
		if (levelIncreased)
		{
			getActiveChar().setCurrentCp(getMaxCp());
			getActiveChar().broadcastPacket(new SocialActionPacket(getActiveChar().ObjectId, SocialActionPacket.LEVEL_UP));
			getActiveChar().sendPacket(SystemMessageId.YOUR_LEVEL_HAS_INCREASED);
			getActiveChar().notifyFriends(FriendStatusPacket.MODE_LEVEL);
		}

		// Notify to scripts
		if (getActiveChar().Events.HasSubscribers<OnPlayerLevelChanged>())
		{
			getActiveChar().Events.NotifyAsync(new OnPlayerLevelChanged(getActiveChar(), getLevel() - value, getLevel()));
		}

		// Update daily mission count.
		getActiveChar().sendPacket(new ExConnectedTimeAndGettableRewardPacket(getActiveChar()));

		// Give AutoGet skills and all normal skills if Auto-Learn is activated.
		getActiveChar().rewardSkills();

        Clan? activeCharClan = getActiveChar().getClan();
		if (activeCharClan != null)
		{
            activeCharClan.updateClanMember(getActiveChar());
            activeCharClan.broadcastToOnlineMembers(new PledgeShowMemberListUpdatePacket(getActiveChar()));
		}

        Party? party = getActiveChar().getParty();
		if (getActiveChar().isInParty() && party != null)
		{
			party.recalculatePartyLevel(); // Recalculate the party level
		}

		// Maybe add some skills when player levels up in transformation.
		Transform? transform = getActiveChar().getTransformation();
		if (transform is not null)
			transform.onLevelUp(getActiveChar());

		// Synchronize level with pet if possible.
		Summon? sPet = getActiveChar().getPet();
		if (sPet != null)
		{
			Pet pet = (Pet) sPet;
			if (pet.getPetData().isSynchLevel() && pet.getLevel() != getLevel())
			{
				int availableLevel = Math.Min(pet.getPetData().getMaxLevel(), getLevel());
				pet.getStat().setLevel(availableLevel);
				pet.getStat().getExpForLevel(availableLevel);
				pet.setCurrentHp(pet.getMaxHp());
				pet.setCurrentMp(pet.getMaxMp());
				pet.broadcastPacket(new SocialActionPacket(getActiveChar().ObjectId, SocialActionPacket.LEVEL_UP));
				pet.updateAndBroadcastStatus(1);
			}
		}

		getActiveChar().broadcastStatusUpdate();

		// Update the overloaded status of the Player
		getActiveChar().refreshOverloaded(true);

		// Send a Server->Client packet UserInfo to the Player
		getActiveChar().updateUserInfo();

		// Send acquirable skill list
		getActiveChar().sendPacket(new AcquireSkillListPacket(getActiveChar()));
		getActiveChar().sendPacket(new ExVoteSystemInfoPacket(getActiveChar()));
		getActiveChar().sendPacket(new ExOneDayReceiveRewardListPacket(getActiveChar(), true));
		return levelIncreased;
	}

	public override bool addSp(long value)
	{
		if (!base.addSp(value))
		{
			return false;
		}

		getActiveChar().broadcastUserInfo(UserInfoType.CURRENT_HPMPCP_EXP_SP);

		return true;
	}

	public override long getExpForLevel(int level)
	{
		return ExperienceData.getInstance().getExpForLevel(level);
	}

	public override Player getActiveChar()
	{
		return (Player)base.getActiveChar();
	}

	public override long getExp()
	{
		if (getActiveChar().isSubClassActive())
		{
            // TODO: null checking hack, subclasses must be handled differently
            SubClassHolder subclass = getActiveChar().getSubClasses().get(getActiveChar().getClassIndex()) ??
                throw new InvalidOperationException("Subsclass is active, but subclass holder is null");

			return subclass.getExp();
		}

		return base.getExp();
	}

	public long getBaseExp()
	{
		return base.getExp();
	}

	public override void setExp(long value)
	{
		if (getActiveChar().isSubClassActive())
		{
            // TODO: null checking hack, subclasses must be handled differently
            SubClassHolder subclass = getActiveChar().getSubClasses().get(getActiveChar().getClassIndex()) ??
                throw new InvalidOperationException("Subsclass is active, but subclass holder is null");

			subclass.setExp(value);
		}
		else
		{
			base.setExp(value);
		}
	}

	public void setStartingExp(long value)
	{
		if (Config.General.BOTREPORT_ENABLE)
		{
			_startingXp = value;
		}
	}

	public long getStartingExp()
	{
		return _startingXp;
	}

	/**
	 * Gets the maximum talisman count.
	 * @return the maximum talisman count
	 */
	public int getTalismanSlots()
	{
		if (!getActiveChar().hasEnteredWorld())
		{
			return 6; // TODO: dirty hack, slots must be handled differently
		}

		return _talismanSlots;
	}

	public void addTalismanSlots(int count)
	{
		Interlocked.Add(ref _talismanSlots, count);
	}

	public bool canEquipCloak()
	{
		return _cloakSlot;
	}

	public void setCloakSlotStatus(bool cloakSlot)
	{
		_cloakSlot = cloakSlot;
	}

	public override int getLevel()
    {
        SubClassHolder? dualClass = getActiveChar().getDualClass();
		if (getActiveChar().isDualClassActive() && dualClass != null)
		{
			return dualClass.getLevel();
		}

		if (getActiveChar().isSubClassActive())
		{
			SubClassHolder? holder = getActiveChar().getSubClasses().get(getActiveChar().getClassIndex());
			if (holder != null)
			{
				return holder.getLevel();
			}
		}
		return base.getLevel();
	}

	public int getBaseLevel()
	{
		return base.getLevel();
	}

	public override void setLevel(int value)
	{
		int level = value;
		if (level > ExperienceData.getInstance().getMaxLevel() - 1)
		{
			level = ExperienceData.getInstance().getMaxLevel() - 1;
		}

		CharInfoTable.getInstance().setLevel(getActiveChar().ObjectId, level);

		if (getActiveChar().isSubClassActive())
		{
            // TODO: null checking hack, subclasses must be handled differently
            SubClassHolder subclass = getActiveChar().getSubClasses().get(getActiveChar().getClassIndex()) ??
                throw new InvalidOperationException("Subsclass is active, but subclass holder is null");

			subclass.setLevel(level);
		}
		else
		{
			base.setLevel(level);
		}
	}

	public override long getSp()
	{
		if (getActiveChar().isSubClassActive())
		{
            // TODO: null checking hack, subclasses must be handled differently
            SubClassHolder subclass = getActiveChar().getSubClasses().get(getActiveChar().getClassIndex()) ??
                throw new InvalidOperationException("Subsclass is active, but subclass holder is null");

			return subclass.getSp();
		}

		return base.getSp();
	}

	public long getBaseSp()
	{
		return base.getSp();
	}

	public override void setSp(long value)
	{
		if (getActiveChar().isSubClassActive())
		{
            // TODO: null checking hack, subclasses must be handled differently
            SubClassHolder subclass = getActiveChar().getSubClasses().get(getActiveChar().getClassIndex()) ??
                throw new InvalidOperationException("Subsclass is active, but subclass holder is null");

			subclass.setSp(value);
		}
		else
		{
			base.setSp(value);
		}
	}

	/*
	 * Return current vitality points in integer format
	 */
	public int getVitalityPoints()
	{
		if (getActiveChar().isSubClassActive())
		{
			SubClassHolder? subClassHolder = getActiveChar().getSubClasses().get(getActiveChar().getClassIndex());
			if (subClassHolder == null)
			{
				return 0;
			}
			return Math.Min(MAX_VITALITY_POINTS, subClassHolder.getVitalityPoints());
		}
		return Math.Min(Math.Max(_vitalityPoints, MIN_VITALITY_POINTS), MAX_VITALITY_POINTS);
	}

	public int getBaseVitalityPoints()
	{
		return Math.Min(Math.Max(_vitalityPoints, MIN_VITALITY_POINTS), MAX_VITALITY_POINTS);
	}

	public double getVitalityExpBonus()
	{
		if (getVitalityPoints() > 0)
		{
			return getValue(Stat.VITALITY_EXP_RATE, Config.Rates.RATE_VITALITY_EXP_MULTIPLIER);
		}
		if (getActiveChar().getLimitedSayhaGraceEndTime() > DateTime.UtcNow)
		{
			return Config.Rates.RATE_LIMITED_SAYHA_GRACE_EXP_MULTIPLIER;
		}
		return 1;
	}

	public void setVitalityPoints(int value)
	{
		if (getActiveChar().isSubClassActive())
		{
            // TODO: null checking hack, subclasses must be handled differently
            SubClassHolder subclass = getActiveChar().getSubClasses().get(getActiveChar().getClassIndex()) ??
                throw new InvalidOperationException("Subsclass is active, but subclass holder is null");

			subclass.setVitalityPoints(value);
			return;
		}

        _vitalityPoints = Math.Min(Math.Max(value, MIN_VITALITY_POINTS), MAX_VITALITY_POINTS);
		getActiveChar().sendPacket(new ExVitalityPointInfoPacket(_vitalityPoints));
	}

	/*
	 * Set current vitality points to this value if quiet = true - does not send system messages
	 */
	public void setVitalityPoints(int value, bool quiet)
	{
		int points = Math.Min(Math.Max(value, MIN_VITALITY_POINTS), MAX_VITALITY_POINTS);
		if (points == getVitalityPoints())
		{
			return;
		}

		if (!quiet)
		{
			if (points < getVitalityPoints())
			{
				getActiveChar().sendPacket(SystemMessageId.YOUR_SAYHA_S_GRACE_HAS_DECREASED);
			}
			else
			{
				getActiveChar().sendPacket(SystemMessageId.YOUR_SAYHA_S_GRACE_HAS_INCREASED);
			}
		}

		setVitalityPoints(points);

		if (points == 0)
		{
			getActiveChar().sendPacket(SystemMessageId.YOUR_SAYHA_S_GRACE_IS_FULLY_EXHAUSTED);
		}
		else if (points == MAX_VITALITY_POINTS)
		{
			getActiveChar().sendPacket(SystemMessageId.YOUR_SAYHA_S_GRACE_IS_AT_MAXIMUM);
		}

		Player player = getActiveChar();
		player.sendPacket(new ExVitalityPointInfoPacket(getVitalityPoints()));
		player.broadcastUserInfo(UserInfoType.VITA_FAME);
		Party? party = player.getParty();
		if (party != null)
		{
			party.broadcastToPartyMembers(player,
				new PartySmallWindowUpdatePacket(player, PartySmallWindowUpdateType.VITALITY_POINTS));
		}
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void updateVitalityPoints(int value, bool useRates, bool quiet)
	{
		if (value == 0 || !Config.Character.ENABLE_VITALITY)
		{
			return;
		}

		double points = value;
		if (useRates)
		{
			if (getActiveChar().isLucky())
			{
				return;
			}

			if (points < 0) // vitality consumed
			{
				double consumeRate = getMul(Stat.VITALITY_CONSUME_RATE, 1);
				if (consumeRate <= 0)
				{
					return;
				}

				points *= consumeRate;
			}

			if (points > 0)
			{
				// vitality increased
				points *= Config.Rates.RATE_VITALITY_GAIN;
			}
			else
			{
				// vitality decreased
				points *= Config.Rates.RATE_VITALITY_LOST;
			}
		}

		if (points > 0)
		{
			points = Math.Min(getVitalityPoints() + points, MAX_VITALITY_POINTS);
		}
		else
		{
			points = Math.Max(getVitalityPoints() + points, MIN_VITALITY_POINTS);
		}

		if (Math.Abs(points - getVitalityPoints()) <= 1e-6)
		{
			return;
		}

		setVitalityPoints((int)points);
	}

	public double getExpBonusMultiplier()
	{
		double bonus = 1.0;
		double vitality = 1.0;
		double bonusExp = 1.0;

		// Bonus from Vitality System
		vitality = getVitalityExpBonus();

		// Bonus exp from skills
		bonusExp = 1 + getValue(Stat.BONUS_EXP, 0) / 100;
		if (vitality > 1.0)
		{
			bonus += vitality - 1;
		}

		if (bonusExp > 1)
		{
			bonus += bonusExp - 1;
		}

		// Check for abnormal bonuses
		bonus = Math.Max(bonus, 1);
		if (Config.Character.MAX_BONUS_EXP > 0)
		{
			bonus = Math.Min(bonus, Config.Character.MAX_BONUS_EXP);
		}

		return bonus;
	}

	public double getSpBonusMultiplier()
	{
		double bonus = 1.0;
		double vitality = 1.0;
		double bonusSp = 1.0;

		// Bonus from Vitality System
		vitality = getVitalityExpBonus();

		// Bonus sp from skills
		bonusSp = 1 + getValue(Stat.BONUS_SP, 0) / 100;
		if (vitality > 1.0)
		{
			bonus += vitality - 1;
		}

		if (bonusSp > 1)
		{
			bonus += bonusSp - 1;
		}

		// Check for abnormal bonuses
		bonus = Math.Max(bonus, 1);
		if (Config.Character.MAX_BONUS_SP > 0)
		{
			bonus = Math.Min(bonus, Config.Character.MAX_BONUS_SP);
		}

		return bonus;
	}

	/**
	 * Gets the maximum brooch jewel count.
	 * @return the maximum brooch jewel count
	 */
	public int getBroochJewelSlots()
	{
		return (int) getValue(Stat.BROOCH_JEWELS, 0);
	}

	/**
	 * Gets the maximum agathion count.
	 * @return the maximum agathion count
	 */
	public int getAgathionSlots()
	{
		return (int) getValue(Stat.AGATHION_SLOTS, 0);
	}

	/**
	 * Gets the maximum artifact book count.
	 * @return the maximum artifact book count
	 */
	public int getArtifactSlots()
	{
		return (int) getValue(Stat.ARTIFACT_SLOTS, 0);
	}

	public double getElementalSpiritXpBonus()
	{
		return getValue(Stat.ELEMENTAL_SPIRIT_BONUS_EXP, 1);
	}

	public double getElementalSpiritPower(ElementalType type, double @base)
	{
		return type == ElementalType.NONE ? 0 : getValue(type.getAttackStat(), @base);
	}

	public double getElementalSpiritCriticalRate(int @base)
	{
		return getValue(Stat.ELEMENTAL_SPIRIT_CRITICAL_RATE, @base);
	}

	public double getElementalSpiritCriticalDamage(double @base)
	{
		return getValue(Stat.ELEMENTAL_SPIRIT_CRITICAL_DAMAGE, @base);
	}

	public double getElementalSpiritDefense(ElementalType type, double @base)
	{
		return type == ElementalType.NONE ? 0 : getValue(type.getDefenseStat(), @base);
	}

	public double getElementSpiritAttack(ElementalType type, double @base)
	{
		return type == ElementalType.NONE ? 0 : getValue(type.getDefenseStat(), @base);
	}

	public override TimeSpan getReuseTime(Skill skill)
	{
		int addedReuse = 0;
		if (skill.HasEffectType(EffectTypes.TELEPORT))
		{
			switch (getActiveChar().getActingPlayer().getEinhasadOverseeingLevel())
			{
				case 6:
				{
					addedReuse = 20000;
					break;
				}
				case 7:
				{
					addedReuse = 30000;
					break;
				}
				case 8:
				{
					addedReuse = 40000;
					break;
				}
				case 9:
				{
					addedReuse = 50000;
					break;
				}
				case 10:
				{
					addedReuse = 60000;
					break;
				}
			}
		}
		return base.getReuseTime(skill) + TimeSpan.FromMilliseconds(addedReuse);
	}

	public override void recalculateStats(bool broadcast)
	{
		if (!getActiveChar().isChangingClass())
		{
			base.recalculateStats(broadcast);
		}
	}

	protected override void onRecalculateStats(bool broadcast)
	{
		base.onRecalculateStats(broadcast);

		Player player = getActiveChar();
		if (player.hasAbnormalType(AbnormalType.ABILITY_CHANGE) && player.hasServitors())
		{
			player.getServitors().Values.ForEach(servitor => servitor.getStat().recalculateStats(broadcast));
		}
	}
}