using L2Dn.GameServer.Model.Holders;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Enchanting;

public readonly struct EnchantResultPacket: IOutgoingPacket
{
	public const int SUCCESS = 0;
	/**
	 * if (Type == ITEME_ENCHT_AG || Type == ITEME_BLESS_ENCHT_AG || Type == ITEME_MULTI_ENCHT_AG || Type == ITEME_ANCIENT_CRYSTAL_ENCHANT_AG) The growth failed. The agathion disappeared. else S1_CRYSTALLIZED calculate challenge points
	 */
	public const int FAIL = 1;
	/**
	 * remove scrolls
	 */
	public const int ERROR = 2;
	/**
	 * Deprecated
	 */
	public const int BLESSED_FAIL = 3;
	/**
	 * if (Type == ITEME_ENCHT_AG || Type == ITEME_BLESS_ENCHT_AG || Type == ITEME_MULTI_ENCHT_AG || Type == ITEME_ANCIENT_CRYSTAL_ENCHANT_AG) The growth failed. The agathion disappeared. else CRYSTALLIZED
	 */
	public const int NO_CRYSTAL = 4;
	/**
	 * Deprecated
	 */
	public const int SAFE_FAIL = 5;
	/**
	 * FAILURE - Enchantment failed. You have obtained the listed items.
	 */
	public const int CHALLENGE_ENCHANT_SAFE = 6;
	/**
	 * DECREASE *will show item*
	 */
	public const int DECREASE = 7; // -> set enchant to 0 in UI
	/**
	 * if(isAutoEnchanting || isAutoEnchantingStop) will reuse scroll if (Type == ITEME_ENCHT_AG || Type == ITEME_BLESS_ENCHT_AG || Type == ITEME_MULTI_ENCHT_AG || Type == ITEME_ANCIENT_CRYSTAL_ENCHANT_AG) IN_CASE_OF_FAILURE_THE_AGATHION_S_GROWTH_LEVEL_WILL_REMAIN_THE_SAME else Enchant failed. The
	 * enchant skill for the corresponding item will be exactly retained. *will show item*
	 */
	public const int REMAIN = 8; // -> remaining the same with sys string
	/**
	 * if(Type == ITEME_ENCHT_AG || Type == ITEME_BLESS_ENCHT_AG || Type == ITEME_MULTI_ENCHT_AG || Type == ITEME_ANCIENT_CRYSTAL_ENCHANT_AG) The growth failed. The agathion disappeared. else Enchantment failed. You have obtained the listed items. *will show item*
	 */
	public const int FAILED_WITH_OPTIONS_NO_AND_NO_POINTS = 9;
	/**
	 * if (Type == ITEME_ENCHT_AG || Type == ITEME_BLESS_ENCHT_AG || Type == ITEME_MULTI_ENCHT_AG || Type == ITEME_ANCIENT_CRYSTAL_ENCHANT_AG) The growth failed. The agathion's growth level is reset. else Enchantment failed. You have obtained the listed items. if (isAutoEnchanting ||
	 * isAutoEnchantingStop && SelectItemInfo.Enchanted == EnchantValue) will reuse scroll in blue auto enchant *will show item*
	 */
	public const int SAFE_FAIL_02 = 10;

	private readonly int _result;
	private readonly ItemHolder _crystal;
	private readonly ItemHolder _additional;
	private readonly int _enchantLevel;

	public EnchantResultPacket(int result, ItemHolder? crystal, ItemHolder? additionalItem, int enchantLevel)
	{
		_result = result;
		_crystal = crystal ?? new ItemHolder(0, 0);
		_additional = additionalItem ?? new ItemHolder(0, 0);
		_enchantLevel = enchantLevel;
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.ENCHANT_RESULT);

		writer.WriteInt32(_result);
		writer.WriteInt32(_crystal.getId());
		writer.WriteInt64(_crystal.getCount());
		writer.WriteInt32(_additional.getId());
		writer.WriteInt64(_additional.getCount());
		writer.WriteInt32(_enchantLevel);
	}
}