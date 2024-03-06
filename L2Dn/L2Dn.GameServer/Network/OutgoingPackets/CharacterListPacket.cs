using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct CharacterListPacket: IOutgoingPacket
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CharacterListPacket));
	
	private static readonly int[] PAPERDOLL_ORDER =
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
	private static readonly int[] PAPERDOLL_ORDER_VISUAL_ID =
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

	private readonly int _accountId;
	private readonly string _accountName;
	private readonly int _activeId;
	private readonly ImmutableArray<CharSelectInfoPackage> _characterPackages;

	public CharacterListPacket(int accountId, string accountName, ImmutableArray<CharSelectInfoPackage> characters,
		int activeId = -1)
	{
		_accountId = accountId;
		_accountName = accountName;
		_characterPackages = characters;
		_activeId = activeId;
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.CHARACTER_SELECTION_INFO);
		
		int size = _characterPackages.Length;
		writer.WriteInt32(size); // Created character count
		writer.WriteInt32(Config.MAX_CHARACTERS_NUMBER_PER_ACCOUNT); // Can prevent players from creating new characters (if 0); (if 1, the client will ask if chars may be created (0x13) Response: (0x0D) )
		writer.WriteByte(size == Config.MAX_CHARACTERS_NUMBER_PER_ACCOUNT); // if 1 can't create new char
		writer.WriteByte(1); // 0=can't play, 1=can play free until level 85, 2=100% free play
		writer.WriteInt32(2); // if 1, Korean client
		writer.WriteByte(0); // Gift message for inactive accounts // 152
		writer.WriteByte(0); // Balthus Knights, if 1 suggests premium account
		
		DateTime? lastAccess = default;
		int activeId = _activeId;
		if (activeId == -1)
		{
			for (int i = 0; i < size; i++)
			{
				if (lastAccess < _characterPackages[i].getLastAccess())
				{
					lastAccess = _characterPackages[i].getLastAccess();
					activeId = i;
				}
			}
		}
		
		for (int i = 0; i < size; i++)
		{
			CharSelectInfoPackage charInfoPackage = _characterPackages[i];
			writer.WriteString(charInfoPackage.getName()); // Character name
			writer.WriteInt32(charInfoPackage.getObjectId()); // Character ID
			writer.WriteString(_accountName); // Account name
			writer.WriteInt32(_accountId); // Account ID
			writer.WriteInt32(0); // Clan ID
			writer.WriteInt32(0); // Builder level
			writer.WriteInt32((int)charInfoPackage.getSex()); // Sex
			writer.WriteInt32((int)charInfoPackage.getRace()); // Race
			writer.WriteInt32((int)charInfoPackage.getBaseClassId());
			writer.WriteInt32(Config.SERVER_ID);
			writer.WriteInt32(charInfoPackage.getX());
			writer.WriteInt32(charInfoPackage.getY());
			writer.WriteInt32(charInfoPackage.getZ());
			writer.WriteDouble(charInfoPackage.getCurrentHp());
			writer.WriteDouble(charInfoPackage.getCurrentMp());
			writer.WriteInt64(charInfoPackage.getSp());
			writer.WriteInt64(charInfoPackage.getExp());
			writer.WriteDouble((float) (charInfoPackage.getExp() - ExperienceData.getInstance().getExpForLevel(charInfoPackage.getLevel())) / (ExperienceData.getInstance().getExpForLevel(charInfoPackage.getLevel() + 1) - ExperienceData.getInstance().getExpForLevel(charInfoPackage.getLevel())));
			writer.WriteInt32(charInfoPackage.getLevel());
			writer.WriteInt32(charInfoPackage.getReputation());
			writer.WriteInt32(charInfoPackage.getPkKills());
			writer.WriteInt32(charInfoPackage.getPvPKills());
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0);
			writer.WriteInt32(0); // Ertheia
			writer.WriteInt32(0); // Ertheia
			foreach (int slot in PAPERDOLL_ORDER)
			{
				writer.WriteInt32(charInfoPackage.getPaperdollItemId(slot));
			}
			foreach (int slot in PAPERDOLL_ORDER_VISUAL_ID)
			{
				writer.WriteInt32(charInfoPackage.getPaperdollItemVisualId(slot));
			}
			
			writer.WriteInt16((short)charInfoPackage.getEnchantEffect(Inventory.PAPERDOLL_CHEST)); // Upper Body enchant level
			writer.WriteInt16((short)charInfoPackage.getEnchantEffect(Inventory.PAPERDOLL_LEGS)); // Lower Body enchant level
			writer.WriteInt16((short)charInfoPackage.getEnchantEffect(Inventory.PAPERDOLL_HEAD)); // Headgear enchant level
			writer.WriteInt16((short)charInfoPackage.getEnchantEffect(Inventory.PAPERDOLL_GLOVES)); // Gloves enchant level
			writer.WriteInt16((short)charInfoPackage.getEnchantEffect(Inventory.PAPERDOLL_FEET)); // Boots enchant level
			writer.WriteInt32(charInfoPackage.getHairStyle());
			writer.WriteInt32(charInfoPackage.getHairColor());
			writer.WriteInt32(charInfoPackage.getFace());
			writer.WriteDouble(charInfoPackage.getMaxHp()); // Maximum HP
			writer.WriteDouble(charInfoPackage.getMaxMp()); // Maximum MP
			writer.WriteInt32(charInfoPackage.getDeleteTimer() != null ? (int)(charInfoPackage.getDeleteTimer().Value - DateTime.UtcNow).TotalSeconds : 0);
			writer.WriteInt32((int)charInfoPackage.getClassId());
			writer.WriteInt32(i == activeId);
			writer.WriteByte((byte)(charInfoPackage.getEnchantEffect(Inventory.PAPERDOLL_RHAND) > 127 ? 127 : charInfoPackage.getEnchantEffect(Inventory.PAPERDOLL_RHAND)));
			writer.WriteInt32(charInfoPackage.getAugmentation() != null ? charInfoPackage.getAugmentation().getOption1Id() : 0);
			writer.WriteInt32(charInfoPackage.getAugmentation() != null ? charInfoPackage.getAugmentation().getOption2Id() : 0);
			writer.WriteInt32(0); // Transformation: Currently on retail when you are on character select you don't see your transformation.
			writer.WriteInt32(0); // Pet NpcId
			writer.WriteInt32(0); // Pet level
			writer.WriteInt32(0); // Pet Food
			writer.WriteInt32(0); // Pet Food Level
			writer.WriteDouble(0); // Current pet HP
			writer.WriteDouble(0); // Current pet MP
			writer.WriteInt32(charInfoPackage.getVitalityPoints()); // Vitality
			writer.WriteInt32((int) Config.RATE_VITALITY_EXP_MULTIPLIER * 100); // Vitality Percent
			writer.WriteInt32(charInfoPackage.getVitalityItemsUsed()); // Remaining vitality item uses
			writer.WriteInt32(charInfoPackage.getAccessLevel() != -100); // Char is active or not
			writer.WriteByte(charInfoPackage.isNoble());
			writer.WriteByte((byte)(Hero.getInstance().isHero(charInfoPackage.getObjectId()) ? 2 : 0)); // Hero glow
			writer.WriteByte(charInfoPackage.isHairAccessoryEnabled()); // Show hair accessory if enabled
			writer.WriteInt32(0); // 235 - ban time left
			writer.WriteInt32(charInfoPackage.getLastAccess()?.getEpochSecond() ?? 0); // 235 - last play time
			writer.WriteByte(0); // 338
			writer.WriteInt32(charInfoPackage.getHairColor() + 1); // 338 - DK color.
			writer.WriteByte((byte)(charInfoPackage.getClassId() == (CharacterClass)217 ? 1 : charInfoPackage.getClassId() == (CharacterClass)218 ? 2 : charInfoPackage.getClassId() == (CharacterClass)219 ? 3 : charInfoPackage.getClassId() == (CharacterClass)220 ? 4 : 0)); // 362
		}
	}
}