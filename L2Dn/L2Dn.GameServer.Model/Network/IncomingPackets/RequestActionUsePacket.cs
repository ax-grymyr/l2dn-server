using System.Collections.Immutable;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Transforms;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestActionUsePacket: IIncomingPacket<GameSession>
{
    private int _actionId;
    private bool _ctrlPressed;
    private bool _shiftPressed;

    public void ReadContent(PacketBitReader reader)
    {
        _actionId = reader.ReadInt32();
        _ctrlPressed = reader.ReadInt32() == 1;
        _shiftPressed = reader.ReadByte() == 1;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		// Don't do anything if player is dead or confused
		if ((player.isFakeDeath() && _actionId != 0) || player.isDead() || player.isControlBlocked())
		{
			connection.Send(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		BuffInfo? info = player.getEffectList().getFirstBuffInfoByAbnormalType(AbnormalType.BOT_PENALTY);
		if (info != null)
		{
			foreach (AbstractEffect effect in info.getEffects())
			{
				if (!effect.checkCondition(_actionId))
				{
					connection.Send(new SystemMessagePacket(SystemMessageId.YOU_HAVE_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_SO_YOUR_ACTIONS_HAVE_BEEN_RESTRICTED));
					connection.Send(ActionFailedPacket.STATIC_PACKET);
					return ValueTask.CompletedTask;
				}
			}
		}

		// Don't allow to do some action if player is transformed
        Transform? transform = player.getTransformation();
		if (player.isTransformed() && transform != null)
		{
			TransformTemplate? transformTemplate = transform.getTemplate(player);
			ImmutableArray<int> allowedActions = transformTemplate?.getBasicActionList() ?? default;
			if (allowedActions.IsDefaultOrEmpty || allowedActions.BinarySearch(_actionId) < 0)
			{
				connection.Send(ActionFailedPacket.STATIC_PACKET);
				PacketLogger.Instance.Warn(player + " used action which he does not have! Id = " + _actionId +
				                           " transform: " + transform.getId());

				return ValueTask.CompletedTask;
			}
		}

		ActionDataHolder? actionHolder = ActionData.getInstance().getActionData(_actionId);
		if (actionHolder != null)
		{
			IPlayerActionHandler? actionHandler = PlayerActionHandler.getInstance().getHandler(actionHolder.getHandler());
			if (actionHandler != null)
			{
				actionHandler.useAction(player, actionHolder, _ctrlPressed, _shiftPressed);
				return ValueTask.CompletedTask;
			}

			PacketLogger.Instance.Warn("Couldn't find handler with name: " + actionHolder.getHandler());
			return ValueTask.CompletedTask;
		}

		switch (_actionId)
		{
			case 51: // General Manufacture
			{
				// Player shouldn't be able to set stores if he/she is alike dead (dead or fake death)
				if (player.isAlikeDead())
				{
					connection.Send(ActionFailedPacket.STATIC_PACKET);
					return ValueTask.CompletedTask;
				}

				if (player.isSellingBuffs())
				{
					connection.Send(ActionFailedPacket.STATIC_PACKET);
					return ValueTask.CompletedTask;
				}

				if (player.getPrivateStoreType() != PrivateStoreType.NONE)
				{
					player.setPrivateStoreType(PrivateStoreType.NONE);
					player.broadcastUserInfo();
				}
				if (player.isSitting())
				{
					player.standUp();
				}

				connection.Send(new RecipeShopManageListPacket(player, false));
				break;
			}
			default:
			{
				PacketLogger.Instance.Warn(player.getName() + ": unhandled action type " + _actionId);
				break;
			}
		}

		return ValueTask.CompletedTask;
    }
}