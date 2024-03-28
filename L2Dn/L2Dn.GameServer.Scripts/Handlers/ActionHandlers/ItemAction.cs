using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.ActionHandlers;

public class ItemAction: IActionHandler
{
	public bool action(Player player, WorldObject target, bool interact)
	{
		Castle castle = CastleManager.getInstance().getCastle(target);
		if ((castle != null) &&
		    (SiegeGuardManager.getInstance().getSiegeGuardByItem(castle.getResidenceId(), target.getId()) != null) &&
		    ((player.getClan() == null) || (castle.getOwnerId() != player.getClanId()) ||
		     !player.hasClanPrivilege(ClanPrivilege.CS_MERCENARIES)))
		{
			player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_CHANGE_MERCENARY_POSITIONS);
			player.setTarget(target);
			player.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
			return false;
		}

		if (!player.isFlying())
		{
			player.getAI().setIntention(CtrlIntention.AI_INTENTION_PICK_UP, target);
		}
		return true;
	}
	
	public InstanceType getInstanceType()
	{
		return InstanceType.Item;
	}
}