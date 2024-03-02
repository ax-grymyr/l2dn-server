using L2Dn.Extensions;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct OrcFortressSiegeInfoHUDPacket: IOutgoingPacket
{
    private readonly int _fortressId;
    private readonly int _siegeState;
    private readonly DateTime _nowTime;
    private readonly TimeSpan _remainTime;
	
    public OrcFortressSiegeInfoHUDPacket(int fortressId, int siegeState, DateTime nowTime, TimeSpan remainTime)
    {
        _fortressId = fortressId;
        _siegeState = siegeState;
        _nowTime = nowTime;
        _remainTime = remainTime;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ADEN_FORTRESS_SIEGE_HUD_INFO);
        
        writer.WriteInt32(_fortressId);
        writer.WriteInt32(_siegeState);
        writer.WriteInt32(_nowTime.getEpochSecond());
        writer.WriteInt32((int)_remainTime.TotalSeconds);
    }
}