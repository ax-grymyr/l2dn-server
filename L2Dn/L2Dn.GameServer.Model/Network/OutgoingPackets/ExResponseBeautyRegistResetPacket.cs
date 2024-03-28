using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExResponseBeautyRegistResetPacket: IOutgoingPacket
{
    public const int FAILURE = 0;
    public const int SUCCESS = 1;
    public const int CHANGE = 0;
    public const int RESTORE = 1;
	
    private readonly Player _player;
    private readonly int _type;
    private readonly int _result;
	
    public ExResponseBeautyRegistResetPacket(Player player, int type, int result)
    {
        _player = player;
        _type = type;
        _result = result;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RESPONSE_BEAUTY_REGIST_RESET);
        
        writer.WriteInt64(_player.getAdena());
        writer.WriteInt64(_player.getBeautyTickets());
        writer.WriteInt32(_type);
        writer.WriteInt32(_result);
        writer.WriteInt32(_player.getVisualHair());
        writer.WriteInt32(_player.getVisualFace());
        writer.WriteInt32(_player.getVisualHairColor());
    }
}