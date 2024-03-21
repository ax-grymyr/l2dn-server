using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans.Entries;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgeRecruitBoardDetailPacket: IIncomingPacket<GameSession>
{
    private int _clanId;

    public void ReadContent(PacketBitReader reader)
    {
        _clanId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        PledgeRecruitInfo pledgeRecruitInfo = ClanEntryManager.getInstance().getClanById(_clanId);
        if (pledgeRecruitInfo == null)
            return ValueTask.CompletedTask;
		
        player.sendPacket(new ExPledgeRecruitBoardDetailPacket(pledgeRecruitInfo));

        return ValueTask.CompletedTask;
    }
}