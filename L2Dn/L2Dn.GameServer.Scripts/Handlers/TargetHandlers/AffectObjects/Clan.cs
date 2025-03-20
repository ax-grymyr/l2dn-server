using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectObjects;

/**
 * @author Nik
 */
public class Clan: IAffectObjectHandler
{
	public bool checkAffectedObject(Creature creature, Creature target)
	{
		if (creature == target)
		{
			return true;
		}

		Player? player = creature.getActingPlayer();
		if (player != null)
		{
			Model.Clans.Clan? clan = player.getClan();
			if (clan != null)
			{
				return clan == target.getClan();
			}
		}
		else if (creature.isNpc() && target.isNpc())
		{
			return ((Npc) creature).isInMyClan(((Npc) target));
		}

		return false;
	}

	public AffectObject getAffectObjectType()
	{
		return AffectObject.CLAN;
	}
}