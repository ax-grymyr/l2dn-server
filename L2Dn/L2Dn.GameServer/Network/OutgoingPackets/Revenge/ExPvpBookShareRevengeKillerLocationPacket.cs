using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Revenge;

/**
 * @author Mobius
 */
public readonly struct ExPvpBookShareRevengeKillerLocationPacket: IOutgoingPacket
{
    private readonly Player _player;
	
    public ExPvpBookShareRevengeKillerLocationPacket(Player player)
    {
        _player = player;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PVPBOOK_SHARE_REVENGE_KILLER_LOCATION);
        writer.WriteSizedString(_player.getName());
        writer.WriteInt32(_player.getX());
        writer.WriteInt32(_player.getY());
        writer.WriteInt32(_player.getZ());
    }
}