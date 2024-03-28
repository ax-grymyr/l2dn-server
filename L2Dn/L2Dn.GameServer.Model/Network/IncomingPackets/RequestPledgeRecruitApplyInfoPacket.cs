using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgeRecruitApplyInfoPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        ClanEntryStatus status;
        if ((player.getClan() != null) && player.isClanLeader() && ClanEntryManager.getInstance().isClanRegistred(player.getClanId().Value))
        {
            status = ClanEntryStatus.ORDERED;
        }
        else if ((player.getClan() == null) && (ClanEntryManager.getInstance().isPlayerRegistred(player.getObjectId())))
        {
            status = ClanEntryStatus.WAITING;
        }
        else
        {
            status = ClanEntryStatus.DEFAULT;
        }
		
        player.sendPacket(new ExPledgeRecruitApplyInfoPacket(status));
        
        return ValueTask.CompletedTask;
    }
}