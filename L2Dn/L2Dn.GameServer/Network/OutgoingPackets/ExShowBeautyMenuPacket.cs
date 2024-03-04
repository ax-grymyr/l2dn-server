using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowBeautyMenuPacket: IOutgoingPacket
{
    // TODO: Enum
    public const int MODIFY_APPEARANCE = 0;
    public const int RESTORE_APPEARANCE = 1;
	
    private readonly Player _player;
    private readonly int _type;
	
    public ExShowBeautyMenuPacket(Player player, int type)
    {
        _player = player;
        _type = type;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_BEAUTY_MENU);
        
        writer.WriteInt32(_type);
        writer.WriteInt32(_player.getVisualHair());
        writer.WriteInt32(_player.getVisualHairColor());
        writer.WriteInt32(_player.getVisualFace());
    }
}