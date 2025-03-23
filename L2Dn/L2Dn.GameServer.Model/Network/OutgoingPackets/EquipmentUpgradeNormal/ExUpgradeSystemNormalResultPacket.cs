using L2Dn.GameServer.Model.Holders;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.EquipmentUpgradeNormal;

public readonly struct ExUpgradeSystemNormalResultPacket: IOutgoingPacket
{
    public static readonly ExUpgradeSystemNormalResultPacket FAIL = new(0, 0, false, [], []);

    private readonly int _result;
    private readonly int _upgradeId;
    private readonly bool _success;
    private readonly List<UniqueItemEnchantHolder> _resultItems;
    private readonly List<UniqueItemEnchantHolder> _bonusItems;

    public ExUpgradeSystemNormalResultPacket(int result, int upgradeId, bool success, List<UniqueItemEnchantHolder> resultItems, List<UniqueItemEnchantHolder> bonusItems)
    {
        _result = result;
        _upgradeId = upgradeId;
        _success = success;
        _resultItems = resultItems;
        _bonusItems = bonusItems;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_UPGRADE_SYSTEM_NORMAL_RESULT);

        writer.WriteInt16((short)_result); // Result ID
        writer.WriteInt32(_upgradeId); // Upgrade ID
        writer.WriteByte(_success); // Success
        writer.WriteInt32(_resultItems.Count); // Array of result items (success/failure) start.
        foreach (UniqueItemEnchantHolder item in _resultItems)
        {
            writer.WriteInt32(item.ObjectId);
            writer.WriteInt32(item.Id);
            writer.WriteInt32(item.EnchantLevel);
            writer.WriteInt32((int)item.Count);
        }

        writer.WriteByte(0); // Is bonus? Do not see any effect.
        writer.WriteInt32(_bonusItems.Count); // Array of bonus items start.
        foreach (UniqueItemEnchantHolder bonus in _bonusItems)
        {
            writer.WriteInt32(bonus.ObjectId);
            writer.WriteInt32(bonus.Id);
            writer.WriteInt32(bonus.EnchantLevel);
            writer.WriteInt32((int)bonus.Count);
        }
    }
}