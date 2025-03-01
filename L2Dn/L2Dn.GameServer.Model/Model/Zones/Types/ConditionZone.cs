using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * @author UnAfraid
 */
public class ConditionZone(int id, ZoneForm form): ZoneType(id, form)
{
	private bool NO_ITEM_DROP;
	private bool NO_BOOKMARK;

    public override void setParameter(string name, string value)
	{
		if (name.equalsIgnoreCase("NoBookmark"))
		{
			NO_BOOKMARK = bool.Parse(value);
		}
		else if (name.equalsIgnoreCase("NoItemDrop"))
		{
			NO_ITEM_DROP = bool.Parse(value);
		}
		else
		{
			base.setParameter(name, value);
		}
	}

	protected override void onEnter(Creature creature)
	{
		if (creature.isPlayer())
		{
			if (NO_BOOKMARK)
			{
				creature.setInsideZone(ZoneId.NO_BOOKMARK, true);
			}
			if (NO_ITEM_DROP)
			{
				creature.setInsideZone(ZoneId.NO_ITEM_DROP, true);
			}
		}
	}

	protected override void onExit(Creature creature)
	{
		if (creature.isPlayer())
		{
			if (NO_BOOKMARK)
			{
				creature.setInsideZone(ZoneId.NO_BOOKMARK, false);
			}
			if (NO_ITEM_DROP)
			{
				creature.setInsideZone(ZoneId.NO_ITEM_DROP, false);
			}
		}
	}
}