using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExVoteSystemInfoPacket: IOutgoingPacket
{
    private readonly int _recomLeft;
    private readonly int _recomHave;
    private readonly int _bonusTime;
    private readonly int _bonusVal;
    private readonly int _bonusType;
	
    public ExVoteSystemInfoPacket(Player player)
    {
        _recomLeft = player.getRecomLeft();
        _recomHave = player.getRecomHave();
        _bonusTime = 0;
        _bonusVal = 0;
        _bonusType = 0;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_VOTE_SYSTEM_INFO);
        writer.WriteInt32(_recomLeft);
        writer.WriteInt32(_recomHave);
        writer.WriteInt32(_bonusTime);
        writer.WriteInt32(_bonusVal);
        writer.WriteInt32(_bonusType);
    }
}