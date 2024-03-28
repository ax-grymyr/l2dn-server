using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgeMemberPowerInfoPacket: IIncomingPacket<GameSession>
{
    private string _player;

    public void ReadContent(PacketBitReader reader)
    {
		reader.ReadInt32(); // Unknown
		_player = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
		// do we need powers to do that??
		Clan? clan = player.getClan();
		if (clan == null)
            return ValueTask.CompletedTask;
		
		ClanMember member = clan.getClanMember(_player);
		if (member == null)
            return ValueTask.CompletedTask;

		player.sendPacket(new PledgeReceivePowerInfoPacket(member));

        return ValueTask.CompletedTask;
    }
}