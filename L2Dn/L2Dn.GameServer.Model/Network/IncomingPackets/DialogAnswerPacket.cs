using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct DialogAnswerPacket: IIncomingPacket<GameSession>
{
    private SystemMessageId _messageId;
    private int _answer;
    private int _requesterId;

    public void ReadContent(PacketBitReader reader)
    {
        _messageId = (SystemMessageId)reader.ReadInt32();
        _answer = reader.ReadInt32();
        _requesterId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    Player? player = session.Player;
	    if (player == null)
		    return ValueTask.CompletedTask;

	    if (player.Events.HasSubscribers<OnPlayerDlgAnswer>())
	    {
		    OnPlayerDlgAnswer onPlayerDlgAnswer = new OnPlayerDlgAnswer(player, _messageId, _answer, _requesterId);
		    if (player.Events.Notify(onPlayerDlgAnswer) && onPlayerDlgAnswer.Terminate)
			    return ValueTask.CompletedTask;
	    }

	    if (_messageId == SystemMessageId.S1_3)
	    {
		    // Custom .offlineplay voiced command dialog.
		    if (player.removeAction(PlayerAction.OFFLINE_PLAY))
		    {
			    if (_answer == 0 || !Config.ENABLE_OFFLINE_PLAY_COMMAND)
				    return ValueTask.CompletedTask;

			    if (Config.OFFLINE_PLAY_PREMIUM && !player.hasPremiumStatus())
			    {
				    player.sendMessage("This command is only available to premium players.");
				    return ValueTask.CompletedTask;
			    }

			    if (!player.isAutoPlaying())
			    {
				    player.sendMessage("You need to enable auto play before exiting.");
				    return ValueTask.CompletedTask;
			    }

			    if (player.isInVehicle() || player.isInsideZone(ZoneId.PEACE))
			    {
				    player.sendPacket(SystemMessageId.YOU_MAY_NOT_LOG_OUT_FROM_THIS_LOCATION);
				    return ValueTask.CompletedTask;
			    }

			    if (player.isRegisteredOnEvent())
			    {
				    player.sendMessage("Cannot use this command while registered on an event.");
				    return ValueTask.CompletedTask;
			    }

			    // Unregister from olympiad.
			    if (OlympiadManager.getInstance().isRegistered(player))
			    {
				    OlympiadManager.getInstance().unRegisterNoble(player);
			    }

			    player.startOfflinePlay();
			    return ValueTask.CompletedTask;
		    }

		    if (player.removeAction(PlayerAction.ADMIN_COMMAND))
		    {
			    string? cmd = player.getAdminConfirmCmd();
			    player.setAdminConfirmCmd(null);
			    if (_answer == 0)
				    return ValueTask.CompletedTask;

			    // The 'useConfirm' must be disabled here, as we don't want to repeat that process.
			    AdminCommandHandler.getInstance().useAdminCommand(player, cmd, false);
		    }
	    }
	    else if (_messageId == SystemMessageId.DO_YOU_WISH_TO_EXIT_THE_GAME)
	    {
		    if (_answer == 0 || !Config.ENABLE_OFFLINE_COMMAND ||
		        (!Config.OFFLINE_TRADE_ENABLE && !Config.OFFLINE_CRAFT_ENABLE))
			    return ValueTask.CompletedTask;

		    if (!player.isInStoreMode())
		    {
			    player.sendPacket(SystemMessageId.PRIVATE_STORE_ALREADY_CLOSED);
			    return ValueTask.CompletedTask;
		    }

		    if (player.isInInstance() || player.isInVehicle() || !player.canLogout())
			    return ValueTask.CompletedTask;

		    // Unregister from olympiad.
		    if (OlympiadManager.getInstance().isRegistered(player))
		    {
			    OlympiadManager.getInstance().unRegisterNoble(player);
		    }

		    if (!OfflineTradeUtil.enteredOfflineMode(player))
		    {
			    Disconnection.of(session, player).defaultSequence(LeaveWorldPacket.STATIC_PACKET);
		    }
	    }
	    else if (_messageId == SystemMessageId.C1_IS_ATTEMPTING_TO_DO_A_RESURRECTION_THAT_RESTORES_S2_S3_XP_ACCEPT ||
	             _messageId == SystemMessageId.YOUR_CHARM_OF_COURAGE_IS_TRYING_TO_RESURRECT_YOU_WOULD_YOU_LIKE_TO_RESURRECT_NOW)
	    {
		    player.reviveAnswer(_answer);
	    }
	    else if (_messageId == SystemMessageId.C1_WANTS_TO_SUMMON_YOU_TO_S2_ACCEPT)
	    {
		    SummonRequestHolder? holder = player.removeScript<SummonRequestHolder>();
		    if (_answer == 1 && holder != null && holder.getSummoner().ObjectId == _requesterId)
		    {
			    player.teleToLocation(holder.Location, true);
		    }
	    }
	    else if (_messageId == SystemMessageId.WOULD_YOU_LIKE_TO_OPEN_THE_GATE)
	    {
		    DoorRequestHolder? holder = player.removeScript<DoorRequestHolder>();
		    if (holder != null && holder.getDoor() == player.getTarget() && _answer == 1)
		    {
			    holder.getDoor().openMe();
		    }
	    }
	    else if (_messageId == SystemMessageId.WOULD_YOU_LIKE_TO_CLOSE_THE_GATE)
	    {
		    DoorRequestHolder? holder = player.removeScript<DoorRequestHolder>();
		    if (holder != null && holder.getDoor() == player.getTarget() && _answer == 1)
		    {
			    holder.getDoor().closeMe();
		    }
	    }

	    return ValueTask.CompletedTask;
    }
}