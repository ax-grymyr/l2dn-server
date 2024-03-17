using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestBbsWritePacket: IIncomingPacket<GameSession>
{
    private string _url;
    private string _arg1;
    private string _arg2;
    private string _arg3;
    private string _arg4;
    private string _arg5;

    public void ReadContent(PacketBitReader reader)
    {
        _url = reader.ReadString();
        _arg1 = reader.ReadString();
        _arg2 = reader.ReadString();
        _arg3 = reader.ReadString();
        _arg4 = reader.ReadString();
        _arg5 = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        CommunityBoardHandler.getInstance().handleWriteCommand(player, _url, _arg1, _arg2, _arg3, _arg4, _arg5);
        return ValueTask.CompletedTask;
    }
}