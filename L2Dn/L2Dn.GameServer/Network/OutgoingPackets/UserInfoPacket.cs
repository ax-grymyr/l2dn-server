using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct UserInfoPacket(int objectId, Character character): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
		writer.WriteByte(0x32); // packet code (0x04 in C4)

		CharacterClassInfo classInfo = StaticData.Templates[character.Class];
		CharacterSpecData specData = StaticData.Templates[classInfo.Race][classInfo.Spec];
		CharacterBaseStats baseStats = specData.BaseStats;
		CollisionDimensions dimensions = specData.GetCollisionDimensions(character.Sex);
		(int level, decimal percents) = StaticData.Levels.GetLevelForExp(character.Exp);

		byte[] mask = new byte[4]; // TODO: mask must be int
		foreach (UserInfoComponent component in Enum.GetValues<UserInfoComponent>())
			UserInfoUtil.AddComponent(mask, component);

		int size = Enum.GetValues<UserInfoComponent>().Where(x => UserInfoUtil.HasComponent(mask, x))
			.Select(x => x.GetSize()).Sum();

		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.BASIC_INFO))
			size += character.Name.Length * 2;

		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.CLAN))
			size += character.Title.Length * 2;

		writer.WriteInt32(objectId); // object id
		writer.WriteInt32(size);
		writer.WriteInt16(29); // 362 - 29
		writer.WriteBytes(mask);
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.RELATION))
		{
			// relation: 8 - party member, 16 - party leader,
			// 32 - clan member at siege state 2
			// 64 - clan leader
			// 128 - in siege
			// 256 - clan member at siege state 1
			writer.WriteInt32(0);
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.BASIC_INFO))
		{
			writer.WriteInt16((short)(23 + character.Name.Length * 2));
			writer.WriteSizedString(character.Name);
			writer.WriteBoolean(true); // is GM
			writer.WriteByte((byte)classInfo.Race); // race
			writer.WriteByte((byte)character.Sex); // sex
			writer.WriteInt32((int)classInfo.BaseClass.Class); // root class id
			writer.WriteInt32((int)character.Class); // class id
			writer.WriteInt32(level); // 270: level
			writer.WriteInt32((int)character.Class); // 286: class id
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.BASE_STATS))
		{
			writer.WriteInt16(18);
			writer.WriteInt16((short)baseStats.Str); // STR
			writer.WriteInt16((short)baseStats.Dex); // DEX
			writer.WriteInt16((short)baseStats.Con); // CON
			writer.WriteInt16((short)baseStats.Int); // INT
			writer.WriteInt16((short)baseStats.Wit); // WIT
			writer.WriteInt16((short)baseStats.Men); // MEN
			writer.WriteInt16(0);
			writer.WriteInt16(0);
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.MAX_HPCPMP))
		{
			writer.WriteInt16(14);
  		    writer.WriteInt32(character.MaxHp); // max HP
			writer.WriteInt32(character.MaxMp); // max MP
			writer.WriteInt32(character.MaxCp); // max CP
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.CURRENT_HPMPCP_EXP_SP))
		{
			writer.WriteInt16(39);
			writer.WriteInt32(character.CurrentHp); // current HP
			writer.WriteInt32(character.CurrentMp); // current MP
			writer.WriteInt32(character.CurrentCp); // current CP
			writer.WriteInt64(character.Sp); // SP
			writer.WriteInt64(character.Exp); // Exp
			writer.WriteDouble((double)percents);
			writer.WriteByte(0); // 430
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.ENCHANTLEVEL))
		{
			writer.WriteInt16(5); // 338
			writer.WriteByte(0); // _enchantLevel
			writer.WriteByte(0); // _armorEnchant
			writer.WriteByte(0); // 338 - cBackEnchant?
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.APPAREANCE))
		{
			writer.WriteInt16(19); // 338
			writer.WriteInt32(character.HairStyle); // hair style
			writer.WriteInt32(character.HairColor); // hair color
			writer.WriteInt32(character.Face); // face
			writer.WriteBoolean(false); // isHairAccessoryEnabled
			writer.WriteInt32(1); // 338 - DK color. = _player.getVisualHairColor() + 1
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.STATUS))
		{
			writer.WriteInt16(6);
			writer.WriteByte(0); // mount type
			writer.WriteByte(0); // private store id
			writer.WriteByte(0); // has dwarven craft = _player.hasDwarvenCraft() || (_player.getSkillLevel(248) > 0) ? 1 : 0
			writer.WriteByte(0);
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.STATS))
		{
			writer.WriteInt16(64); // 270
			writer.WriteInt16(20); // _player.getActiveWeaponItem() != null ? 40 : 20
			writer.WriteInt32(baseStats.PAtk); // PAtk
			writer.WriteInt32(baseStats.PAtkSpd); // PAtkSpeed
			writer.WriteInt32(baseStats.PDef); // PDef
			writer.WriteInt32(20); // Evasion
			writer.WriteInt32(25); // Accuracy
			writer.WriteInt32(baseStats.CritRate); // PCritical
			writer.WriteInt32(baseStats.MAtk); // MAtk
			writer.WriteInt32(baseStats.MAtkSpd); // MAtkSpeed
			writer.WriteInt32(baseStats.PAtkSpd - 1); // Seems like atk speed - 1
			writer.WriteInt32(25); // Magic evasion
			writer.WriteInt32(360); // MDef
			writer.WriteInt32(25); // Magic accuracy
			writer.WriteInt32(100); // MCritical
			writer.WriteInt32(0); // 270 - weapon bonus PAtk
			writer.WriteInt32(0); // 270 - weapon bonus MAtk

		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.ELEMENTALS))
		{
			writer.WriteInt16(14);
			writer.WriteInt16(0);
			writer.WriteInt16(0);
			writer.WriteInt16(0);
			writer.WriteInt16(0);
			writer.WriteInt16(0);
			writer.WriteInt16(0);
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.POSITION))
		{
			writer.WriteInt16(18);
			writer.WriteInt32(character.LocationX); // X
			writer.WriteInt32(character.LocationY); // Y
			writer.WriteInt32(character.LocationZ); // Z
			writer.WriteInt32(0); // vehicle object id
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.SPEED))
		{
			writer.WriteInt16(18);
			writer.WriteInt16((short)baseStats.RunSpd); // run speed
			writer.WriteInt16((short)(baseStats.RunSpd / 2)); // walk speed
			writer.WriteInt16((short)baseStats.RunSpd); // swim run speed
			writer.WriteInt16((short)(baseStats.RunSpd / 2)); // swim walk speed
			writer.WriteInt16((short)baseStats.RunSpd); // fly run speed
			writer.WriteInt16((short)(baseStats.RunSpd / 2)); // fly walk speed
			writer.WriteInt16((short)baseStats.RunSpd); // fly run speed
			writer.WriteInt16((short)(baseStats.RunSpd / 2)); // fly walk speed
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.MULTIPLIER))
		{
			writer.WriteInt16(18);
			writer.WriteDouble(1.0); // run multiplier
			writer.WriteDouble(1.0); // attack speed multiplier
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.COL_RADIUS_HEIGHT))
		{
			writer.WriteInt16(18);
			writer.WriteDouble((double)dimensions.Radius); // collision radius
			writer.WriteDouble((double)dimensions.Height); // collision height
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.ATK_ELEMENTAL))
		{
			writer.WriteInt16(5);
			writer.WriteByte(0);
			writer.WriteInt16(0);
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.CLAN))
		{
			writer.WriteInt16((short)(32 + character.Title.Length * 2));
			writer.WriteSizedString(character.Title);
			writer.WriteInt16(0); // pledge type ???
			writer.WriteInt32(0); // clan id
			writer.WriteInt32(0); // clan crest large id
			writer.WriteInt32(0); // clan crest id
			writer.WriteInt32(0); // clan privileges bitmask
			writer.WriteBoolean(false); // is clan leader
			writer.WriteInt32(0); // ally id
			writer.WriteInt32(0); // ally crest id
			writer.WriteBoolean(false); // is in matching room
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.SOCIAL))
		{
			writer.WriteInt16(34); // 447
			writer.WriteBoolean(false); // pvp flag
			writer.WriteInt32(character.Reputation); // Reputation
			writer.WriteBoolean(false); // is noble
			writer.WriteBoolean(false); // 152 - Value for enabled changed to 2?  _player.isHero() || (_player.isGM() && Config.GM_HERO_AURA) ? 2 : 0
			writer.WriteByte(0); // pledge class
			writer.WriteInt32(character.PkCounter); // pk kills
			writer.WriteInt32(character.PvpCounter); // pvp kills
			writer.WriteInt16(0); // recommendations left
			writer.WriteInt16(0); // recommendations have

			// AFK animation.
			// if ((_player.getClan() != null) && (CastleManager.getInstance().getCastleByOwner(_player.getClan()) != null)) // 196
			// {
			// 	writer.WriteInt32(_player.isClanLeader() ? 100 : 101);
			// }
			// else
			// {
			writer.WriteInt32(0);
			//}

			writer.WriteInt32(0); // 228
			writer.WriteInt32(0); // 447
		}

		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.VITA_FAME))
		{
			writer.WriteInt16(19); // 196
			writer.WriteInt32(0); // vitality points
			writer.WriteByte(0); // Vita Bonus
			writer.WriteInt32(0); // _player.getFame()
			writer.WriteInt32(0); // _player.getRaidbossPoints()
			writer.WriteByte(0); // 196
			writer.WriteInt16(0); // Henna Seal Engraving Gauge
			writer.WriteByte(0); // 196
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.SLOTS))
		{
			writer.WriteInt16(12); // 152
			writer.WriteByte(0); // talisman slots
			writer.WriteByte(0); // brooch jewels slots
			writer.WriteByte(0); // team id???
			writer.WriteInt32(0);
			// if (_player.getInventory().getAgathionSlots() > 0)
			// {
			// 	writer.WriteByte(1); // Charm slots
			// 	writer.WriteByte(_player.getInventory().getAgathionSlots() - 1);
			// }
			// else
			// {
			writer.WriteByte(0); // Charm slots
			writer.WriteByte(0);
			// }

			writer.WriteByte(0); // Artifact set slots // 152
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.MOVEMENTS))
		{
			writer.WriteInt16(4);
			writer.WriteByte(0); // _player.isInsideZone(ZoneId.WATER) ? 1 : _player.isFlyingMounted() ? 2 : 0
			writer.WriteBoolean(true); // is running
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.COLOR))
		{
			writer.WriteInt16(10);
			writer.WriteInt32(character.NameColor);
			writer.WriteInt32(character.TitleColor);
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.INVENTORY_LIMIT))
		{
			writer.WriteInt16(13);
			writer.WriteInt16(0);
			writer.WriteInt16(0);
			writer.WriteInt16(100); // inventory limit
			writer.WriteByte(0); // is cursed weapon equipped _player.isCursedWeaponEquipped() ? CursedWeaponsManager.getInstance().getLevel(_player.getCursedWeaponEquippedId()) : 0
			writer.WriteByte(0); // 196
			writer.WriteByte(0); // 196
			writer.WriteByte(0); // 196
			writer.WriteByte(0); // 196
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.TRUE_HERO))
		{
			writer.WriteInt16(9);
			writer.WriteInt32(0);
			writer.WriteInt16(0);
			writer.WriteByte(0); // is true hero =_player.isTrueHero() ? 100 : 0
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.ATT_SPIRITS)) // 152
		{
			writer.WriteInt16(34);
			writer.WriteInt32(0); // fire spirit attack
			writer.WriteInt32(0); // water spirit attack
			writer.WriteInt32(0); // wind spirit attack
			writer.WriteInt32(0); // earth spirit attack
			writer.WriteInt32(0); // fire spirit defense
			writer.WriteInt32(0); // water spirit defense
			writer.WriteInt32(0); // wind spirit defense
			writer.WriteInt32(0); // earth spirit defense
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.RANKING)) // 196
		{
			writer.WriteInt16(6);
			writer.WriteInt32(0); // RankManager.getInstance().getPlayerGlobalRank(_player) == 1 ? 1 : RankManager.getInstance().getPlayerRaceRank(_player) == 1 ? 2 : 0
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.STAT_POINTS)) // 235
		{
			writer.WriteInt16(16);
			writer.WriteInt16(0); // Usable points (Elixirs) = _player.getLevel() < 76 ? 0 : (_player.getLevel() - 75) + _player.getVariables().getInt(PlayerVariables.ELIXIRS_AVAILABLE, 0) + (int) _player.getStat().getValue(Stat.ELIXIR_USAGE_LIMIT, 0)
			writer.WriteInt16((short)baseStats.Str); // STR
			writer.WriteInt16((short)baseStats.Dex); // DEX
			writer.WriteInt16((short)baseStats.Con); // CON
			writer.WriteInt16((short)baseStats.Int); // INT
			writer.WriteInt16((short)baseStats.Wit); // WIT
			writer.WriteInt16((short)baseStats.Men); // MEN
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.STAT_ABILITIES)) // 235
		{
			writer.WriteInt16(18);
			writer.WriteInt16(0); // additional STR
			writer.WriteInt16(0); // additional DEX
			writer.WriteInt16(0); // additional CON
			writer.WriteInt16(0); // additional INT
			writer.WriteInt16(0); // additional WIT
			writer.WriteInt16(0); // additional MEN
			writer.WriteInt16(0);
			writer.WriteInt16(0);
		}
		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.ELIXIR_USED)) // 286
		{
			writer.WriteInt16(0); // count : _player.getVariables().getInt(PlayerVariables.ELIXIRS_AVAILABLE, 0)
			writer.WriteInt16(0);
		}

		if (UserInfoUtil.HasComponent(mask, UserInfoComponent.VANGUARD_MOUNT)) // 362
		{
			writer.WriteByte(1); // 362 - Vanguard mount. // _player.getClassId().level() + 1
		}

		// C4
		// writer.WriteInt32(location.X); // X
		// writer.WriteInt32(location.Y); // Y
		// writer.WriteInt32(location.Z); // Z
		// writer.WriteInt32(0); // Boat Id
		// writer.WriteInt32(123); // player object id
		// writer.WriteString("Cassidi"); // name
		// writer.WriteInt32((int)Race.Human); // race
		// writer.WriteInt32((int)Sex.Female); // sex: 0 - male, 1 - female
		// writer.WriteInt32((int)CharacterClass.Fighter);
		// writer.WriteInt32(5); // Level
		// writer.WriteInt32((int)exp); // EXP
		// writer.WriteInt32(baseStats.Str); // STR
		// writer.WriteInt32(baseStats.Dex); // DEX
		// writer.WriteInt32(baseStats.Con); // CON
		// writer.WriteInt32(baseStats.Int); // INT
		// writer.WriteInt32(baseStats.Wit); // WIT
		// writer.WriteInt32(baseStats.Men); // MEN
		// writer.WriteInt32(400); // max HP
		// writer.WriteInt32(400); // current HP
		// writer.WriteInt32(300); // max MP
		// writer.WriteInt32(300); // current MP
		// writer.WriteInt32(0); // SP
		// writer.WriteInt32(200000); // Current Load (items weight)
		// writer.WriteInt32(500000); // max load
		// writer.WriteInt32(20); // active weapon: 20 no weapon, 40 weapon equipped
		//
		// // inventory
		// for (int i = 0; i < 16; i++)
		// 	writer.WriteInt32(0); // object id
		//
		// for (int i = 0; i < 16; i++)
		// 	writer.WriteInt32(0); // item id
		//
		// // writer.WriteInt32(_inventory.getPaperdollObjectId(Inventory.PAPERDOLL_UNDER));
		// // writer.WriteInt32(_inventory.getPaperdollObjectId(Inventory.PAPERDOLL_REAR));
		// // writer.WriteInt32(_inventory.getPaperdollObjectId(Inventory.PAPERDOLL_LEAR));
		// // writer.WriteInt32(_inventory.getPaperdollObjectId(Inventory.PAPERDOLL_NECK));
		// // writer.WriteInt32(_inventory.getPaperdollObjectId(Inventory.PAPERDOLL_RFINGER));
		// // writer.WriteInt32(_inventory.getPaperdollObjectId(Inventory.PAPERDOLL_LFINGER));
		// // writer.WriteInt32(_inventory.getPaperdollObjectId(Inventory.PAPERDOLL_HEAD));
		// // writer.WriteInt32(_inventory.getPaperdollObjectId(Inventory.PAPERDOLL_RHAND));
		// // writer.WriteInt32(_inventory.getPaperdollObjectId(Inventory.PAPERDOLL_LHAND));
		// // writer.WriteInt32(_inventory.getPaperdollObjectId(Inventory.PAPERDOLL_GLOVES));
		// // writer.WriteInt32(_inventory.getPaperdollObjectId(Inventory.PAPERDOLL_CHEST));
		// // writer.WriteInt32(_inventory.getPaperdollObjectId(Inventory.PAPERDOLL_LEGS));
		// // writer.WriteInt32(_inventory.getPaperdollObjectId(Inventory.PAPERDOLL_FEET));
		// // writer.WriteInt32(_inventory.getPaperdollObjectId(Inventory.PAPERDOLL_BACK));
		// // writer.WriteInt32(_inventory.getPaperdollObjectId(Inventory.PAPERDOLL_LRHAND));
		// // writer.WriteInt32(_inventory.getPaperdollObjectId(Inventory.PAPERDOLL_HAIR));
		// //
		// // writer.WriteInt32(_inventory.getPaperdollItemId(Inventory.PAPERDOLL_UNDER));
		// // writer.WriteInt32(_inventory.getPaperdollItemId(Inventory.PAPERDOLL_REAR));
		// // writer.WriteInt32(_inventory.getPaperdollItemId(Inventory.PAPERDOLL_LEAR));
		// // writer.WriteInt32(_inventory.getPaperdollItemId(Inventory.PAPERDOLL_NECK));
		// // writer.WriteInt32(_inventory.getPaperdollItemId(Inventory.PAPERDOLL_RFINGER));
		// // writer.WriteInt32(_inventory.getPaperdollItemId(Inventory.PAPERDOLL_LFINGER));
		// // writer.WriteInt32(_inventory.getPaperdollItemId(Inventory.PAPERDOLL_HEAD));
		// // writer.WriteInt32(_inventory.getPaperdollItemId(Inventory.PAPERDOLL_RHAND));
		// // writer.WriteInt32(_inventory.getPaperdollItemId(Inventory.PAPERDOLL_LHAND));
		// // writer.WriteInt32(_inventory.getPaperdollItemId(Inventory.PAPERDOLL_GLOVES));
		// // writer.WriteInt32(_inventory.getPaperdollItemId(Inventory.PAPERDOLL_CHEST));
		// // writer.WriteInt32(_inventory.getPaperdollItemId(Inventory.PAPERDOLL_LEGS));
		// // writer.WriteInt32(_inventory.getPaperdollItemId(Inventory.PAPERDOLL_FEET));
		// // writer.WriteInt32(_inventory.getPaperdollItemId(Inventory.PAPERDOLL_BACK));
		// // writer.WriteInt32(_inventory.getPaperdollItemId(Inventory.PAPERDOLL_LRHAND));
		// // writer.WriteInt32(_inventory.getPaperdollItemId(Inventory.PAPERDOLL_HAIR));
		//
		// writer.WriteInt32(baseStats.PAtk); // PAtk
		// writer.WriteInt32(baseStats.PAtkSpd); // PAtkSpeed
		// writer.WriteInt32(baseStats.PDef); // PDef
		// writer.WriteInt32(20); // Evasion rate
		// writer.WriteInt32(25); // accuracy
		// writer.WriteInt32(baseStats.CritRate); // critical hit
		// writer.WriteInt32(baseStats.MAtk); // MAtk
		// writer.WriteInt32(baseStats.MAtkSpd); // MAtkSpeed
		// writer.WriteInt32(48); // PAtkSpeed
		// writer.WriteInt32(360); // MDef
		// writer.WriteInt32(0); // PVP flag: 0-non-pvp 1-pvp = violett name
		// writer.WriteInt32(0); // Karma
		// writer.WriteInt32(baseStats.RunSpd); // base run speed
		// writer.WriteInt32(108); // base walk speed
		// writer.WriteInt32(159); // swim run speed
		// writer.WriteInt32(108); // swim walk speed
		// writer.WriteInt32(0);
		// writer.WriteInt32(0);
		// writer.WriteInt32(159); // fly run speed
		// writer.WriteInt32(108); // fly walk speed
		// writer.WriteDouble(1.0); // run speed multiplier
		// writer.WriteDouble(1.0); // attack speed multiplier
		// writer.WriteDouble((double)dimensions.Radius); // collision radius
		// writer.WriteDouble((double)dimensions.Height); // collision height
		// writer.WriteInt32(0); // hair style
		// writer.WriteInt32(0); // hair color
		// writer.WriteInt32(0); // face style
		// writer.WriteInt32(0); // builder level (isGM)
		// writer.WriteString("Title"); // title
		//
		// writer.WriteInt32(0); // clan id
		// writer.WriteInt32(0); // clan crest id
		// writer.WriteInt32(0); // ally id
		// writer.WriteInt32(0); // ally crest id
		//
		// writer.WriteInt32(0); // siege flags: 0x40 leader rights, attacker - 0x180 sword over name, defender - 0x80 shield, 0xC0 crown (|leader), 0x1C0 flag (|leader)
		// writer.WriteByte(0); // mount type
		// writer.WriteByte(0); // private store type
		// writer.WriteByte(0); // has dwarven craft
		// writer.WriteInt32(0); // pk kills
		// writer.WriteInt32(0); // pvp kills
		//
		// writer.WriteInt16(0); // cubic count
		// //writer.WriteInt16(0); // cubic id for each
		//
		// writer.WriteByte(0); // is in party match room
		// writer.WriteInt32(0); // abnormal effect
		// writer.WriteByte(0);
		// writer.WriteInt32(0); // clan privileges
		// // C4 addition
		// writer.WriteInt32(0); // swim?
		// writer.WriteInt32(0);
		// writer.WriteInt32(0);
		// writer.WriteInt32(0);
		// writer.WriteInt32(0);
		// writer.WriteInt32(0);
		// writer.WriteInt32(0);
		// // C4 addition end
		// writer.WriteInt16(0); // c2 recommendations remaining
		// writer.WriteInt16(0); // c2 recommendations received
		// writer.WriteInt32(0); // _player.getMountNpcId() > 0 ? _player.getMountNpcId() + 1000000 : 0
		// writer.WriteInt16(0); // inventory limit
		// writer.WriteInt32((int)CharacterClass.Fighter); // class id
		// writer.WriteInt32(0); // special effects? circles around player...
		// writer.WriteInt32(600); // max CP
		// writer.WriteInt32(550); // current CP
		// writer.WriteByte(0); //_player.isMounted() ? 0 : _player.getEnchantEffect());
		// writer.WriteByte(0); // team circle around feet 1= Blue, 2 = red
		// writer.WriteInt32(0); // clan crest large id
		// writer.WriteByte(0); // is noble - 1: symbol on char menu ctrl+I
		// writer.WriteByte(0); // 1: Hero Aura
		//
		// writer.WriteByte(0); // Fishing Mode
		// writer.WriteInt32(0); // fishing x
		// writer.WriteInt32(0); // fishing y
		// writer.WriteInt32(0); // fishing z
		//
		// writer.WriteInt32(0); // name color
		// // Add heading?
    }
}


