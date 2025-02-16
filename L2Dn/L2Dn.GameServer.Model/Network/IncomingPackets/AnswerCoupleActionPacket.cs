﻿using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct AnswerCoupleActionPacket: IIncomingPacket<GameSession>
{
    private int _objectId;
    private int _actionId;
    private int _answer;

    public void ReadContent(PacketBitReader reader)
    {
        _actionId = reader.ReadInt32();
        _answer = reader.ReadInt32();
        _objectId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Player target = World.getInstance().getPlayer(_objectId);
        if (target == null)
            return ValueTask.CompletedTask;
		
        if (target.getMultiSocialTarget() != player.ObjectId || target.getMultiSociaAction() != _actionId)
            return ValueTask.CompletedTask;
		
        if (_answer == 0) // cancel
        {
            target.sendPacket(SystemMessageId.THE_COUPLE_ACTION_REQUEST_HAS_BEEN_DENIED);
        }
        else if (_answer == 1) // approve
        {
            int distance = (int)player.Distance2D(target);
            if (distance > 125 || distance < 15 || player.ObjectId == target.ObjectId)
            {
                player.sendPacket(SystemMessageId.THE_REQUEST_CANNOT_BE_COMPLETED_BECAUSE_THE_TARGET_DOES_NOT_MEET_LOCATION_REQUIREMENTS);
                target.sendPacket(SystemMessageId.THE_REQUEST_CANNOT_BE_COMPLETED_BECAUSE_THE_TARGET_DOES_NOT_MEET_LOCATION_REQUIREMENTS);
                return ValueTask.CompletedTask;
            }
            
            int heading = player.HeadingTo(target);
            player.broadcastPacket(new ExRotationPacket(player.ObjectId, heading));
            player.setHeading(heading);
            heading = target.HeadingTo(player);
            target.setHeading(heading);
            target.broadcastPacket(new ExRotationPacket(target.ObjectId, heading));
            player.broadcastPacket(new SocialActionPacket(player.ObjectId, _actionId));
            target.broadcastPacket(new SocialActionPacket(_objectId, _actionId));
        }
        else if (_answer == -1) // refused
        {
            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_IS_SET_TO_REFUSE_COUPLE_ACTIONS_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
            sm.Params.addPcName(player);
            target.sendPacket(sm);
        }
        
        target.setMultiSocialAction(0, 0);
        
        return ValueTask.CompletedTask;
    }
}