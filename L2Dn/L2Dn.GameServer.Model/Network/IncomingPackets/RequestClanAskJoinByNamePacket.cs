using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestClanAskJoinByNamePacket: IIncomingPacket<GameSession>
{
    private string _playerName;
    private int _pledgeType;

    public void ReadContent(PacketBitReader reader)
    {
        _playerName = reader.ReadString();
        _pledgeType = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null || player.getClan() == null)
            return ValueTask.CompletedTask;
		
        Player invitedPlayer = World.getInstance().getPlayer(_playerName);
        if (!player.getClan().checkClanJoinCondition(player, invitedPlayer, _pledgeType))
            return ValueTask.CompletedTask;

        if (!player.getRequest().setRequest(invitedPlayer, this))
            return ValueTask.CompletedTask;
		
        invitedPlayer.sendPacket(new AskJoinPledgePacket(player, player.getClan().getName()));
        return ValueTask.CompletedTask;
    }
	
    public int getPledgeType()
    {
        return _pledgeType;
    }
}