internal enum UserInfoComponent
{
	RELATION = 0x00,
	BASIC_INFO = 0x01,
	BASE_STATS = 0x02,
	MAX_HPCPMP = 0x03,
	CURRENT_HPMPCP_EXP_SP = 0x04,
	ENCHANTLEVEL = 0x05,
	APPAREANCE = 0x06,
	STATUS = 0x07,

	STATS = 0x08,
	ELEMENTALS = 0x09,
	POSITION = 0x0A,
	SPEED = 0x0B,
	MULTIPLIER = 0x0C,
	COL_RADIUS_HEIGHT = 0x0D,
	ATK_ELEMENTAL = 0x0E,
	CLAN = 0x0F,

	SOCIAL = 0x10,
	VITA_FAME = 0x11,
	SLOTS = 0x12,
	MOVEMENTS = 0x13,
	COLOR = 0x14,
	INVENTORY_LIMIT = 0x15,
	TRUE_HERO = 0x16,

	ATT_SPIRITS = 0x17,

	RANKING = 0x18,

	STAT_POINTS = 0x19,
	STAT_ABILITIES = 0x1A,

	ELIXIR_USED = 0x1B,

	VANGUARD_MOUNT = 0x1C,
	UNK_414 = 0x1D,
}

internal static class UserInfoUtil
{
	private static readonly byte[] _defaultFlagArray = [0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01];

