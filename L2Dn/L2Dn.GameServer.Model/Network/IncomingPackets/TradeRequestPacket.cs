using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct TradeRequestPacket: IIncomingPacket<GameSession>
{
    private int _objectId;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;
		
		if (!player.getAccessLevel().allowTransaction())
		{
			player.sendMessage("Transactions are disabled for your current Access Level.");
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		BuffInfo info = player.getEffectList().getFirstBuffInfoByAbnormalType(AbnormalType.BOT_PENALTY);
		if (info != null)
		{
			foreach (AbstractEffect effect in info.getEffects())
			{
				if (!effect.checkCondition(BotReportTable.TRADE_ACTION_BLOCK_ID))
				{
					player.sendPacket(SystemMessageId.YOU_HAVE_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_SO_YOUR_ACTIONS_HAVE_BEEN_RESTRICTED);
					player.sendPacket(ActionFailedPacket.STATIC_PACKET);
					return ValueTask.CompletedTask;
				}
			}
		}
		
		WorldObject target = World.getInstance().findObject(_objectId);

		// If there is no target, target is far away or
		// they are in different instances
		// trade request is ignored and there is no system message.
		if ((target == null) || !player.isInSurroundingRegion(target) || (target.getInstanceWorld() != player.getInstanceWorld()))
			return ValueTask.CompletedTask;
		
		// If target and acting player are the same, trade request is ignored
		// and the following system message is sent to acting player.
		if (target.getObjectId() == player.getObjectId())
		{
			player.sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
			return ValueTask.CompletedTask;
		}
		
		SystemMessagePacket sm;
		if (FakePlayerData.getInstance().isTalkable(target.getName()))
		{
			string name = FakePlayerData.getInstance().getProperName(target.getName());
			bool npcInRange = false;
			foreach (Npc npc in World.getInstance().getVisibleObjectsInRange<Npc>(player, 150))
			{
				if (string.Equals(npc.getName(), name))
					npcInRange = true;
			}
			
			if (!npcInRange)
			{
				player.sendPacket(SystemMessageId.YOUR_TARGET_IS_OUT_OF_RANGE);
				return ValueTask.CompletedTask;
			}
			
			if (!player.isProcessingRequest())
			{
				sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_REQUESTED_A_TRADE_WITH_C1);
				sm.Params.addString(name);
				player.sendPacket(sm);
				ThreadPool.schedule(() => scheduleDeny(player, name), 10000);
				player.blockRequest();
			}
			else
			{
				player.sendPacket(SystemMessageId.YOU_ARE_ALREADY_TRADING_WITH_SOMEONE);
			}

			return ValueTask.CompletedTask;
		}
		
		if (!target.isPlayer())
		{
			player.sendPacket(SystemMessageId.INVALID_TARGET);
			return ValueTask.CompletedTask;
		}
		
		Player partner = target.getActingPlayer();
		if (partner.isInOlympiadMode() || player.isInOlympiadMode())
		{
			player.sendMessage("A user currently participating in the Olympiad cannot accept or request a trade.");
			return ValueTask.CompletedTask;
		}
		
		info = partner.getEffectList().getFirstBuffInfoByAbnormalType(AbnormalType.BOT_PENALTY);
		if (info != null)
		{
			foreach (AbstractEffect effect in info.getEffects())
			{
				if (!effect.checkCondition(BotReportTable.TRADE_ACTION_BLOCK_ID))
				{
					sm = new SystemMessagePacket(SystemMessageId.C1_HAS_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_AND_IS_CURRENTLY_BEING_INVESTIGATED);
					sm.Params.addString(partner.getName());
					player.sendPacket(sm);
					player.sendPacket(ActionFailedPacket.STATIC_PACKET);
					return ValueTask.CompletedTask;
				}
			}
		}
		
		// L2J Customs: Karma punishment
		if (!Config.ALT_GAME_KARMA_PLAYER_CAN_TRADE && (player.getReputation() < 0))
		{
			player.sendMessage("You cannot trade while you are in a chaotic state.");
			return ValueTask.CompletedTask;
		}
		
		if (!Config.ALT_GAME_KARMA_PLAYER_CAN_TRADE && (partner.getReputation() < 0))
		{
			player.sendMessage("You cannot request a trade while your target is in a chaotic state.");
			return ValueTask.CompletedTask;
		}
		
		if (Config.JAIL_DISABLE_TRANSACTION && (player.isJailed() || partner.isJailed()))
		{
			player.sendMessage("You cannot trade while you are in in Jail.");
			return ValueTask.CompletedTask;
		}
		
		if ((player.getPrivateStoreType() != PrivateStoreType.NONE) || (partner.getPrivateStoreType() != PrivateStoreType.NONE))
		{
			player.sendPacket(SystemMessageId.WHILE_OPERATING_A_PRIVATE_STORE_OR_WORKSHOP_YOU_CANNOT_DISCARD_DESTROY_OR_TRADE_AN_ITEM);
			return ValueTask.CompletedTask;
		}
		
		if (player.isProcessingTransaction())
		{
			player.sendPacket(SystemMessageId.YOU_ARE_ALREADY_TRADING_WITH_SOMEONE);
			return ValueTask.CompletedTask;
		}
		
		if (partner.isProcessingRequest() || partner.isProcessingTransaction())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_ON_ANOTHER_TASK_PLEASE_TRY_AGAIN_LATER);
			sm.Params.addString(partner.getName());
			player.sendPacket(sm);
			return ValueTask.CompletedTask;
		}
		
		if (partner.getTradeRefusal())
		{
			player.sendMessage("That person is in trade refusal mode.");
			return ValueTask.CompletedTask;
		}
		
		if (BlockList.isBlocked(partner, player))
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_HAS_ADDED_YOU_TO_THEIR_IGNORE_LIST);
			sm.Params.addString(partner.getName());
			player.sendPacket(sm);
			return ValueTask.CompletedTask;
		}
		
		if (player.calculateDistance3D(partner.getLocation().Location3D) > 150)
		{
			player.sendPacket(SystemMessageId.YOUR_TARGET_IS_OUT_OF_RANGE);
			return ValueTask.CompletedTask;
		}
		
		player.onTransactionRequest(partner);
		partner.sendPacket(new SendTradeRequestPacket(player.getObjectId()));
		sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_REQUESTED_A_TRADE_WITH_C1);
		sm.Params.addString(partner.getName());
		player.sendPacket(sm);
		return ValueTask.CompletedTask;
    }

    private static void scheduleDeny(Player player, string name)
    {
	    if (player != null)
	    {
		    SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_HAS_DENIED_YOUR_REQUEST_TO_TRADE);
		    sm.Params.addString(name);
		    player.sendPacket(sm);
		    player.onTransactionResponse();
	    }
    }
}