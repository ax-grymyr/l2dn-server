using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;
using Clan = L2Dn.GameServer.Model.Clans.Clan;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public struct CharacterInfoPacket: IOutgoingPacket
{
	private static readonly int[] PAPERDOLL_ORDER =
	[
		Inventory.PAPERDOLL_UNDER,
		Inventory.PAPERDOLL_HEAD,
		Inventory.PAPERDOLL_RHAND,
		Inventory.PAPERDOLL_LHAND,
		Inventory.PAPERDOLL_GLOVES,
		Inventory.PAPERDOLL_CHEST,
		Inventory.PAPERDOLL_LEGS,
		Inventory.PAPERDOLL_FEET,
		Inventory.PAPERDOLL_CLOAK,
		Inventory.PAPERDOLL_RHAND,
		Inventory.PAPERDOLL_HAIR,
		Inventory.PAPERDOLL_HAIR2
	];

	private static readonly int[] PAPERDOLL_ORDER_AUGMENT =
	[
		Inventory.PAPERDOLL_RHAND,
		Inventory.PAPERDOLL_LHAND,
		Inventory.PAPERDOLL_RHAND
	];

	private static readonly int[] PAPERDOLL_ORDER_VISUAL_ID =
	[
		Inventory.PAPERDOLL_RHAND,
		Inventory.PAPERDOLL_LHAND,
		Inventory.PAPERDOLL_RHAND,
		Inventory.PAPERDOLL_GLOVES,
		Inventory.PAPERDOLL_CHEST,
		Inventory.PAPERDOLL_LEGS,
		Inventory.PAPERDOLL_FEET,
		Inventory.PAPERDOLL_HAIR,
		Inventory.PAPERDOLL_HAIR2
	];
	
	private readonly Player _player;
	private readonly Clan _clan;
	private int _objId;
	private int _x;
	private int _y;
	private int _z;
	private int _heading;
	private readonly int _mAtkSpd;
	private readonly int _pAtkSpd;
	private readonly int _runSpd;
	private readonly int _walkSpd;
	private readonly int _swimRunSpd;
	private readonly int _swimWalkSpd;
	private readonly int _flyRunSpd;
	private readonly int _flyWalkSpd;
	private readonly double _moveMultiplier;
	private readonly float _attackSpeedMultiplier;
	private int _enchantLevel = 0;
	private int _armorEnchant = 0;
	private int _vehicleId = 0;
	private readonly bool _gmSeeInvis;

	public CharacterInfoPacket(Player player, bool gmSeeInvis)
	{
		_player = player;
		_objId = player.getObjectId();
		_clan = player.getClan();
		if ((_player.getVehicle() != null) && (_player.getInVehiclePosition() != null))
		{
			_x = _player.getInVehiclePosition().getX();
			_y = _player.getInVehiclePosition().getY();
			_z = _player.getInVehiclePosition().getZ();
			_vehicleId = _player.getVehicle().getObjectId();
		}
		else
		{
			_x = _player.getX();
			_y = _player.getY();
			_z = _player.getZ();
		}

		_heading = _player.getHeading();
		_mAtkSpd = _player.getMAtkSpd();
		_pAtkSpd = _player.getPAtkSpd();
		_attackSpeedMultiplier = (float)_player.getAttackSpeedMultiplier();
		_moveMultiplier = player.getMovementSpeedMultiplier();
		_runSpd = (int)Math.Round(player.getRunSpeed() / _moveMultiplier);
		_walkSpd = (int)Math.Round(player.getWalkSpeed() / _moveMultiplier);
		_swimRunSpd = (int)Math.Round(player.getSwimRunSpeed() / _moveMultiplier);
		_swimWalkSpd = (int)Math.Round(player.getSwimWalkSpeed() / _moveMultiplier);
		_flyRunSpd = player.isFlying() ? _runSpd : 0;
		_flyWalkSpd = player.isFlying() ? _walkSpd : 0;
		_enchantLevel = player.getInventory().getWeaponEnchant();
		_armorEnchant = player.getInventory().getArmorMinEnchant();
		_gmSeeInvis = gmSeeInvis;
	}

	public CharacterInfoPacket(Decoy decoy, bool gmSeeInvis)
		: this(decoy.getActingPlayer(), gmSeeInvis)
	{
		_objId = decoy.getObjectId();
		_x = decoy.getX();
		_y = decoy.getY();
		_z = decoy.getZ();
		_heading = decoy.getHeading();
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.CHAR_INFO);
		writer.WriteByte(0); // Grand Crusade
		writer.WriteInt32(_x); // Confirmed
		writer.WriteInt32(_y); // Confirmed
		writer.WriteInt32(_z); // Confirmed
		writer.WriteInt32(_vehicleId); // Confirmed
		writer.WriteInt32(_objId); // Confirmed
		writer.WriteString(_player.getAppearance().getVisibleName()); // Confirmed
		writer.WriteInt16((short)_player.getRace()); // Confirmed
		writer.WriteByte(_player.getAppearance().isFemale()); // Confirmed
		writer.WriteInt32((int)_player.getBaseTemplate().getClassId().GetRootClass());

		foreach (int slot in PAPERDOLL_ORDER)
		{
			writer.WriteInt32(_player.getInventory().getPaperdollItemDisplayId(slot)); // Confirmed
		}

		foreach (int slot in PAPERDOLL_ORDER_AUGMENT)
		{
			VariationInstance augment = _player.getInventory().getPaperdollAugmentation(slot);
			writer.WriteInt32(augment != null ? augment.getOption1Id() : 0); // Confirmed
			writer.WriteInt32(augment != null ? augment.getOption2Id() : 0); // Confirmed
		}

		writer.WriteByte((byte)_armorEnchant);

		foreach (int slot in PAPERDOLL_ORDER_VISUAL_ID)
		{
			writer.WriteInt32(_player.getInventory().getPaperdollItemVisualId(slot));
		}

		writer.WriteByte(_player.getPvpFlag());
		writer.WriteInt32(_player.getReputation());
		writer.WriteInt32(_mAtkSpd);
		writer.WriteInt32(_pAtkSpd);
		writer.WriteInt16((short)_runSpd);
		writer.WriteInt16((short)_walkSpd);
		writer.WriteInt16((short)_swimRunSpd);
		writer.WriteInt16((short)_swimWalkSpd);
		writer.WriteInt16((short)_flyRunSpd);
		writer.WriteInt16((short)_flyWalkSpd);
		writer.WriteInt16((short)_flyRunSpd);
		writer.WriteInt16((short)_flyWalkSpd);
		writer.WriteDouble(_moveMultiplier);
		writer.WriteDouble(_attackSpeedMultiplier);
		writer.WriteDouble(_player.getCollisionRadius());
		writer.WriteDouble(_player.getCollisionHeight());
		writer.WriteInt32(_player.getVisualHair());
		writer.WriteInt32(_player.getVisualHairColor());
		writer.WriteInt32(_player.getVisualFace());
		writer.WriteString(_gmSeeInvis ? "Invisible" : _player.getAppearance().getVisibleTitle());
		writer.WriteInt32(_player.getAppearance().getVisibleClanId());
		writer.WriteInt32(_player.getAppearance().getVisibleClanCrestId());
		writer.WriteInt32(_player.getAppearance().getVisibleAllyId());
		writer.WriteInt32(_player.getAppearance().getVisibleAllyCrestId());
		writer.WriteByte(!_player.isSitting()); // Confirmed
		writer.WriteByte(_player.isRunning()); // Confirmed
		writer.WriteByte(_player.isInCombat()); // Confirmed
		writer.WriteByte(!_player.isInOlympiadMode() && _player.isAlikeDead()); // Confirmed
		writer.WriteByte(_player.isInvisible());
		writer.WriteByte((byte)_player.getMountType()); // 1-on Strider, 2-on Wyvern, 3-on Great Wolf, 0-no mount
		writer.WriteByte((byte)_player.getPrivateStoreType()); // Confirmed

		writer.WriteInt16((short)_player.getCubics().size()); // Confirmed
		foreach (int cubicId in _player.getCubics().Keys)
		{
			writer.WriteInt16((short)cubicId);
		}

		writer.WriteByte(_player.isInMatchingRoom()); // Confirmed
		writer.WriteByte((byte)(_player.isInsideZone(ZoneId.WATER) ? 1 : _player.isFlyingMounted() ? 2 : 0));
		writer.WriteInt16((short)_player.getRecomHave()); // Confirmed
		writer.WriteInt32(_player.getMountNpcId() == 0 ? 0 : _player.getMountNpcId() + 1000000);
		writer.WriteInt32((int)_player.getClassId()); // Confirmed
		writer.WriteInt32(0); // TODO: Find me!
		writer.WriteByte((byte)(_player.isMounted() ? 0 : _enchantLevel)); // Confirmed
		writer.WriteByte((byte)_player.getTeam()); // Confirmed
		writer.WriteInt32(_player.getClanCrestLargeId());
		writer.WriteByte(_player.isNoble()); // Confirmed
		writer.WriteByte((byte)(_player.isHero() || (_player.isGM() && Config.GM_HERO_AURA)
			? 2
			: 0)); // 152 - Value for enabled changed to 2?

		writer.WriteByte(_player.isFishing()); // Confirmed
		ILocational baitLocation = _player.getFishing().getBaitLocation();
		if (baitLocation != null)
		{
			writer.WriteInt32(baitLocation.getX()); // Confirmed
			writer.WriteInt32(baitLocation.getY()); // Confirmed
			writer.WriteInt32(baitLocation.getZ()); // Confirmed
		}
		else
		{
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0);
		}

		writer.WriteInt32(_player.getAppearance().getNameColor()); // Confirmed
		writer.WriteInt32(_heading); // Confirmed
		writer.WriteByte((byte)_player.getPledgeClass());
		writer.WriteInt16((short)_player.getPledgeType());
		writer.WriteInt32(_player.getAppearance().getTitleColor()); // Confirmed
		writer.WriteByte((byte)(_player.isCursedWeaponEquipped()
			? CursedWeaponsManager.getInstance().getLevel(_player.getCursedWeaponEquippedId())
			: 0));
		
		writer.WriteInt32(_clan != null ? _clan.getReputationScore() : 0);
		writer.WriteInt32(_player.getTransformationDisplayId()); // Confirmed
		writer.WriteInt32(_player.getAgathionId()); // Confirmed
		writer.WriteByte(0); // nPvPRestrainStatus
		writer.WriteInt32((int)Math.Round(_player.getCurrentCp())); // Confirmed
		writer.WriteInt32(_player.getMaxHp()); // Confirmed
		writer.WriteInt32((int)Math.Round(_player.getCurrentHp())); // Confirmed
		writer.WriteInt32(_player.getMaxMp()); // Confirmed
		writer.WriteInt32((int)Math.Round(_player.getCurrentMp())); // Confirmed
		writer.WriteByte(0); // cBRLectureMark

		Set<AbnormalVisualEffect> abnormalVisualEffects = _player.getEffectList().getCurrentAbnormalVisualEffects();
		Team team = (Config.BLUE_TEAM_ABNORMAL_EFFECT != null) && (Config.RED_TEAM_ABNORMAL_EFFECT != null)
			? _player.getTeam()
			: Team.NONE;
		writer.WriteInt32(abnormalVisualEffects.size() + (_gmSeeInvis ? 1 : 0) +
		                  (team != Team.NONE ? 1 : 0)); // Confirmed
		foreach (AbnormalVisualEffect abnormalVisualEffect in abnormalVisualEffects)
		{
			writer.WriteInt16((short)abnormalVisualEffect); // Confirmed
		}

		if (_gmSeeInvis)
		{
			writer.WriteInt16((short)AbnormalVisualEffect.STEALTH);
		}

		if (team == Team.BLUE)
		{
			if (Config.BLUE_TEAM_ABNORMAL_EFFECT != null)
			{
				writer.WriteInt16((short)Config.BLUE_TEAM_ABNORMAL_EFFECT);
			}
		}
		else if ((team == Team.RED) && (Config.RED_TEAM_ABNORMAL_EFFECT != null))
		{
			writer.WriteInt16((short)Config.RED_TEAM_ABNORMAL_EFFECT);
		}

		writer.WriteByte(_player.isTrueHero() ? (byte)100 : (byte)0);
		writer.WriteByte(_player.isHairAccessoryEnabled()); // Hair accessory
		writer.WriteByte((byte)_player.getAbilityPointsUsed()); // Used Ability Points
		writer.WriteInt32(0); // nCursedWeaponClassId

		// AFK animation.
		if ((_player.getClan() != null) && (CastleManager.getInstance().getCastleByOwner(_player.getClan()) != null))
		{
			writer.WriteInt32(_player.isClanLeader() ? 100 : 101);
		}
		else
		{
			writer.WriteInt32(0);
		}

		// Rank.
		writer.WriteInt32(RankManager.getInstance().getPlayerGlobalRank(_player) == 1 ? 1 :
			RankManager.getInstance().getPlayerRaceRank(_player) == 1 ? 2 : 0);
		writer.WriteInt16(0);
		writer.WriteByte(0);
		writer.WriteInt32((int)_player.getClassId());
		writer.WriteByte(0);
		writer.WriteInt32(_player.getVisualHairColor() + 1); // 338 - DK color.
		writer.WriteInt32(0);
		writer.WriteByte((byte)(_player.getClassId().GetLevel() + 1)); // 362 - Vanguard mount.
	}
}