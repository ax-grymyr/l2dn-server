using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A scripted zone... Creation of such a zone should require somekind of script reference which can handle onEnter() / onExit()
 * @author durgus
 */
public class ScriptZone(int id, ZoneForm form): ZoneType(id, form)
{
    protected override void onEnter(Creature creature)
	{
		creature.setInsideZone(ZoneId.SCRIPT, true);
	}

	protected override void onExit(Creature creature)
	{
		creature.setInsideZone(ZoneId.SCRIPT, false);
	}
}