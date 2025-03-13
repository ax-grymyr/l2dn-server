using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct ActionPacket: IIncomingPacket<GameSession>
{
    private int _objectId;
    private int _originX;
    private int _originY;
    private int _originZ;
    private byte _actionId;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32(); // Target object Identifier
        _originX = reader.ReadInt32();
        _originY = reader.ReadInt32();
        _originZ = reader.ReadInt32();
        _actionId = reader.ReadByte(); // Action identifier : 0-Simple click, 1-Shift click
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		// if (!client.getFloodProtectors().canPerformPlayerAction())
		// {
		// 	return;
		// }

		// Get the current Player of the player
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		if (player.inObserverMode())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_USE_THIS_FUNCTION_IN_THE_SPECTATOR_MODE);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		BuffInfo? info = player.getEffectList().getFirstBuffInfoByAbnormalType(AbnormalType.BOT_PENALTY);
		if (info != null)
		{
			foreach (AbstractEffect effect in info.getEffects())
			{
				if (!effect.checkCondition(-4))
				{
					player.sendPacket(SystemMessageId.YOU_HAVE_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_SO_YOUR_ACTIONS_HAVE_BEEN_RESTRICTED);
					player.sendPacket(ActionFailedPacket.STATIC_PACKET);
					return ValueTask.CompletedTask;
				}
			}
		}

		WorldObject? obj;
        AirShip? airShip = player.getAirShip();
		if (player.getTargetId() == _objectId)
		{
			obj = player.getTarget();
		}
		else if (player.isInAirShip() && airShip != null && airShip.getHelmObjectId() == _objectId)
		{
			obj = player.getAirShip();
		}
		else
		{
			obj = World.getInstance().findObject(_objectId);
		}

		// If object requested does not exist, add warn msg into logs
		if (obj == null)
		{
			// pressing e.g. pickup many times quickly would get you here
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		if ((!obj.isTargetable() || player.isTargetingDisabled()) && !player.canOverrideCond(PlayerCondOverride.TARGET_ALL))
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		// Players can't interact with objects in the other instances
		if (obj.getInstanceWorld() != player.getInstanceWorld())
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		// Only GMs can directly interact with invisible characters
		if (!obj.isVisibleFor(player))
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		// Check if the target is valid, if the player haven't a shop or isn't the requester of a transaction (ex : FriendInvite, JoinAlly, JoinParty...)
		if (player.getActiveRequester() != null)
		{
			// Actions prohibited when in trade
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		player.onActionRequest();

		switch (_actionId)
		{
			case 0:
			{
				obj.onAction(player);
				break;
			}
			case 1:
			{
				if (!player.isGM() && (!(obj.isNpc() && Config.ALT_GAME_VIEWNPC) || obj.isFakePlayer()))
				{
					obj.onAction(player, false);
				}
				else
				{
					obj.onActionShift(player);
				}
				break;
			}
			default:
			{
				// Invalid action detected (probably client cheating), log this
				PacketLogger.Instance.Warn(GetType().Name + ": Character: " + player.getName() +
				                           " requested invalid action: " + _actionId);

				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				break;
			}
		}

		return ValueTask.CompletedTask;
	}
}