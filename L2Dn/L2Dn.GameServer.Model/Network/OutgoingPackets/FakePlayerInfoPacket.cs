using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;
using Clan = L2Dn.GameServer.Model.Clans.Clan;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct FakePlayerInfoPacket: IOutgoingPacket
{
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

	private readonly Npc _npc;
	private readonly int _objId;
	private readonly int _x;
	private readonly int _y;
	private readonly int _z;
	private readonly int _heading;
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
	private readonly FakePlayerHolder _fpcHolder;
	private readonly Clan? _clan;

	public FakePlayerInfoPacket(Npc npc)
	{
		_npc = npc;
		_objId = npc.ObjectId;
		_x = npc.getX();
		_y = npc.getY();
		_z = npc.getZ();
		_heading = npc.getHeading();
		_mAtkSpd = npc.getMAtkSpd();
		_pAtkSpd = npc.getPAtkSpd();
		_attackSpeedMultiplier = (float) npc.getAttackSpeedMultiplier();
		_moveMultiplier = npc.getMovementSpeedMultiplier();
		_runSpd = (int) Math.Round(npc.getRunSpeed() / _moveMultiplier);
		_walkSpd = (int) Math.Round(npc.getWalkSpeed() / _moveMultiplier);
		_swimRunSpd = (int) Math.Round(npc.getSwimRunSpeed() / _moveMultiplier);
		_swimWalkSpd = (int) Math.Round(npc.getSwimWalkSpeed() / _moveMultiplier);
		_flyRunSpd = npc.isFlying() ? _runSpd : 0;
		_flyWalkSpd = npc.isFlying() ? _walkSpd : 0;
		_fpcHolder = FakePlayerData.getInstance().getInfo(npc.getId());
		_clan = ClanTable.getInstance().getClan(_fpcHolder.getClanId());
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.CHAR_INFO); // TODO: this must be same packet as CharacterInfoPacket
		writer.WriteByte(0); // Grand Crusade
		writer.WriteInt32(_x);
		writer.WriteInt32(_y);
		writer.WriteInt32(_z);
		writer.WriteInt32(0); // vehicleId
		writer.WriteInt32(_objId);
		writer.WriteString(_npc.getName());
		writer.WriteInt16((short)_npc.getRace());
		writer.WriteByte(_npc.getTemplate().getSex() == Sex.Female);
		writer.WriteInt32((int)_fpcHolder.getClassId().GetRootClass());
		writer.WriteInt32(0); // Inventory.PAPERDOLL_UNDER
		writer.WriteInt32(_fpcHolder.getEquipHead());
		writer.WriteInt32(_fpcHolder.getEquipRHand());
		writer.WriteInt32(_fpcHolder.getEquipLHand());
		writer.WriteInt32(_fpcHolder.getEquipGloves());
		writer.WriteInt32(_fpcHolder.getEquipChest());
		writer.WriteInt32(_fpcHolder.getEquipLegs());
		writer.WriteInt32(_fpcHolder.getEquipFeet());
		writer.WriteInt32(_fpcHolder.getEquipCloak());
		writer.WriteInt32(_fpcHolder.getEquipRHand()); // dual hand
		writer.WriteInt32(_fpcHolder.getEquipHair());
		writer.WriteInt32(_fpcHolder.getEquipHair2());

		foreach (int slot in PAPERDOLL_ORDER_AUGMENT)
		{
			writer.WriteInt32(0);
			writer.WriteInt32(0);
		}

		writer.WriteByte((byte)_fpcHolder.getArmorEnchantLevel());
		foreach (int slot in PAPERDOLL_ORDER_VISUAL_ID)
		{
			writer.WriteInt32(0);
		}

		writer.WriteByte((byte)_npc.getScriptValue()); // getPvpFlag()
		writer.WriteInt32(_npc.getReputation());
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
		writer.WriteDouble(_npc.getCollisionRadius());
		writer.WriteDouble(_npc.getCollisionHeight());
		writer.WriteInt32(_fpcHolder.getHair());
		writer.WriteInt32(_fpcHolder.getHairColor());
		writer.WriteInt32(_fpcHolder.getFace());
		writer.WriteString(_npc.getTemplate().getTitle());
		if (_clan != null)
		{
			writer.WriteInt32(_clan.getId());
			writer.WriteInt32(_clan.getCrestId() ?? 0);
			writer.WriteInt32(_clan.getAllyId() ?? 0);
			writer.WriteInt32(_clan.getAllyCrestId() ?? 0);
		}
		else
		{
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0);
		}
		writer.WriteByte(1); // isSitting() ? 0 : 1 (at some initial tests it worked)
		writer.WriteByte(_npc.isRunning());
		writer.WriteByte(_npc.isInCombat());
		writer.WriteByte(_npc.isAlikeDead());
		writer.WriteByte(_npc.isInvisible());
		writer.WriteByte(0); // 1-on Strider, 2-on Wyvern, 3-on Great Wolf, 0-no mount
		writer.WriteByte(0); // getPrivateStoreType().getId()
		writer.WriteInt16(0); // getCubics().size()
		// getCubics().keySet().forEach(packet::writeH);
		writer.WriteByte(0);
		writer.WriteByte(_npc.isInsideZone(ZoneId.WATER));
		writer.WriteInt16((short)_fpcHolder.getRecommends());
		writer.WriteInt32(0); // getMountNpcId() == 0 ? 0 : getMountNpcId() + 1000000
		writer.WriteInt32((int)_fpcHolder.getClassId());
		writer.WriteInt32(0);
		writer.WriteByte((byte)_fpcHolder.getWeaponEnchantLevel()); // isMounted() ? 0 : _enchantLevel
		writer.WriteByte((byte)_npc.getTeam());
		writer.WriteInt32(_clan?.getCrestLargeId() ?? 0);
		writer.WriteByte((byte)_fpcHolder.getNobleLevel());
		writer.WriteByte((byte)(_fpcHolder.isHero() ? 2 : 0)); // 152 - Value for enabled changed to 2
		writer.WriteByte(_fpcHolder.isFishing());
		writer.WriteInt32(_fpcHolder.getBaitLocationX());
		writer.WriteInt32(_fpcHolder.getBaitLocationY());
		writer.WriteInt32(_fpcHolder.getBaitLocationZ());
		writer.WriteInt32(_fpcHolder.getNameColor());
		writer.WriteInt32(_heading);
		writer.WriteByte((byte)_fpcHolder.getPledgeStatus());
		writer.WriteInt16(0); // getPledgeType()
		writer.WriteInt32(_fpcHolder.getTitleColor());
		writer.WriteByte(0); // isCursedWeaponEquipped
		writer.WriteInt32(0); // getAppearance().getVisibleClanId() > 0 ? getClan().getReputationScore() : 0
		writer.WriteInt32(0); // getTransformationDisplayId()
		writer.WriteInt32(_fpcHolder.getAgathionId());
		writer.WriteByte(0);
		writer.WriteInt32(0); // getCurrentCp()
		writer.WriteInt32(_npc.getMaxHp());
		writer.WriteInt32((int)Math.Round(_npc.getCurrentHp()));
		writer.WriteInt32(_npc.getMaxMp());
		writer.WriteInt32((int)Math.Round(_npc.getCurrentMp()));
		writer.WriteByte(0);

		Set<AbnormalVisualEffect> abnormalVisualEffects = _npc.getEffectList().getCurrentAbnormalVisualEffects();
		writer.WriteInt32(abnormalVisualEffects.size() + (_npc.isInvisible() ? 1 : 0));
		foreach (AbnormalVisualEffect abnormalVisualEffect in abnormalVisualEffects)
		{
			writer.WriteInt16((short)abnormalVisualEffect);
		}
		if (_npc.isInvisible())
		{
			writer.WriteInt16((short)AbnormalVisualEffect.STEALTH);
		}

		writer.WriteByte(0); // isTrueHero() ? 100 : 0
		writer.WriteByte((_fpcHolder.getHair() > 0) || (_fpcHolder.getEquipHair2() > 0));
		writer.WriteByte(0); // Used Ability Points
		writer.WriteInt32(0); // CursedWeaponClassId

		writer.WriteInt32(0); // AFK animation.

		writer.WriteInt32(0); // Rank.
		writer.WriteInt16(0);
		writer.WriteByte(0);
		writer.WriteInt32((int)_fpcHolder.getClassId());
		writer.WriteByte(0);
		writer.WriteInt32(_fpcHolder.getHairColor() + 1); // 338 - DK color.
		writer.WriteInt32(0);
		writer.WriteByte((byte)(_fpcHolder.getClassId().GetLevel() + 1)); // 362 - Vanguard mount.
	}
}