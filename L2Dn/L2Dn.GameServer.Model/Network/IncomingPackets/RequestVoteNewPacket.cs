using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestVoteNewPacket: IIncomingPacket<GameSession>
{
    private int _targetId;

    public void ReadContent(PacketBitReader reader)
    {
        _targetId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        SystemMessagePacket sm;
        WorldObject obj = player.getTarget();
        if (!(obj is Player))
        {
            if (obj == null)
            {
                player.sendPacket(SystemMessageId.SELECT_TARGET);
            }
            else if (obj.isFakePlayer() && FakePlayerData.getInstance().isTalkable(obj.getName()))
            {
                if (player.getRecomLeft() <= 0)
                {
                    player.sendPacket(SystemMessageId.YOU_ARE_OUT_OF_RECOMMENDATIONS_TRY_AGAIN_LATER);
                    return ValueTask.CompletedTask;
                }
				
                sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_RECOMMENDED_C1_YOU_HAVE_S2_RECOMMENDATIONS_LEFT);
                sm.Params.addString(FakePlayerData.getInstance().getProperName(obj.getName()));
                sm.Params.addInt(player.getRecomLeft());
                player.sendPacket(sm);
				
                player.setRecomLeft(player.getRecomLeft() - 1);
                player.updateUserInfo();
                player.sendPacket(new ExVoteSystemInfoPacket(player));
            }
            else
            {
                player.sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
            }

            return ValueTask.CompletedTask;
        }
		
        Player target = (Player)obj;
        if (target.getObjectId() != _targetId)
            return ValueTask.CompletedTask;
		
        if (target == player)
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_RECOMMEND_YOURSELF);
            return ValueTask.CompletedTask;
        }
		
        if (player.getRecomLeft() <= 0)
        {
            player.sendPacket(SystemMessageId.YOU_ARE_OUT_OF_RECOMMENDATIONS_TRY_AGAIN_LATER);
            return ValueTask.CompletedTask;
        }
		
        if (target.getRecomHave() >= 255)
        {
            player.sendPacket(SystemMessageId.YOUR_SELECTED_TARGET_CAN_NO_LONGER_RECEIVE_A_RECOMMENDATION);
            return ValueTask.CompletedTask;
        }
		
        player.giveRecom(target);
		
        sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_RECOMMENDED_C1_YOU_HAVE_S2_RECOMMENDATIONS_LEFT);
        sm.Params.addPcName(target);
        sm.Params.addInt(player.getRecomLeft());
        player.sendPacket(sm);
		
        sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_BEEN_RECOMMENDED_BY_C1);
        sm.Params.addPcName(player);
        target.sendPacket(sm);
		
        player.updateUserInfo();
        target.broadcastUserInfo();
		
        player.sendPacket(new ExVoteSystemInfoPacket(player));
        target.sendPacket(new ExVoteSystemInfoPacket(target));
        
        return ValueTask.CompletedTask;
    }
}