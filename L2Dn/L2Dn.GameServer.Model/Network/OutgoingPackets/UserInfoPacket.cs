using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Model.Enums;
using L2Dn.Packets;
using L2Dn.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct UserInfoPacket: IOutgoingPacket
{
	private readonly MaskablePacketHelper<UserInfoType> _helper;
	private readonly Player _player;
	private readonly int _relation;
	private readonly int _runSpd;
	private readonly int _walkSpd;
	private readonly int _swimRunSpd;
	private readonly int _swimWalkSpd;
	private readonly int _flRunSpd = 0;
	private readonly int _flWalkSpd = 0;
	private readonly int _flyRunSpd;
	private readonly int _flyWalkSpd;
	private readonly double _moveMultiplier;
	private readonly int _enchantLevel;
	private readonly int _armorEnchant;
	private readonly string _title;

	public UserInfoPacket(Player player, bool addAll = true)
	{
		_helper = new MaskablePacketHelper<UserInfoType>(4);
		_player = player;
		_relation = calculateRelation(player);
		_moveMultiplier = player.getMovementSpeedMultiplier();
		_runSpd = (int) Math.Round(player.getRunSpeed() / _moveMultiplier);
		_walkSpd = (int) Math.Round(player.getWalkSpeed() / _moveMultiplier);
		_swimRunSpd = (int) Math.Round(player.getSwimRunSpeed() / _moveMultiplier);
		_swimWalkSpd = (int) Math.Round(player.getSwimWalkSpeed() / _moveMultiplier);
		_flyRunSpd = player.isFlying() ? _runSpd : 0;
		_flyWalkSpd = player.isFlying() ? _walkSpd : 0;
		_enchantLevel = player.getInventory().getWeaponEnchant();
		_armorEnchant = player.getInventory().getArmorMinEnchant();
		_title = player.getTitle();

		if (player.isGM() && player.isInvisible())
		{
			_title = "[Invisible]";
		}

		if (addAll)
			_helper.AddAllComponents();
	}

	public void AddComponentType(UserInfoType component)
	{
		_helper.AddComponent(component);
	}

	public void WriteContent(PacketBitWriter writer)
	{
		if (_player == null)
		{
			return;
		}

		int initSize = 5;
		foreach (UserInfoType type in EnumUtil.GetValues<UserInfoType>())
		{
			if (_helper.HasComponent(type))
			{
				switch (type)
				{
					case UserInfoType.BASIC_INFO:
					{
						initSize += type.GetBlockLength() + _player.getAppearance().getVisibleName().Length * 2;
						break;
					}
					case UserInfoType.CLAN:
					{
						initSize += type.GetBlockLength() + _title.Length * 2;
						break;
					}
					default:
					{
						initSize += type.GetBlockLength();
						break;
					}
				}
			}
		}

		writer.WritePacketCode(OutgoingPacketCodes.USER_INFO);

		writer.WriteInt32(_player.ObjectId);
		writer.WriteInt32(initSize);
		writer.WriteInt16(29); // 362 - 29
		_helper.WriteMask(writer);

		if (_helper.HasComponent(UserInfoType.RELATION))
		{
			writer.WriteInt32(_relation);
		}

		if (_helper.HasComponent(UserInfoType.BASIC_INFO))
		{
			writer.WriteInt16((short)(23 + _player.getAppearance().getVisibleName().Length * 2));
			writer.WriteSizedString(_player.getName());

            // TODO This is player GM flag, but Client does not display all system messages for non-GM players for some reason.
            // As for temporary solution, all characters are set as GMs for the client.
            // It needs to be figured out which client settings must be changed to display all system messages for non-GM players.
            writer.WriteByte(true); //_player.isGM());

            writer.WriteByte((byte)_player.getRace());
			writer.WriteByte((byte)_player.getAppearance().getSex()); // sex
			writer.WriteInt32((int)_player.getBaseTemplate().getClassId().GetRootClass());
			writer.WriteInt32((int)_player.getClassId());
			writer.WriteInt32(_player.getLevel()); // 270
			writer.WriteInt32((int)_player.getClassId()); // 286
		}

		if (_helper.HasComponent(UserInfoType.BASE_STATS))
		{
			writer.WriteInt16(18);
			writer.WriteInt16((short)_player.getSTR());
			writer.WriteInt16((short)_player.getDEX());
			writer.WriteInt16((short)_player.getCON());
			writer.WriteInt16((short)_player.getINT());
			writer.WriteInt16((short)_player.getWIT());
			writer.WriteInt16((short)_player.getMEN());
			writer.WriteInt16(0);
			writer.WriteInt16(0);
		}

		if (_helper.HasComponent(UserInfoType.MAX_HPCPMP))
		{
			writer.WriteInt16(14);
			writer.WriteInt32(_player.getMaxHp());
			writer.WriteInt32(_player.getMaxMp());
			writer.WriteInt32(_player.getMaxCp());
		}

		if (_helper.HasComponent(UserInfoType.CURRENT_HPMPCP_EXP_SP))
		{
			int level = _player.getLevel();
			long exp = _player.getExp();
			long expForLevel = ExperienceData.getInstance().getExpForLevel(level);
			long expForNextLevel = ExperienceData.getInstance().getExpForLevel(level + 1);
			double expPercents = 1.0 * (exp - expForLevel) / (expForNextLevel - expForLevel);

			writer.WriteInt16(39);
			writer.WriteInt32((int) Math.Round(_player.getCurrentHp()));
			writer.WriteInt32((int) Math.Round(_player.getCurrentMp()));
			writer.WriteInt32((int) Math.Round(_player.getCurrentCp()));
			writer.WriteInt64(_player.getSp());
			writer.WriteInt64(_player.getExp());
			writer.WriteDouble(expPercents);
			writer.WriteByte(0); // 430
		}

		if (_helper.HasComponent(UserInfoType.ENCHANTLEVEL))
		{
			writer.WriteInt16(5); // 338
			writer.WriteByte((byte)_enchantLevel);
			writer.WriteByte((byte)_armorEnchant);
			writer.WriteByte(0); // 338 - cBackEnchant?
		}

		if (_helper.HasComponent(UserInfoType.APPAREANCE))
		{
			writer.WriteInt16(19); // 338
			writer.WriteInt32(_player.getVisualHair());
			writer.WriteInt32(_player.getVisualHairColor());
			writer.WriteInt32(_player.getVisualFace());
			writer.WriteByte(_player.isHairAccessoryEnabled());
			writer.WriteInt32(_player.getVisualHairColor() + 1); // 338 - DK color.
		}

		if (_helper.HasComponent(UserInfoType.STATUS))
		{
			writer.WriteInt16(6);
			writer.WriteByte((byte)_player.getMountType());
			writer.WriteByte((byte)_player.getPrivateStoreType());
			writer.WriteByte(_player.hasDwarvenCraft() || _player.getSkillLevel(248) > 0);
			writer.WriteByte(0);
		}

		if (_helper.HasComponent(UserInfoType.STATS))
		{
			writer.WriteInt16(64); // 270
			writer.WriteInt16((short)(_player.getActiveWeaponItem() != null ? 40 : 20));
			writer.WriteInt32(_player.getPAtk());
			writer.WriteInt32(_player.getPAtkSpd());
			writer.WriteInt32(_player.getPDef());
			writer.WriteInt32(_player.getEvasionRate());
			writer.WriteInt32(_player.getAccuracy());
			writer.WriteInt32(_player.getCriticalHit());
			writer.WriteInt32(_player.getMAtk());
			writer.WriteInt32(_player.getMAtkSpd());
			writer.WriteInt32(_player.getPAtkSpd()); // Seems like atk speed - 1
			writer.WriteInt32(_player.getMagicEvasionRate());
			writer.WriteInt32(_player.getMDef());
			writer.WriteInt32(_player.getMagicAccuracy());
			writer.WriteInt32(_player.getMCriticalHit());
			writer.WriteInt32(_player.getStat().getWeaponBonusPAtk()); // 270
			writer.WriteInt32(_player.getStat().getWeaponBonusMAtk()); // 270
		}

		if (_helper.HasComponent(UserInfoType.ELEMENTALS))
		{
			writer.WriteInt16(14);
			writer.WriteInt16(0);
			writer.WriteInt16(0);
			writer.WriteInt16(0);
			writer.WriteInt16(0);
			writer.WriteInt16(0);
			writer.WriteInt16(0);
		}

		if (_helper.HasComponent(UserInfoType.POSITION))
		{
			writer.WriteInt16(18);
			writer.WriteInt32(_player.getX());
			writer.WriteInt32(_player.getY());
			writer.WriteInt32(_player.getZ());
			writer.WriteInt32(_player.getVehicle()?.ObjectId ?? 0);
		}

		if (_helper.HasComponent(UserInfoType.SPEED))
		{
			writer.WriteInt16(18);
			writer.WriteInt16((short)_runSpd);
			writer.WriteInt16((short)_walkSpd);
			writer.WriteInt16((short)_swimRunSpd);
			writer.WriteInt16((short)_swimWalkSpd);
			writer.WriteInt16((short)_flRunSpd);
			writer.WriteInt16((short)_flWalkSpd);
			writer.WriteInt16((short)_flyRunSpd);
			writer.WriteInt16((short)_flyWalkSpd);
		}

		if (_helper.HasComponent(UserInfoType.MULTIPLIER))
		{
			writer.WriteInt16(18);
			writer.WriteDouble(_moveMultiplier);
			writer.WriteDouble(_player.getAttackSpeedMultiplier());
		}

		if (_helper.HasComponent(UserInfoType.COL_RADIUS_HEIGHT))
		{
			writer.WriteInt16(18);
			writer.WriteDouble(_player.getCollisionRadius());
			writer.WriteDouble(_player.getCollisionHeight());
		}

		if (_helper.HasComponent(UserInfoType.ATK_ELEMENTAL))
		{
			writer.WriteInt16(5);
			writer.WriteByte(0);
			writer.WriteInt16(0);
		}

		if (_helper.HasComponent(UserInfoType.CLAN))
		{
			writer.WriteInt16((short)(32 + _title.Length * 2));
			writer.WriteSizedString(_title);
			writer.WriteInt16((short)_player.getPledgeType());
			writer.WriteInt32(_player.getClanId() ?? 0);
			writer.WriteInt32(_player.getClanCrestLargeId() ?? 0);
			writer.WriteInt32(_player.getClanCrestId() ?? 0);
			writer.WriteInt32((int)_player.getClanPrivileges());
			writer.WriteByte(_player.isClanLeader());
			writer.WriteInt32(_player.getAllyId() ?? 0);
			writer.WriteInt32(_player.getAllyCrestId() ?? 0);
			writer.WriteByte(_player.isInMatchingRoom());
		}

		if (_helper.HasComponent(UserInfoType.SOCIAL))
		{
			writer.WriteInt16(34); // 447
			writer.WriteByte((byte)_player.getPvpFlag());
			writer.WriteInt32(_player.getReputation()); // Reputation
			writer.WriteByte(_player.isNoble());
			writer.WriteByte((byte)(_player.isHero() || (_player.isGM() && Config.GM_HERO_AURA) ? 2 : 0)); // 152 - Value for enabled changed to 2?
			writer.WriteByte((byte)_player.getPledgeClass());
			writer.WriteInt32(_player.getPkKills());
			writer.WriteInt32(_player.getPvpKills());
			writer.WriteInt16((short)_player.getRecomLeft());
			writer.WriteInt16((short)_player.getRecomHave());

			// AFK animation.
            Clan? clan = _player.getClan();
			if (clan != null && CastleManager.getInstance().getCastleByOwner(clan) != null) // 196
			{
				writer.WriteInt32(_player.isClanLeader() ? 100 : 101);
			}
			else
			{
				writer.WriteInt32(0);
			}
			writer.WriteInt32(0); // 228
			writer.WriteInt32(0); // 447
		}

		if (_helper.HasComponent(UserInfoType.VITA_FAME))
		{
			writer.WriteInt16(19); // 196
			writer.WriteInt32(_player.getVitalityPoints());
			writer.WriteByte(0); // Vita Bonus
			writer.WriteInt32(0); // _player.getFame()
			writer.WriteInt32(0); // _player.getRaidbossPoints()
			writer.WriteByte(0); // 196
			writer.WriteInt16(0); // Henna Seal Engraving Gauge
			writer.WriteByte(0); // 196
		}

		if (_helper.HasComponent(UserInfoType.SLOTS))
		{
			writer.WriteInt16(12); // 152
			writer.WriteByte((byte)_player.getInventory().getTalismanSlots());
			writer.WriteByte((byte)_player.getInventory().getBroochJewelSlots());
			writer.WriteByte((byte)_player.getTeam());
			writer.WriteInt32(0);
			if (_player.getInventory().getAgathionSlots() > 0)
			{
				writer.WriteByte(1); // Charm slots
				writer.WriteByte((byte)(_player.getInventory().getAgathionSlots() - 1));
			}
			else
			{
				writer.WriteByte(0); // Charm slots
				writer.WriteByte(0);
			}
			writer.WriteByte((byte)_player.getInventory().getArtifactSlots()); // Artifact set slots // 152
		}

		if (_helper.HasComponent(UserInfoType.MOVEMENTS))
		{
			writer.WriteInt16(4);
			writer.WriteByte((byte)(_player.isInsideZone(ZoneId.WATER) ? 1 : _player.isFlyingMounted() ? 2 : 0));
			writer.WriteByte(_player.isRunning());
		}

		if (_helper.HasComponent(UserInfoType.COLOR))
		{
			writer.WriteInt16(10);
			writer.WriteInt32(_player.getAppearance().getNameColor().Value);
			writer.WriteInt32(_player.getAppearance().getTitleColor().Value);
		}

		if (_helper.HasComponent(UserInfoType.INVENTORY_LIMIT))
		{
			writer.WriteInt16(13);
			writer.WriteInt16(0);
			writer.WriteInt16(0);
			writer.WriteInt16((short)_player.getInventoryLimit());
			writer.WriteByte((byte)(_player.isCursedWeaponEquipped() ? CursedWeaponsManager.getInstance().getLevel(_player.getCursedWeaponEquippedId()) : 0));
			writer.WriteByte(0); // 196
			writer.WriteByte(0); // 196
			writer.WriteByte(0); // 196
			writer.WriteByte(0); // 196
		}

		if (_helper.HasComponent(UserInfoType.TRUE_HERO))
		{
			writer.WriteInt16(9);
			writer.WriteInt32(0);
			writer.WriteInt16(0);
			writer.WriteByte((byte)(_player.isTrueHero() ? 100 : 0));
		}

		if (_helper.HasComponent(UserInfoType.ATT_SPIRITS)) // 152
		{
			writer.WriteInt16(34);
			writer.WriteInt32((int) _player.getFireSpiritAttack());
			writer.WriteInt32((int) _player.getWaterSpiritAttack());
			writer.WriteInt32((int) _player.getWindSpiritAttack());
			writer.WriteInt32((int) _player.getEarthSpiritAttack());
			writer.WriteInt32((int) _player.getFireSpiritDefense());
			writer.WriteInt32((int) _player.getWaterSpiritDefense());
			writer.WriteInt32((int) _player.getWindSpiritDefense());
			writer.WriteInt32((int) _player.getEarthSpiritDefense());
		}

		if (_helper.HasComponent(UserInfoType.RANKING)) // 196
		{
			writer.WriteInt16(6);
			writer.WriteInt32(RankManager.getInstance().getPlayerGlobalRank(_player) == 1 ? 1 : RankManager.getInstance().getPlayerRaceRank(_player) == 1 ? 2 : 0);
		}

		if (_helper.HasComponent(UserInfoType.STAT_POINTS)) // 235
		{
			PlayerVariables playerVariables = _player.getVariables();
			writer.WriteInt16(16);
			writer.WriteInt16((short)(_player.getLevel() < 76 ? 0 : _player.getLevel() - 75 + _player.getVariables().Get(PlayerVariables.ELIXIRS_AVAILABLE, 0) + (int) _player.getStat().getValue(Stat.ELIXIR_USAGE_LIMIT, 0))); // Usable points
			writer.WriteInt16((short)playerVariables.Get(PlayerVariables.STAT_STR, 0));
			writer.WriteInt16((short)playerVariables.Get(PlayerVariables.STAT_DEX, 0));
			writer.WriteInt16((short)playerVariables.Get(PlayerVariables.STAT_CON, 0));
			writer.WriteInt16((short)playerVariables.Get(PlayerVariables.STAT_INT, 0));
			writer.WriteInt16((short)playerVariables.Get(PlayerVariables.STAT_WIT, 0));
			writer.WriteInt16((short)playerVariables.Get(PlayerVariables.STAT_MEN, 0));
		}

		if (_helper.HasComponent(UserInfoType.STAT_ABILITIES)) // 235
		{
			PlayerVariables playerVariables = _player.getVariables();
			PlayerTemplate playerTemplate = _player.getTemplate();
			writer.WriteInt16(18);
			writer.WriteInt16((short)(_player.getSTR() - playerTemplate.getBaseSTR() - playerVariables.Get(PlayerVariables.STAT_STR, 0))); // additional STR
			writer.WriteInt16((short)(_player.getDEX() - playerTemplate.getBaseDEX() - playerVariables.Get(PlayerVariables.STAT_DEX, 0))); // additional DEX
			writer.WriteInt16((short)(_player.getCON() - playerTemplate.getBaseCON() - playerVariables.Get(PlayerVariables.STAT_CON, 0))); // additional CON
			writer.WriteInt16((short)(_player.getINT() - playerTemplate.getBaseINT() - playerVariables.Get(PlayerVariables.STAT_INT, 0))); // additional INT
			writer.WriteInt16((short)(_player.getWIT() - playerTemplate.getBaseWIT() - playerVariables.Get(PlayerVariables.STAT_WIT, 0))); // additional WIT
			writer.WriteInt16((short)(_player.getMEN() - playerTemplate.getBaseMEN() - playerVariables.Get(PlayerVariables.STAT_MEN, 0))); // additional MEN
			writer.WriteInt16(0);
			writer.WriteInt16(0);
		}

		if (_helper.HasComponent(UserInfoType.ELIXIR_USED)) // 286
		{
			writer.WriteInt16((short)_player.getVariables().Get(PlayerVariables.ELIXIRS_AVAILABLE, 0)); // count
			writer.WriteInt16(0);
		}

		if (_helper.HasComponent(UserInfoType.VANGUARD_MOUNT)) // 362
		{
			writer.WriteByte((byte)(_player.getClassId().GetLevel() + 1)); // 362 - Vanguard mount.
		}

		// TODO: this should not be here

		// Send exp bonus change.
		if (_helper.HasComponent(UserInfoType.VITA_FAME))
		{
			_player.sendUserBoostStat();
		}
	}

	private int calculateRelation(Player player)
	{
		int relation = 0;
		Party? party = player.getParty();
		Clan? clan = player.getClan();
		if (party != null)
		{
			relation |= 8; // Party member
			if (party.getLeader() == _player)
			{
				relation |= 16; // Party leader
			}
		}
		if (clan != null)
		{
			if (player.getSiegeState() == 1)
			{
				relation |= 256; // Clan member
			}
			else if (player.getSiegeState() == 2)
			{
				relation |= 32; // Clan member
			}
			if (clan.getLeaderId() == player.ObjectId)
			{
				relation |= 64; // Clan leader
			}
		}
		if (player.getSiegeState() != 0)
		{
			relation |= 128; // In siege
		}
		return relation;
	}
}