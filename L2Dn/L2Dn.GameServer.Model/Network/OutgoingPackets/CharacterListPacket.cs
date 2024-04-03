using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct CharacterListPacket(int playKey1, string accountName, CharacterInfoList characters)
	: IOutgoingPacket
{
	private static readonly int[] _paperdollOrder =
	[
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
		Inventory.PAPERDOLL_AGATHION1, // 152
		Inventory.PAPERDOLL_AGATHION2, // 152
		Inventory.PAPERDOLL_AGATHION3, // 152
		Inventory.PAPERDOLL_AGATHION4, // 152
		Inventory.PAPERDOLL_AGATHION5, // 152
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
		Inventory.PAPERDOLL_ARTIFACT_BOOK, // 152
		Inventory.PAPERDOLL_ARTIFACT1, // 152
		Inventory.PAPERDOLL_ARTIFACT2, // 152
		Inventory.PAPERDOLL_ARTIFACT3, // 152
		Inventory.PAPERDOLL_ARTIFACT4, // 152
		Inventory.PAPERDOLL_ARTIFACT5, // 152
		Inventory.PAPERDOLL_ARTIFACT6, // 152
		Inventory.PAPERDOLL_ARTIFACT7, // 152
		Inventory.PAPERDOLL_ARTIFACT8, // 152
		Inventory.PAPERDOLL_ARTIFACT9, // 152
		Inventory.PAPERDOLL_ARTIFACT10, // 152
		Inventory.PAPERDOLL_ARTIFACT11, // 152
		Inventory.PAPERDOLL_ARTIFACT12, // 152
		Inventory.PAPERDOLL_ARTIFACT13, // 152
		Inventory.PAPERDOLL_ARTIFACT14, // 152
		Inventory.PAPERDOLL_ARTIFACT15, // 152
		Inventory.PAPERDOLL_ARTIFACT16, // 152
		Inventory.PAPERDOLL_ARTIFACT17, // 152
		Inventory.PAPERDOLL_ARTIFACT18, // 152
		Inventory.PAPERDOLL_ARTIFACT19, // 152
		Inventory.PAPERDOLL_ARTIFACT20, // 152
		Inventory.PAPERDOLL_ARTIFACT21 // 152
	];
	private static readonly int[] _paperdollOrderVisualId =
	[
		Inventory.PAPERDOLL_RHAND,
		Inventory.PAPERDOLL_LHAND,
		Inventory.PAPERDOLL_GLOVES,
		Inventory.PAPERDOLL_CHEST,
		Inventory.PAPERDOLL_LEGS,
		Inventory.PAPERDOLL_FEET,
		Inventory.PAPERDOLL_RHAND,
		Inventory.PAPERDOLL_HAIR,
		Inventory.PAPERDOLL_HAIR2
	];

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.CHARACTER_SELECTION_INFO);
		
		int size = characters.Count;
		writer.WriteInt32(size); // Created character count
		writer.WriteInt32(Config.MAX_CHARACTERS_NUMBER_PER_ACCOUNT); // Can prevent players from creating new characters (if 0); (if 1, the client will ask if chars may be created (0x13) Response: (0x0D) )
		writer.WriteByte(size == Config.MAX_CHARACTERS_NUMBER_PER_ACCOUNT); // if 1 can't create new char
		writer.WriteByte(2); // 0=can't play, 1=can play free until level 85, 2=100% free play
		writer.WriteInt32(2); // if 1, Korean client
		writer.WriteByte(0); // Gift message for inactive accounts // 152
		writer.WriteByte(0); // Balthus Knights, if 1 suggests premium account
		
		if (size == 0)
			return;
        
		for (int i = 0; i < size; i++)
		{
			CharacterInfo charInfo = characters.Characters[i];
			writer.WriteString(charInfo.Name); // Character name
			writer.WriteInt32(charInfo.Id); // Character ID
			writer.WriteString(accountName); // Account name
			writer.WriteInt32(playKey1); // Account ID
			writer.WriteInt32(0); // Clan ID
			writer.WriteInt32(0); // Builder level
			writer.WriteInt32((int)charInfo.Sex); // Sex
			writer.WriteInt32((int)charInfo.Race); // Race
			writer.WriteInt32((int)charInfo.BaseClass);
			writer.WriteInt32(Config.SERVER_ID);
			writer.WriteInt32(charInfo.X);
			writer.WriteInt32(charInfo.Y);
			writer.WriteInt32(charInfo.Z);
			writer.WriteDouble(charInfo.CurrentHp);
			writer.WriteDouble(charInfo.CurrentMp);
			writer.WriteInt64(charInfo.Sp);
			writer.WriteInt64(charInfo.Exp);
			
			writer.WriteDouble(1.0 *
			                   (charInfo.Exp - ExperienceData.getInstance()
				                   .getExpForLevel(charInfo.Level)) /
			                   (ExperienceData.getInstance().getExpForLevel(charInfo.Level + 1) -
			                    ExperienceData.getInstance().getExpForLevel(charInfo.Level)));
			
			writer.WriteInt32(charInfo.Level);
			writer.WriteInt32(charInfo.Reputation);
			writer.WriteInt32(charInfo.PkKills);
			writer.WriteInt32(charInfo.PvpKills);
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0); // Ertheia
			writer.WriteInt32(0); // Ertheia
			
			foreach (int slot in _paperdollOrder)
				writer.WriteInt32(charInfo.Paperdoll[slot].ItemId);

			foreach (int slot in _paperdollOrderVisualId)
				writer.WriteInt32(charInfo.Paperdoll[slot].ItemVisualId);
			
			writer.WriteInt16((short)charInfo.ChestEnchantLevel); // Upper Body enchant level
			writer.WriteInt16((short)charInfo.LegsEnchantLevel); // Lower Body enchant level
			writer.WriteInt16((short)charInfo.HeadEnchantLevel); // Headgear enchant level
			writer.WriteInt16((short)charInfo.GlovesEnchantLevel); // Gloves enchant level
			writer.WriteInt16((short)charInfo.BootsEnchantLevel); // Boots enchant level
			writer.WriteInt32(charInfo.HairStyle);
			writer.WriteInt32(charInfo.HairColor);
			writer.WriteInt32(charInfo.Face);
			writer.WriteDouble(charInfo.MaxHp); // Maximum HP
			writer.WriteDouble(charInfo.MaxMp); // Maximum MP

			DateTime? deleteTime = charInfo.DeleteTime;
			if (deleteTime != null)
				writer.WriteInt32((int)(deleteTime.Value - DateTime.UtcNow).TotalSeconds);
			else
				writer.WriteInt32(0);
			
			writer.WriteInt32((int)charInfo.Class);
			writer.WriteInt32(i == characters.SelectedIndex);
			writer.WriteByte((byte)(charInfo.WeaponEnchantLevel > 127 ? 127 : charInfo.WeaponEnchantLevel));

			writer.WriteInt32(charInfo.WeaponAugmentationOption1Id); // Weapon augmentation options
			writer.WriteInt32(charInfo.WeaponAugmentationOption2Id);

			writer.WriteInt32(0); // Transformation: Currently on retail when you are on character select you don't see your transformation.
			writer.WriteInt32(0); // Pet NpcId
			writer.WriteInt32(0); // Pet level
			writer.WriteInt32(0); // Pet Food
			writer.WriteInt32(0); // Pet Food Level
			writer.WriteDouble(0); // Current pet HP
			writer.WriteDouble(0); // Current pet MP
			writer.WriteInt32(charInfo.VitalityPoints); // Vitality
			writer.WriteInt32((int)Config.RATE_VITALITY_EXP_MULTIPLIER * 100); // Vitality Percent
			writer.WriteInt32(charInfo.VitalityItemsUsed); // Remaining vitality item uses
			writer.WriteInt32(charInfo.AccessLevel != -100); // Char is active or not // TODO add database field
			writer.WriteByte(charInfo.IsNoble);
			writer.WriteByte((byte)(Hero.getInstance().isHero(charInfo.Id) ? 2 : 0)); // Hero glow
			writer.WriteByte(charInfo.HairAccessoryEnabled); // Show hair accessory if enabled
			writer.WriteInt32(0); // 235 - ban time left
			writer.WriteInt32(charInfo.LastAccessTime?.getEpochSecond() ?? 0); // 235 - last play time
			writer.WriteByte(0); // 338
			writer.WriteInt32(charInfo.HairColor + 1); // 338 - DK color.

			writer.WriteByte((byte)(charInfo.Class == CharacterClass.ORC_LANCER ? 1 :
				charInfo.Class == CharacterClass.RIDER ? 2 :
				charInfo.Class == CharacterClass.DRAGOON ? 3 :
				charInfo.Class == CharacterClass.VANGUARD_RIDER ? 4 : 0)); // 362
		}
	}
}