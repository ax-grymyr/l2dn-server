using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct GMViewCharacterInfoPacket: IOutgoingPacket
{
	private static readonly int[] PAPERDOLL_ORDER =
	{
		Inventory.PAPERDOLL_UNDER,
		Inventory.PAPERDOLL_REAR,
		Inventory.PAPERDOLL_LEAR,
		Inventory.PAPERDOLL_NECK,
		Inventory.PAPERDOLL_RFINGER,
		Inventory.PAPERDOLL_LFINGER,
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
		Inventory.PAPERDOLL_HAIR2,
		Inventory.PAPERDOLL_RBRACELET,
		Inventory.PAPERDOLL_LBRACELET,
		Inventory.PAPERDOLL_AGATHION1,
		Inventory.PAPERDOLL_AGATHION2,
		Inventory.PAPERDOLL_AGATHION3,
		Inventory.PAPERDOLL_AGATHION4,
		Inventory.PAPERDOLL_AGATHION5,
		Inventory.PAPERDOLL_DECO1,
		Inventory.PAPERDOLL_DECO2,
		Inventory.PAPERDOLL_DECO3,
		Inventory.PAPERDOLL_DECO4,
		Inventory.PAPERDOLL_DECO5,
		Inventory.PAPERDOLL_DECO6,
		Inventory.PAPERDOLL_BELT,
		Inventory.PAPERDOLL_BROOCH,
		Inventory.PAPERDOLL_BROOCH_JEWEL1,
		Inventory.PAPERDOLL_BROOCH_JEWEL2,
		Inventory.PAPERDOLL_BROOCH_JEWEL3,
		Inventory.PAPERDOLL_BROOCH_JEWEL4,
		Inventory.PAPERDOLL_BROOCH_JEWEL5,
		Inventory.PAPERDOLL_BROOCH_JEWEL6,
		Inventory.PAPERDOLL_ARTIFACT_BOOK,
		Inventory.PAPERDOLL_ARTIFACT1,
		Inventory.PAPERDOLL_ARTIFACT2,
		Inventory.PAPERDOLL_ARTIFACT3,
		Inventory.PAPERDOLL_ARTIFACT4,
		Inventory.PAPERDOLL_ARTIFACT5,
		Inventory.PAPERDOLL_ARTIFACT6,
		Inventory.PAPERDOLL_ARTIFACT7,
		Inventory.PAPERDOLL_ARTIFACT8,
		Inventory.PAPERDOLL_ARTIFACT9,
		Inventory.PAPERDOLL_ARTIFACT10,
		Inventory.PAPERDOLL_ARTIFACT11,
		Inventory.PAPERDOLL_ARTIFACT12,
		Inventory.PAPERDOLL_ARTIFACT13,
		Inventory.PAPERDOLL_ARTIFACT14,
		Inventory.PAPERDOLL_ARTIFACT15,
		Inventory.PAPERDOLL_ARTIFACT16,
		Inventory.PAPERDOLL_ARTIFACT17,
		Inventory.PAPERDOLL_ARTIFACT18,
		Inventory.PAPERDOLL_ARTIFACT19,
		Inventory.PAPERDOLL_ARTIFACT20,
		Inventory.PAPERDOLL_ARTIFACT21,
	};

	private readonly Player _player;
	private readonly int _runSpd;
	private readonly int _walkSpd;
	private readonly int _swimRunSpd;
	private readonly int _swimWalkSpd;
	private readonly int _flyRunSpd;
	private readonly int _flyWalkSpd;
	private readonly double _moveMultiplier;

	public GMViewCharacterInfoPacket(Player player)
	{
		_player = player;
		_moveMultiplier = player.getMovementSpeedMultiplier();
		_runSpd = (int)Math.Round(player.getRunSpeed() / _moveMultiplier);
		_walkSpd = (int)Math.Round(player.getWalkSpeed() / _moveMultiplier);
		_swimRunSpd = (int)Math.Round(player.getSwimRunSpeed() / _moveMultiplier);
		_swimWalkSpd = (int)Math.Round(player.getSwimWalkSpeed() / _moveMultiplier);
		_flyRunSpd = player.isFlying() ? _runSpd : 0;
		_flyWalkSpd = player.isFlying() ? _walkSpd : 0;
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.GM_VIEW_CHARACTER_INFO);

		writer.WriteInt32(_player.getX());
		writer.WriteInt32(_player.getY());
		writer.WriteInt32(_player.getZ());
		writer.WriteInt32(_player.getHeading());
		writer.WriteInt32(_player.ObjectId);
		writer.WriteString(_player.getName());
		writer.WriteInt32((int)_player.getRace());
		writer.WriteInt32((int)_player.getAppearance().getSex());
		writer.WriteInt32((int)_player.getClassId());
		writer.WriteInt32(_player.getLevel());
		writer.WriteInt64(_player.getExp());
		writer.WriteDouble((_player.getExp() - ExperienceData.getInstance().getExpForLevel(_player.getLevel())) / (ExperienceData.getInstance().getExpForLevel(_player.getLevel() + 1) - ExperienceData.getInstance().getExpForLevel(_player.getLevel()))); // High Five exp %
		writer.WriteInt32(_player.getSTR());
		writer.WriteInt32(_player.getDEX());
		writer.WriteInt32(_player.getCON());
		writer.WriteInt32(_player.getINT());
		writer.WriteInt32(_player.getWIT());
		writer.WriteInt32(_player.getMEN());
		writer.WriteInt32(0); // LUC
		writer.WriteInt32(0); // CHA
		writer.WriteInt32(_player.getMaxHp());
		writer.WriteInt32((int) _player.getCurrentHp());
		writer.WriteInt32(_player.getMaxMp());
		writer.WriteInt32((int) _player.getCurrentMp());
		writer.WriteInt64(_player.getSp());
		writer.WriteInt32(_player.getCurrentLoad());
		writer.WriteInt32(_player.getMaxLoad());
		writer.WriteInt32(_player.getPkKills());

		foreach (int slot in PAPERDOLL_ORDER)
		{
			writer.WriteInt32(_player.getInventory().getPaperdollObjectId(slot));
		}

		foreach (int slot in PAPERDOLL_ORDER)
		{
			writer.WriteInt32(_player.getInventory().getPaperdollItemDisplayId(slot));
		}

		for (int slot = 0; slot < 11; slot++)
		{
			VariationInstance? augment = _player.getInventory().getPaperdollAugmentation(slot);
			writer.WriteInt32(augment != null ? augment.getOption1Id() : 0); // Confirmed
			writer.WriteInt32(augment != null ? augment.getOption2Id() : 0); // Confirmed
		}

		for (int index = 0; index < 98; index++)
		{
			writer.WriteInt32(0); // unk
		}

		writer.WriteByte(0); // unk
		writer.WriteByte(0); // unk
		writer.WriteByte((byte)_player.getInventory().getTalismanSlots()); // CT2.3
		writer.WriteByte(_player.getInventory().canEquipCloak()); // CT2.3
		writer.WriteByte(0);
		writer.WriteInt16(0);
		writer.WriteInt32(_player.getPAtk());
		writer.WriteInt32(_player.getPAtkSpd());
		writer.WriteInt32(_player.getPDef());
		writer.WriteInt32(_player.getEvasionRate());
		writer.WriteInt32(_player.getAccuracy());
		writer.WriteInt32(_player.getCriticalHit());
		writer.WriteInt32(_player.getMAtk());
		writer.WriteInt32(_player.getMAtkSpd());
		writer.WriteInt32(_player.getPAtkSpd());
		writer.WriteInt32(_player.getMDef());
		writer.WriteInt32(_player.getMagicEvasionRate());
		writer.WriteInt32(_player.getMagicAccuracy());
		writer.WriteInt32(_player.getMCriticalHit());
		writer.WriteInt32((int)_player.getPvpFlag()); // 0-non-pvp 1-pvp = violett name
		writer.WriteInt32(_player.getReputation());
		writer.WriteInt32(_runSpd);
		writer.WriteInt32(_walkSpd);
		writer.WriteInt32(_swimRunSpd);
		writer.WriteInt32(_swimWalkSpd);
		writer.WriteInt32(_flyRunSpd);
		writer.WriteInt32(_flyWalkSpd);
		writer.WriteInt32(_flyRunSpd);
		writer.WriteInt32(_flyWalkSpd);
		writer.WriteDouble(_moveMultiplier);
		writer.WriteDouble(_player.getAttackSpeedMultiplier()); // 2.9); //
		writer.WriteDouble(_player.getCollisionRadius()); // scale
		writer.WriteDouble(_player.getCollisionHeight()); // y offset ??!? fem dwarf 4033
		writer.WriteInt32(_player.getAppearance().getHairStyle());
		writer.WriteInt32(_player.getAppearance().getHairColor());
		writer.WriteInt32(_player.getAppearance().getFace());
		writer.WriteInt32(_player.isGM() ? 1 : 0); // builder level
		writer.WriteString(_player.getTitle());
		writer.WriteInt32(_player.getClanId() ?? 0); // pledge id
		writer.WriteInt32(_player.getClanCrestId() ?? 0); // pledge crest id
		writer.WriteInt32(_player.getAllyId() ?? 0); // ally id
		writer.WriteByte((byte)_player.getMountType()); // mount type
		writer.WriteByte((byte)_player.getPrivateStoreType());
		writer.WriteByte(_player.hasDwarvenCraft());
		writer.WriteInt32(_player.getPkKills());
		writer.WriteInt32(_player.getPvpKills());
		writer.WriteInt16((short)_player.getRecomLeft());
		writer.WriteInt16((short)_player.getRecomHave()); // Blue value for name (0 = white, 255 = pure blue)
		writer.WriteInt32((int)_player.getClassId());
		writer.WriteInt32(0); // special effects? circles around player...
		writer.WriteInt32(_player.getMaxCp());
		writer.WriteInt32((int) _player.getCurrentCp());
		writer.WriteByte(_player.isRunning()); // changes the Speed display on Status Window
		writer.WriteByte(321 - 256); // TODO: writer.WriteByte(321) ?????
		writer.WriteInt32((int)_player.getPledgeClass()); // changes the text above CP on Status Window
		writer.WriteByte(_player.isNoble());
		writer.WriteByte(_player.isHero());
		writer.WriteInt32(_player.getAppearance().getNameColor().Value);
		writer.WriteInt32(_player.getAppearance().getTitleColor().Value);

		AttributeType attackAttribute = _player.getAttackElement();
		writer.WriteInt16((short)attackAttribute);
		writer.WriteInt16((short)_player.getAttackElementValue(attackAttribute));
		foreach (AttributeType type in AttributeTypeUtil.AttributeTypes)
		{
			writer.WriteInt16((short)_player.getDefenseElementValue(type));
		}

		writer.WriteInt32(_player.getFame());
		writer.WriteInt32(_player.getVitalityPoints());
		writer.WriteInt32(0);
		writer.WriteInt32(0);
	}
}