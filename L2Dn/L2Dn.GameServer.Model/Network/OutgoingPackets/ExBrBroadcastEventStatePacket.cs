using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExBrBroadcastEventStatePacket: IOutgoingPacket
{
    public const int APRIL_FOOLS = 20090401;
    public const int EVAS_INFERNO = 20090801; // event state (0 - hide, 1 - show), day (1-14), percent (0-100)
    public const int HALLOWEEN_EVENT = 20091031; // event state (0 - hide, 1 - show)
    public const int RAISING_RUDOLPH = 20091225; // event state (0 - hide, 1 - show)
    public const int LOVERS_JUBILEE = 20100214; // event state (0 - hide, 1 - show)
	
    private readonly int _eventId;
    private readonly int _eventState;
    private readonly int _param0;
    private readonly int _param1;
    private readonly int _param2;
    private readonly int _param3;
    private readonly int _param4;
    private readonly String _param5;
    private readonly String _param6;
	
    public ExBrBroadcastEventStatePacket(int eventId, int eventState)
    {
        _eventId = eventId;
        _eventState = eventState;
    }
	
    public ExBrBroadcastEventStatePacket(int eventId, int eventState, int param0, int param1, int param2, int param3, int param4, String param5, String param6)
    {
        _eventId = eventId;
        _eventState = eventState;
        _param0 = param0;
        _param1 = param1;
        _param2 = param2;
        _param3 = param3;
        _param4 = param4;
        _param5 = param5;
        _param6 = param6;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BR_BROADCAST_EVENT_STATE);
        
        writer.WriteInt32(_eventId);
        writer.WriteInt32(_eventState);
        writer.WriteInt32(_param0);
        writer.WriteInt32(_param1);
        writer.WriteInt32(_param2);
        writer.WriteInt32(_param3);
        writer.WriteInt32(_param4);
        writer.WriteString(_param5);
        writer.WriteString(_param6);
    }
}