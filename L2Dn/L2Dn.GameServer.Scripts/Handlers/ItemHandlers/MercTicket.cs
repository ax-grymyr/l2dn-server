using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

/**
 * Mercenary Ticket Item Handler.
 * @author St3eT
 */
public class MercTicket: /*AbstractNpcAI, */ IItemHandler
{
	private readonly Map<int, Item> _items = new();

	public bool useItem(Playable playable, Item item, bool forceUse)
	{
        Player? player = playable.getActingPlayer();
		if (!playable.isPlayer() || player == null)
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}

		Castle? castle = CastleManager.getInstance().getCastle(player);
		if (castle == null || player.getClan() == null || castle.getOwnerId() != player.getClanId() || !player.hasClanPrivilege(ClanPrivilege.CS_MERCENARIES))
		{
			player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_POSITION_MERCENARIES);
			return false;
		}

		int castleId = castle.getResidenceId();
		SiegeGuardHolder? holder = SiegeGuardManager.getInstance().getSiegeGuardByItem(castleId, item.Id);
		if (holder == null || castleId != holder.CastleId)
		{
			player.sendPacket(SystemMessageId.MERCENARIES_CANNOT_BE_POSITIONED_HERE);
			return false;
		}

        if (castle.getSiege().isInProgress())
        {
            player.sendPacket(SystemMessageId.THIS_MERCENARY_CANNOT_BE_POSITIONED_ANYMORE);
            return false;
        }

        if (SiegeGuardManager.getInstance().isTooCloseToAnotherTicket(player))
        {
            player.sendPacket(SystemMessageId.POSITIONING_CANNOT_BE_DONE_HERE_BECAUSE_THE_DISTANCE_BETWEEN_MERCENARIES_IS_TOO_SHORT);
            return false;
        }

        if (SiegeGuardManager.getInstance().isAtNpcLimit(castleId, item.Id))
        {
            player.sendPacket(SystemMessageId.THIS_MERCENARY_CANNOT_BE_POSITIONED_ANYMORE);
            return false;
        }

        _items.put(player.ObjectId, item);
		ConfirmDialogPacket dlg = new ConfirmDialogPacket(SystemMessageId.PLACE_S1_IN_THE_CURRENT_LOCATION_AND_DIRECTION_DO_YOU_WISH_TO_CONTINUE, 15000);
		dlg.Params.addNpcName(holder.NpcId);
		player.sendPacket(dlg);
		player.addAction(PlayerAction.MERCENARY_CONFIRM);
		return true;
	}

	// TODO
	// @RegisterEvent(EventType.ON_PLAYER_DLG_ANSWER)
	// @RegisterType(ListenerRegisterType.GLOBAL_PLAYERS)
	public void onPlayerDlgAnswer(OnPlayerDlgAnswer @event)
	{
		Player player = @event.getPlayer();
		if (player.removeAction(PlayerAction.MERCENARY_CONFIRM) && _items.TryGetValue(player.ObjectId, out Item? item))
		{
			if (SiegeGuardManager.getInstance().isTooCloseToAnotherTicket(player))
			{
				player.sendPacket(SystemMessageId.POSITIONING_CANNOT_BE_DONE_HERE_BECAUSE_THE_DISTANCE_BETWEEN_MERCENARIES_IS_TOO_SHORT);
				return;
			}

			if (@event.getAnswer() == 1)
			{
				SiegeGuardManager.getInstance().addTicket(item.Id, player);
				player.destroyItem("Consume", item.ObjectId, 1, null, false); // Remove item from char's inventory
			}

			_items.remove(player.ObjectId);
		}
	}
}