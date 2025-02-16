using L2Dn.GameServer.AI;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.Geometry;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

/**
 * Social Action player action handler.
 * @author Nik
 */
public class SocialAction: IPlayerActionHandler
{
	public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
	{
		switch (data.getOptionId())
		{
			case 2: // Greeting
			case 3: // Victory
			case 4: // Advance
			case 5: // No
			case 6: // Yes
			case 7: // Bow
			case 8: // Unaware
			case 9: // Social Waiting
			case 10: // Laugh
			case 11: // Applaud
			case 12: // Dance
			case 13: // Sorrow
			case 14: // Charm
			case 15: // Shyness
			case 28: // Propose
			case 29: // Provoke
			{
				useSocial(player, data.getOptionId());
				break;
			}
			case 30: // Beauty Shop
			{
				if (useSocial(player, data.getOptionId()))
				{
					player.broadcastInfo();
				}
				break;
			}
			case 16: // Exchange Bows
			case 17: // High Five
			case 18: // Couple Dance
			{
				useCoupleSocial(player, data.getOptionId());
				break;
			}
		}
	}

	public bool isPetAction()
	{
		return false;
	}

	private bool useSocial(Player player, int id)
	{
		if (player.isFishing())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_DO_THAT_WHILE_FISHING_3);
			return false;
		}
		
		if (player.canMakeSocialAction())
		{
			player.broadcastPacket(new SocialActionPacket(player.ObjectId, id));
			
			// Notify to scripts
			if (player.Events.HasSubscribers<OnPlayerSocialAction>())
			{
				player.Events.NotifyAsync(new OnPlayerSocialAction(player, id));
			}
		}
		
		return true;
	}
	
	private void scheduleDeny(Player player)
	{
		if (player != null)
		{
			player.sendPacket(SystemMessageId.THE_COUPLE_ACTION_REQUEST_HAS_BEEN_DENIED);
			player.onTransactionResponse();
		}
	}
	
	private void useCoupleSocial(Player player, int id)
	{
		if (player == null)
		{
			return;
		}
		
		WorldObject target = player.getTarget();
		if ((target == null))
		{
			player.sendPacket(SystemMessageId.INVALID_TARGET);
			return;
		}

		SystemMessagePacket sm;
		if (FakePlayerData.getInstance().isTalkable(target.getName()))
		{
			sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_REQUESTED_A_COUPLE_ACTION_WITH_C1);
			sm.Params.addString(target.getName());
			player.sendPacket(sm);
			
			if (!player.isProcessingRequest())
			{
				ThreadPool.schedule(() => scheduleDeny(player), 10000);
				player.blockRequest();
			}
			return;
		}
		
		if (!target.isPlayer())
		{
			player.sendPacket(SystemMessageId.INVALID_TARGET);
			return;
		}
		
		int distance = (int) player.Distance2D(target);
		if ((distance > 125) || (distance < 15) || (player.ObjectId == target.ObjectId))
		{
			player.sendPacket(SystemMessageId.THE_REQUEST_CANNOT_BE_COMPLETED_BECAUSE_THE_TARGET_DOES_NOT_MEET_LOCATION_REQUIREMENTS);
			return;
		}
		
		if (player.isInStoreMode() || player.isCrafting())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_IN_PRIVATE_STORE_MODE_OR_IN_A_BATTLE_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
			sm.Params.addPcName(player);
			player.sendPacket(sm);
			return;
		}
		
		if (player.isInCombat() || player.isInDuel() || AttackStanceTaskManager.getInstance().hasAttackStanceTask(player))
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_IN_A_BATTLE_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
			sm.Params.addPcName(player);
			player.sendPacket(sm);
			return;
		}
		
		if (player.isFishing())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_DO_THAT_WHILE_FISHING_3);
			return;
		}
		
		if (player.getReputation() < 0)
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_IN_A_CHAOTIC_STATE_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
			sm.Params.addPcName(player);
			player.sendPacket(sm);
			return;
		}
		
		if (player.isInOlympiadMode())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_PARTICIPATING_IN_THE_OLYMPIAD_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
			sm.Params.addPcName(player);
			player.sendPacket(sm);
			return;
		}
		
		if (player.isInSiege())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_IN_A_CASTLE_SIEGE_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
			sm.Params.addPcName(player);
			player.sendPacket(sm);
			return;
		}
		
		if (player.isInHideoutSiege())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_PARTICIPATING_IN_A_CLAN_HALL_SIEGE_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
			sm.Params.addPcName(player);
			player.sendPacket(sm);
		}
		
		if (player.isMounted() || player.isFlyingMounted() || player.isInBoat() || player.isInAirShip())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_RIDING_A_SHIP_STEED_OR_STRIDER_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
			sm.Params.addPcName(player);
			player.sendPacket(sm);
			return;
		}
		
		if (player.isTransformed())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_CURRENTLY_TRANSFORMING_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
			sm.Params.addPcName(player);
			player.sendPacket(sm);
			return;
		}
		
		if (player.isAlikeDead())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_CURRENTLY_DEAD_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
			sm.Params.addPcName(player);
			player.sendPacket(sm);
			return;
		}
		
		// Checks for partner.
		Player partner = target.getActingPlayer();
		if (partner.isInStoreMode() || partner.isCrafting())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_IN_PRIVATE_STORE_MODE_OR_IN_A_BATTLE_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
			sm.Params.addPcName(partner);
			player.sendPacket(sm);
			return;
		}
		
		if (partner.isInCombat() || partner.isInDuel() || AttackStanceTaskManager.getInstance().hasAttackStanceTask(partner))
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_IN_A_BATTLE_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
			sm.Params.addPcName(partner);
			player.sendPacket(sm);
			return;
		}
		
		if (partner.getMultiSociaAction() > 0)
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_ALREADY_PARTICIPATING_IN_A_COUPLE_ACTION_AND_CANNOT_BE_REQUESTED_FOR_ANOTHER_COUPLE_ACTION);
			sm.Params.addPcName(partner);
			player.sendPacket(sm);
			return;
		}
		
		if (partner.isFishing())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_FISHING_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
			sm.Params.addPcName(partner);
			player.sendPacket(sm);
			return;
		}
		
		if (partner.getReputation() < 0)
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_IN_A_CHAOTIC_STATE_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
			sm.Params.addPcName(partner);
			player.sendPacket(sm);
			return;
		}
		
		if (partner.isInOlympiadMode())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_PARTICIPATING_IN_THE_OLYMPIAD_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
			sm.Params.addPcName(partner);
			player.sendPacket(sm);
			return;
		}
		
		if (partner.isInHideoutSiege())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_PARTICIPATING_IN_A_CLAN_HALL_SIEGE_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
			sm.Params.addPcName(partner);
			player.sendPacket(sm);
			return;
		}
		
		if (partner.isInSiege())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_IN_A_CASTLE_SIEGE_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
			sm.Params.addPcName(partner);
			player.sendPacket(sm);
			return;
		}
		
		if (partner.isMounted() || partner.isFlyingMounted() || partner.isInBoat() || partner.isInAirShip())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_RIDING_A_SHIP_STEED_OR_STRIDER_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
			sm.Params.addPcName(partner);
			player.sendPacket(sm);
			return;
		}
		
		if (partner.isTeleporting())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_CURRENTLY_TELEPORTING_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
			sm.Params.addPcName(partner);
			player.sendPacket(sm);
			return;
		}
		
		if (partner.isTransformed())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_CURRENTLY_TRANSFORMING_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
			sm.Params.addPcName(partner);
			player.sendPacket(sm);
			return;
		}
		
		if (partner.isAlikeDead())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_CURRENTLY_DEAD_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION);
			sm.Params.addPcName(partner);
			player.sendPacket(sm);
			return;
		}
		
		if (player.isAllSkillsDisabled() || partner.isAllSkillsDisabled())
		{
			player.sendPacket(SystemMessageId.THE_COUPLE_ACTION_WAS_CANCELLED);
			return;
		}
		
		player.setMultiSocialAction(id, partner.ObjectId);
		sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_REQUESTED_A_COUPLE_ACTION_WITH_C1);
		sm.Params.addPcName(partner);
		player.sendPacket(sm);

		if ((player.getAI().getIntention() != CtrlIntention.AI_INTENTION_IDLE) ||
		    (partner.getAI().getIntention() != CtrlIntention.AI_INTENTION_IDLE))
		{
			NextAction nextAction = new NextAction(CtrlEvent.EVT_ARRIVED, CtrlIntention.AI_INTENTION_MOVE_TO,
				() => partner.sendPacket(new ExAskCoupleActionPacket(player.ObjectId, id)));
			player.getAI().setNextAction(nextAction);
			return;
		}

		if (player.isCastingNow())
		{
			NextAction nextAction = new NextAction(CtrlEvent.EVT_FINISH_CASTING, CtrlIntention.AI_INTENTION_CAST,
				() => partner.sendPacket(new ExAskCoupleActionPacket(player.ObjectId, id)));
			player.getAI().setNextAction(nextAction);
			return;
		}

		partner.sendPacket(new ExAskCoupleActionPacket(player.ObjectId, id));
	}
}