	public static void AddComponent(byte[] mask, UserInfoComponent component)
	{
		int bit = (int)component;
		mask[bit >> 3] |= _defaultFlagArray[bit & 7];
	}

	public static bool HasComponent(byte[] mask, UserInfoComponent component)
	{
		int bit = (int)component;
		return (mask[bit >> 3] & _defaultFlagArray[bit & 7]) != 0;
	}

	public static int GetSize(this UserInfoComponent component) =>
		component switch
		{
			UserInfoComponent.RELATION => 4,
			UserInfoComponent.BASIC_INFO => 23,
			UserInfoComponent.BASE_STATS => 18,
			UserInfoComponent.MAX_HPCPMP => 14,
			UserInfoComponent.CURRENT_HPMPCP_EXP_SP => 39,
			UserInfoComponent.ENCHANTLEVEL => 5,
			UserInfoComponent.APPAREANCE => 19,
			UserInfoComponent.STATUS => 6,

			UserInfoComponent.STATS => 64,
			UserInfoComponent.ELEMENTALS => 14,
			UserInfoComponent.POSITION => 18,
			UserInfoComponent.SPEED => 18,
			UserInfoComponent.MULTIPLIER => 18,
			UserInfoComponent.COL_RADIUS_HEIGHT => 18,
			UserInfoComponent.ATK_ELEMENTAL => 5,
			UserInfoComponent.CLAN => 32,

			UserInfoComponent.SOCIAL => 34,
			UserInfoComponent.VITA_FAME => 19,
			UserInfoComponent.SLOTS => 12,
			UserInfoComponent.MOVEMENTS => 4,
			UserInfoComponent.COLOR => 10,
			UserInfoComponent.INVENTORY_LIMIT => 13,
			UserInfoComponent.TRUE_HERO => 9,

			UserInfoComponent.ATT_SPIRITS => 34,

			UserInfoComponent.RANKING => 6,

			UserInfoComponent.STAT_POINTS => 16,
			UserInfoComponent.STAT_ABILITIES => 18,

			UserInfoComponent.ELIXIR_USED => 1,

			UserInfoComponent.VANGUARD_MOUNT => 1,
			UserInfoComponent.UNK_414 => 1,
			_ => throw new ArgumentOutOfRangeException()
		};
}
