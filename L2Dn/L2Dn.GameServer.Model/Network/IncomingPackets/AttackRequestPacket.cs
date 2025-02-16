using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct AttackRequestPacket: IIncomingPacket<GameSession>
{
    private int _objectId;
    private int _originX;
    private int _originY;
    private int _originZ;
    private int _attackId;
    
    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
        _originX = reader.ReadInt32();
        _originY = reader.ReadInt32();
        _originZ = reader.ReadInt32();
        _attackId = reader.ReadByte(); // 0 for simple click 1 for shift-click
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    // TODO flood protection
	    // if (!client.getFloodProtectors().canPerformPlayerAction())
	    // {
	    // 	return;
	    // }

	    Player? player = session.Player;
	    if (player == null)
		    return ValueTask.CompletedTask;

	    // Avoid Attacks in Boat.
	    if (player.isPlayable() && player.isInBoat())
	    {
		    connection.Send(new SystemMessagePacket(SystemMessageId.UNAVAILABLE_WHILE_SWIMMING));
		    connection.Send(ActionFailedPacket.STATIC_PACKET);
		    return ValueTask.CompletedTask;
	    }

	    BuffInfo info = player.getEffectList().getFirstBuffInfoByAbnormalType(AbnormalType.BOT_PENALTY);
	    if (info != null)
	    {
		    foreach (AbstractEffect effect in info.getEffects())
		    {
			    if (!effect.checkCondition(-1))
			    {
				    connection.Send(new SystemMessagePacket(SystemMessageId
					    .YOU_HAVE_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_SO_YOUR_ACTIONS_HAVE_BEEN_RESTRICTED));
				    
				    connection.Send(ActionFailedPacket.STATIC_PACKET);
				    return ValueTask.CompletedTask;
			    }
		    }
	    }

	    // avoid using expensive operations if not needed
	    WorldObject target;
	    if (player.getTargetId() == _objectId)
	    {
		    target = player.getTarget();
	    }
	    else
	    {
		    target = World.getInstance().findObject(_objectId);
	    }

	    if (target == null)
	    {
		    connection.Send(ActionFailedPacket.STATIC_PACKET);
		    return ValueTask.CompletedTask;
	    }

	    if ((!target.isTargetable() || player.isTargetingDisabled()) &&
	        !player.canOverrideCond(PlayerCondOverride.TARGET_ALL))
	    {
		    connection.Send(ActionFailedPacket.STATIC_PACKET);
		    return ValueTask.CompletedTask;
	    }

	    // Players can't attack objects in the other instances
	    if (target.getInstanceWorld() != player.getInstanceWorld())
	    {
		    connection.Send(ActionFailedPacket.STATIC_PACKET);
		    return ValueTask.CompletedTask;
	    }

	    // Only GMs can directly attack invisible characters
	    if (!target.isVisibleFor(player))
	    {
		    connection.Send(ActionFailedPacket.STATIC_PACKET);
		    return ValueTask.CompletedTask;
	    }

	    player.onActionRequest();

	    if (player.getTarget() != target)
	    {
		    target.onAction(player);
	    }
	    else if ((target.ObjectId != player.ObjectId) &&
	             (player.getPrivateStoreType() == PrivateStoreType.NONE) && (player.getActiveRequester() == null))
	    {
		    target.onForcedAttack(player);
	    }
	    else
	    {
		    connection.Send(ActionFailedPacket.STATIC_PACKET);
	    }

	    return ValueTask.CompletedTask;
    }
}