using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct CharacterListPacket(
	string login,
	int playKey1,
	List<Character> characters,
	Character? lastSelectedCharacter): IOutgoingPacket
{
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WriteByte(0x09); // packet code (0x13 in C4)

		writer.WriteInt32(characters.Count); // character count

		// Can prevent players from creating new characters (if 0); (if 1, the client will ask if chars may be created (0x13) Response: (0x0D) )
		writer.WriteInt32(7); // max characters per account
		writer.WriteByte(0); // if 1 can't create new char
		writer.WriteByte(2); // 0=can't play, 1=can play free until level 85, 2=100% free play
		writer.WriteInt32(2); // if 1, Korean client
		writer.WriteByte(0); // Gift message for inactive accounts // 152
		writer.WriteByte(0); // Balthus Knights, if 1 suggests premium account

		foreach (Character character in characters)
		{
			bool selected = character == lastSelectedCharacter;
			CharacterClass characterClass = character.Class;
			CharacterClassInfo classInfo = StaticData.Templates[characterClass];
			(int level, decimal percents) = StaticData.Levels.GetLevelForExp(character.Exp);

			writer.WriteString(character.Name); // character name
			writer.WriteInt32(character.Id); // character id
			writer.WriteString(login); // login
			writer.WriteInt32(playKey1);
			writer.WriteInt32(0); // clan id
			writer.WriteInt32(0); // builder level ???
			writer.WriteInt32((int)character.Sex); // sex
			writer.WriteInt32((int)classInfo.Race); // race
			writer.WriteInt32((int)classInfo.BaseClass.Class); // base class id
			writer.WriteInt32(1); // server id

			writer.WriteInt32(character.LocationX); // x
			writer.WriteInt32(character.LocationY); // y
			writer.WriteInt32(character.LocationZ); // z
			writer.WriteDouble(character.CurrentHp); // current HP
			writer.WriteDouble(character.CurrentMp); // current MP
			writer.WriteInt64(character.Sp); // SP
			writer.WriteInt64(character.Exp); // EXP
			writer.WriteDouble((double)percents); // percents
			writer.WriteInt32(level); // level
			writer.WriteInt32(character.Reputation); // karma
			writer.WriteInt32(character.PkCounter); // pk kills
			writer.WriteInt32(character.PvpCounter); // pvp kills
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0); // Ertheia
			writer.WriteInt32(0); // Ertheia

			for (int i = 0; i < 60; i++)
				writer.WriteInt32(0); // clothes

			for (int i = 0; i < 9; i++)
				writer.WriteInt32(0); // clothes

			// writer.WriteInt32(charInfoPackage.getPaperdollObjectId(Inventory.PAPERDOLL_UNDER));
			// writer.WriteInt32(charInfoPackage.getPaperdollObjectId(Inventory.PAPERDOLL_REAR));
			// writer.WriteInt32(charInfoPackage.getPaperdollObjectId(Inventory.PAPERDOLL_LEAR));
			// writer.WriteInt32(charInfoPackage.getPaperdollObjectId(Inventory.PAPERDOLL_NECK));
			// writer.WriteInt32(charInfoPackage.getPaperdollObjectId(Inventory.PAPERDOLL_RFINGER));
			// writer.WriteInt32(charInfoPackage.getPaperdollObjectId(Inventory.PAPERDOLL_LFINGER));
			// writer.WriteInt32(charInfoPackage.getPaperdollObjectId(Inventory.PAPERDOLL_HEAD));
			// writer.WriteInt32(charInfoPackage.getPaperdollObjectId(Inventory.PAPERDOLL_RHAND));
			// writer.WriteInt32(charInfoPackage.getPaperdollObjectId(Inventory.PAPERDOLL_LHAND));
			// writer.WriteInt32(charInfoPackage.getPaperdollObjectId(Inventory.PAPERDOLL_GLOVES));
			// writer.WriteInt32(charInfoPackage.getPaperdollObjectId(Inventory.PAPERDOLL_CHEST));
			// writer.WriteInt32(charInfoPackage.getPaperdollObjectId(Inventory.PAPERDOLL_LEGS));
			// writer.WriteInt32(charInfoPackage.getPaperdollObjectId(Inventory.PAPERDOLL_FEET));
			// writer.WriteInt32(charInfoPackage.getPaperdollObjectId(Inventory.PAPERDOLL_BACK));
			// writer.WriteInt32(charInfoPackage.getPaperdollObjectId(Inventory.PAPERDOLL_LRHAND));
			// writer.WriteInt32(charInfoPackage.getPaperdollObjectId(Inventory.PAPERDOLL_HAIR));
			// writer.WriteInt32(charInfoPackage.getPaperdollItemId(Inventory.PAPERDOLL_UNDER));
			// writer.WriteInt32(charInfoPackage.getPaperdollItemId(Inventory.PAPERDOLL_REAR));
			// writer.WriteInt32(charInfoPackage.getPaperdollItemId(Inventory.PAPERDOLL_LEAR));
			// writer.WriteInt32(charInfoPackage.getPaperdollItemId(Inventory.PAPERDOLL_NECK));
			// writer.WriteInt32(charInfoPackage.getPaperdollItemId(Inventory.PAPERDOLL_RFINGER));
			// writer.WriteInt32(charInfoPackage.getPaperdollItemId(Inventory.PAPERDOLL_LFINGER));
			// writer.WriteInt32(charInfoPackage.getPaperdollItemId(Inventory.PAPERDOLL_HEAD));
			// writer.WriteInt32(charInfoPackage.getPaperdollItemId(Inventory.PAPERDOLL_RHAND));
			// writer.WriteInt32(charInfoPackage.getPaperdollItemId(Inventory.PAPERDOLL_LHAND));
			// writer.WriteInt32(charInfoPackage.getPaperdollItemId(Inventory.PAPERDOLL_GLOVES));
			// writer.WriteInt32(charInfoPackage.getPaperdollItemId(Inventory.PAPERDOLL_CHEST));
			// writer.WriteInt32(charInfoPackage.getPaperdollItemId(Inventory.PAPERDOLL_LEGS));
			// writer.WriteInt32(charInfoPackage.getPaperdollItemId(Inventory.PAPERDOLL_FEET));
			// writer.WriteInt32(charInfoPackage.getPaperdollItemId(Inventory.PAPERDOLL_BACK));
			// writer.WriteInt32(charInfoPackage.getPaperdollItemId(Inventory.PAPERDOLL_LRHAND));
			// writer.WriteInt32(charInfoPackage.getPaperdollItemId(Inventory.PAPERDOLL_HAIR));

			writer.WriteInt16(0); // Upper Body enchant level
			writer.WriteInt16(0); // Lower Body enchant level
			writer.WriteInt16(0); // Headgear enchant level
			writer.WriteInt16(0); // Gloves enchant level
			writer.WriteInt16(0); // Boots enchant level

			writer.WriteInt32(character.HairStyle); // hair style
			writer.WriteInt32(character.HairColor); // hair color
			writer.WriteInt32(character.Face); // face
			writer.WriteDouble(character.MaxHp); // hp max
			writer.WriteDouble(character.MaxMp); // mp max
			writer.WriteInt32(0); // seconds left before character deletion (0 - not deleting, -1 - char banned)
			writer.WriteInt32((int)characterClass); // class id
			writer.WriteInt32(selected ? 1 : 0); // 1 - selected, 0 - not selected (c3 auto-select char)
			writer.WriteByte(0); // enchant effect 0..127
			writer.WriteInt32(0); // augmentation options 1
			writer.WriteInt32(0); // augmentation options 2
			writer.WriteInt32(
				0); // Transformation: Currently on retail when you are on character select you don't see your transformation.
			writer.WriteInt32(0); // Pet NpcId
			writer.WriteInt32(0); // Pet level
			writer.WriteInt32(0); // Pet Food
			writer.WriteInt32(0); // Pet Food Level
			writer.WriteDouble(0); // Current pet HP
			writer.WriteDouble(0); // Current pet MP
			writer.WriteInt32(0); // Vitality points
			writer.WriteInt32(0); // Vitality Percent : 100% for example
			writer.WriteInt32(0); // Remaining vitality item uses
			writer.WriteInt32(1); // Char is active or not
			writer.WriteBoolean(false);
			writer.WriteByte(0); // Hero glow: 2 - yes, 0 - no
			writer.WriteBoolean(true); // Show hair accessory if enabled
			writer.WriteInt32(0); // 235 - ban time left
			writer.WriteInt32(0); // 235 - last play time  =((int) (charInfoPackage.getLastAccess() / 1000)
			writer.WriteByte(0); // 338
			writer.WriteInt32(1); // 338 - DK color (hair color + 1)
			writer.WriteByte(
				0); // 362: charInfoPackage.getClassId() == 217 ? 1 : charInfoPackage.getClassId() == 218 ? 2 : charInfoPackage.getClassId() == 219 ? 3 : charInfoPackage.getClassId() == 220 ? 4 : 0
		}
	}
}
