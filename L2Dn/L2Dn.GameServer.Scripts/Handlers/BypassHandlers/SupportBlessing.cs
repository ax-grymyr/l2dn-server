using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

public class SupportBlessing: IBypassHandler
{
	private static readonly string[] COMMANDS =
    [
        "GiveBlessing",
    ];

	public bool useBypass(string command, Player player, Creature? target)
	{
		if (target == null || !target.isNpc())
		{
			return false;
		}

		Npc npc = (Npc) target;

		// If the player is too high level, display a message and return
		if (player.getLevel() > 39 || player.getClassId().GetLevel() >= 2)
		{
			npc.showChatWindow(player, "html/default/SupportBlessingHighLevel.htm");
			return true;
		}
		npc.setTarget(player);
		SkillCaster.triggerCast(npc, player, CommonSkill.BLESSING_OF_PROTECTION.getSkill());
		return false;
	}

	public string[] getBypassList()
	{
		return COMMANDS;
	}
}