using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.RandomCraft;

public readonly struct ExCraftInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
	
    public ExCraftInfoPacket(Player player)
    {
        _player = player;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CRAFT_INFO);
        
        PlayerRandomCraft rc = _player.getRandomCraft();
        writer.WriteInt32(rc.getFullCraftPoints()); // Full points owned
        writer.WriteInt32(rc.getCraftPoints()); // Craft Points (10k = 1%)
        writer.WriteByte(rc.isSayhaRoll()); // Will get sayha?
    }
}