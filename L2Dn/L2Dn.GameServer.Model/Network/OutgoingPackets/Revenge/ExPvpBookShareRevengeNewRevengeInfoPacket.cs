using L2Dn.GameServer.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Revenge;

/**
 * @author Mobius
 */
public readonly struct ExPvpBookShareRevengeNewRevengeInfoPacket: IOutgoingPacket
{
    private readonly string _victimName;
    private readonly string _killerName;
    private readonly RevengeType _type;

    public ExPvpBookShareRevengeNewRevengeInfoPacket(string victimName, string killerName, RevengeType type)
    {
        _victimName = victimName;
        _killerName = killerName;
        _type = type;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PVPBOOK_SHARE_REVENGE_NEW_REVENGEINFO);
        writer.WriteInt32((int)_type);
        writer.WriteSizedString(_victimName);
        writer.WriteSizedString(_killerName);
    }
}