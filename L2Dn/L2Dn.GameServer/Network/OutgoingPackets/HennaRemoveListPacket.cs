using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct HennaRemoveListPacket: IOutgoingPacket
{
    private readonly Player _player;
	
    public HennaRemoveListPacket(Player player)
    {
        _player = player;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.HENNA_UNEQUIP_LIST);
        
        // TODO: implement
        // writeLong(_player.getAdena());
        // writeInt(3); // seems to be max size
        // writeInt(3 - _player.getHennaEmptySlots());
        // for (Henna henna : _player.getHennaList())
        // {
        // if (henna != null)
        // {
        // writeInt(henna.getDyeId());
        // writeInt(henna.getDyeItemId());
        // writeLong(henna.getCancelCount());
        // writeLong(henna.getCancelFee());
        // writeInt(0);
        // }
        // }
    }
}