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
using L2Dn.Geometry;
using L2Dn.Packets;
using Clan = L2Dn.GameServer.Model.Clans.Clan;
using Config = L2Dn.GameServer.Configuration.Config;

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
	private int _objId;
	private Location _location;
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
		_objId = player.ObjectId;

        Vehicle? vehicle = _player.getVehicle();
		if (vehicle != null)
		{
			_location = new Location(_player.getInVehiclePosition(), _player.getHeading());
			_vehicleId = vehicle.ObjectId;
		}
		else
		{
			_location = _player.Location;
		}

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
		_objId = decoy.ObjectId;
		_location = decoy.Location;
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.CHAR_INFO);
		writer.WriteByte(0); // Grand Crusade
		writer.WriteLocation3D(_location.Location3D); // Confirmed
		writer.WriteInt32(_vehicleId); // Confirmed
		writer.WriteInt32(_objId); // Confirmed
		writer.WriteString(_player.getAppearance().getVisibleName()); // Confirmed
		writer.WriteInt16((short)_player.getRace()); // Confirmed
		writer.WriteByte((byte)_player.getAppearance().getSex()); // Confirmed
		writer.WriteInt32((int)_player.getBaseTemplate().getClassId().GetRootClass());

		foreach (int slot in PAPERDOLL_ORDER)
		{
			writer.WriteInt32(_player.getInventory().getPaperdollItemDisplayId(slot)); // Confirmed
		}

		foreach (int slot in PAPERDOLL_ORDER_AUGMENT)
		{
			VariationInstance? augment = _player.getInventory().getPaperdollAugmentation(slot);
			writer.WriteInt32(augment != null ? augment.getOption1Id() : 0); // Confirmed
			writer.WriteInt32(augment != null ? augment.getOption2Id() : 0); // Confirmed
		}

		writer.WriteByte((byte)_armorEnchant);

		foreach (int slot in PAPERDOLL_ORDER_VISUAL_ID)
		{
			writer.WriteInt32(_player.getInventory().getPaperdollItemVisualId(slot));
		}

		writer.WriteByte((byte)_player.getPvpFlag());
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
		writer.WriteInt32(_player.getAppearance().getVisibleClanId() ?? 0);
		writer.WriteInt32(_player.getAppearance().getVisibleClanCrestId() ?? 0);
		writer.WriteInt32(_player.getAppearance().getVisibleAllyId() ?? 0);
		writer.WriteInt32(_player.getAppearance().getVisibleAllyCrestId() ?? 0);
		writer.WriteByte(!_player.isSitting()); // Confirmed
		writer.WriteByte(_player.isRunning()); // Confirmed
		writer.WriteByte(_player.isInCombat()); // Confirmed
		writer.WriteByte(!_player.isInOlympiadMode() && _player.isAlikeDead()); // Confirmed
		writer.WriteByte(_player.isInvisible());
		writer.WriteByte((byte)_player.getMountType()); // 1-on Strider, 2-on Wyvern, 3-on Great Wolf, 0-no mount
		writer.WriteByte((byte)_player.getPrivateStoreType()); // Confirmed

		writer.WriteInt16((short)_player.getCubics().Count); // Confirmed
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
		writer.WriteInt32(_player.getClanCrestLargeId() ?? 0);
		writer.WriteByte(_player.isNoble()); // Confirmed
		writer.WriteByte((byte)(_player.isHero() || (_player.isGM() && Config.General.GM_HERO_AURA)
			? 2
			: 0)); // 152 - Value for enabled changed to 2?

		writer.WriteByte(_player.isFishing()); // Confirmed

		Location3D baitLocation = _player.getFishing().getBaitLocation() ?? default;
		writer.WriteLocation3D(baitLocation);

		writer.WriteInt32(_player.getAppearance().getNameColor().Value); // Confirmed
		writer.WriteInt32(_location.Heading); // Confirmed
		writer.WriteByte((byte)_player.getPledgeClass());
		writer.WriteInt16((short)_player.getPledgeType());
		writer.WriteInt32(_player.getAppearance().getTitleColor().Value); // Confirmed
		writer.WriteByte((byte)(_player.isCursedWeaponEquipped()
			? CursedWeaponsManager.getInstance().getLevel(_player.getCursedWeaponEquippedId())
			: 0));

        Clan? clan = _player.getClan();
		writer.WriteInt32(clan != null ? clan.getReputationScore() : 0);
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
		Team team = Config.General.BLUE_TEAM_ABNORMAL_EFFECT != AbnormalVisualEffect.None && Config.General.RED_TEAM_ABNORMAL_EFFECT != AbnormalVisualEffect.None
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
			if (Config.General.BLUE_TEAM_ABNORMAL_EFFECT != AbnormalVisualEffect.None)
			{
				writer.WriteInt16((short)Config.General.BLUE_TEAM_ABNORMAL_EFFECT);
			}
		}
		else if (team == Team.RED && Config.General.RED_TEAM_ABNORMAL_EFFECT != AbnormalVisualEffect.None)
		{
			writer.WriteInt16((short)Config.General.RED_TEAM_ABNORMAL_EFFECT);
		}

		writer.WriteByte(_player.isTrueHero() ? (byte)100 : (byte)0);
		writer.WriteByte(_player.isHairAccessoryEnabled()); // Hair accessory
		writer.WriteByte((byte)_player.getAbilityPointsUsed()); // Used Ability Points
		writer.WriteInt32(0); // nCursedWeaponClassId

		// AFK animation.
		if (clan != null && CastleManager.getInstance().getCastleByOwner(clan) != null)
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