using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

public class SkillList: IBypassHandler
{
	private static readonly string[] COMMANDS =
	{
		"SkillList"
	};
	
	public bool useBypass(string command, Player player, Creature target)
	{
		if ((target == null) || !target.isNpc())
		{
			return false;
		}

		Folk.showSkillList(player, (Npc) target, player.getClassId());
		return true;
	}
	
	public string[] getBypassList()
	{
		return COMMANDS;
	}
}