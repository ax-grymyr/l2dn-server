using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPcCafePointInfoPacket: IOutgoingPacket
{
    private readonly int _points;
    private readonly int _mAddPoint;
    private readonly int _mPeriodType;
    private readonly int _remainTime;
    private readonly int _pointType;
    private readonly int _time;
	
    public ExPcCafePointInfoPacket()
    {
        _points = 0;
        _mAddPoint = 0;
        _remainTime = 0;
        _mPeriodType = 0;
        _pointType = 0;
        _time = 0;
    }
	
    public ExPcCafePointInfoPacket(int points, int pointsToAdd, int time)
    {
        _points = points;
        _mAddPoint = pointsToAdd;
        _mPeriodType = 1;
        _remainTime = 0; // No idea why but retail sends 42..
        _pointType = pointsToAdd < 0 ? 2 : 1; // When using points is 3
        _time = time;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PCCAFE_POINT_INFO);
        writer.WriteInt32(_points); // num points
        writer.WriteInt32(_mAddPoint); // points inc display
        writer.WriteByte((byte)_mPeriodType); // period(0=don't show window,1=acquisition,2=use points)
        writer.WriteInt32(_remainTime); // period hours left
        writer.WriteByte((byte)_pointType); // points inc display color(0=yellow, 1=cyan-blue, 2=red, all other black)
        writer.WriteInt32(_time * 3); // value is in seconds * 3
    }
}