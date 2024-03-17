using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestAllyInfoPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
	        return ValueTask.CompletedTask;
		
        SystemMessagePacket sm;
        int? allianceId = player.getAllyId();
        if (allianceId > 0)
        {
            AllianceInfoPacket ai = new AllianceInfoPacket(allianceId.Value);
            player.sendPacket(ai);
			
            // send for player
            sm = new SystemMessagePacket(SystemMessageId.ALLIANCE_INFORMATION);
            player.sendPacket(sm);
			
            sm = new SystemMessagePacket(SystemMessageId.ALLIANCE_NAME_S1);
            sm.Params.addString(ai.getName());
            player.sendPacket(sm);
			
            sm = new SystemMessagePacket(SystemMessageId.ALLIANCE_LEADER_S2_OF_S1);
            sm.Params.addString(ai.getLeaderC());
            sm.Params.addString(ai.getLeaderP());
            player.sendPacket(sm);
			
            sm = new SystemMessagePacket(SystemMessageId.CONNECTION_S1_TOTAL_S2);
            sm.Params.addInt(ai.getOnline());
            sm.Params.addInt(ai.getTotal());
            player.sendPacket(sm);
			
            sm = new SystemMessagePacket(SystemMessageId.AFFILIATED_CLANS_TOTAL_S1_CLAN_S);
            sm.Params.addInt(ai.getAllies().Length);
            player.sendPacket(sm);
			
            sm = new SystemMessagePacket(SystemMessageId.CLAN_INFORMATION);
            foreach (ClanInfo aci in ai.getAllies())
            {
                player.sendPacket(sm);
				
                sm = new SystemMessagePacket(SystemMessageId.CLAN_NAME_S1);
                sm.Params.addString(aci.getClan().getName());
                player.sendPacket(sm);
				
                sm = new SystemMessagePacket(SystemMessageId.CLAN_LEADER_S1);
                sm.Params.addString(aci.getClan().getLeaderName());
                player.sendPacket(sm);
				
                sm = new SystemMessagePacket(SystemMessageId.CLAN_LEVEL_S1);
                sm.Params.addInt(aci.getClan().getLevel());
                player.sendPacket(sm);
				
                sm = new SystemMessagePacket(SystemMessageId.CONNECTION_S1_TOTAL_S2);
                sm.Params.addInt(aci.getOnline());
                sm.Params.addInt(aci.getTotal());
                player.sendPacket(sm);
				
                sm = new SystemMessagePacket(SystemMessageId.EMPTY_4);
            }
			
            sm = new SystemMessagePacket(SystemMessageId.EMPTY_5);
            player.sendPacket(sm);
        }
        else
        {
            player.sendPacket(SystemMessageId.YOU_ARE_NOT_IN_AN_ALLIANCE);
        }
        
        return ValueTask.CompletedTask;
    }
}