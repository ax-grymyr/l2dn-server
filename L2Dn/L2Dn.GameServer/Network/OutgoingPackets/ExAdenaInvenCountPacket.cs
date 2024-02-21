using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExAdenaInvenCountPacket: IOutgoingPacket
{
    private readonly Player _player;
	
    public ExAdenaInvenCountPacket(Player player)
    {
        _player = player;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ADENA_INVEN_COUNT);
        
        writer.WriteInt64(_player.getAdena());
        writer.WriteInt16((short)_player.getInventory().getSize());
    }
}