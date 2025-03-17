using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;
using L2Dn.GameServer.StaticData.Xml.Zones;
using Config = L2Dn.GameServer.Configuration.Config;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * @author UnAfraid
 */
public class SayuneZone(int id, ZoneForm form): Zone(id, form)
{
	private int _mapId = -1;

    public override void setParameter(XmlZoneStatName name, string value)
	{
		switch (name)
		{
			case XmlZoneStatName.mapId:
			{
				_mapId = int.Parse(value);
				break;
			}
			default:
			{
				base.setParameter(name, value);
				break;
			}
		}
	}

	protected override void onEnter(Creature creature)
    {
        Player? player = creature.getActingPlayer();
		if (creature.isPlayer() && player != null &&
            /* creature.isInCategory(CategoryType.SIXTH_CLASS_GROUP) || */Config.SayuneForAll.FREE_JUMPS_FOR_ALL &&
		    !player.isMounted() && !creature.isTransformed())
		{
			creature.setInsideZone(ZoneId.SAYUNE, true);
			ThreadPool.execute(new FlyMoveStartTask(this, player));
		}
	}

	protected override void onExit(Creature creature)
	{
		if (creature.isPlayer())
		{
			creature.setInsideZone(ZoneId.SAYUNE, false);
		}
	}

	public int getMapId()
	{
		return _mapId;
	}
}