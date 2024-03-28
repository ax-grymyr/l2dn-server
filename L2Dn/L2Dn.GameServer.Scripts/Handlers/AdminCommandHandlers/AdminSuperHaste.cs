using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * The classical custom L2J implementation of the old //gmspeed GM command.
 * @author lord_rex (No, it wasn't me at all. Eclipse added my name there.)
 */
public class AdminSuperHaste: IAdminCommandHandler
{
	public static readonly String[] ADMIN_COMMANDS =
	{
		"admin_superhaste",
		"admin_superhaste_menu",
		"admin_speed",
		"admin_speed_menu",
	};
	
	private static int SUPER_HASTE_ID = 7029;
	
	public bool useAdminCommand(String command, Player player)
	{
		StringTokenizer st = new StringTokenizer(command);
		String cmd = st.nextToken();
		switch (cmd)
		{
			case "admin_superhaste":
			case "admin_speed":
			{
				try
				{
					int val = int.Parse(st.nextToken());
					bool sendMessage = player.isAffectedBySkill(SUPER_HASTE_ID);
					player.stopSkillEffects((val == 0) && sendMessage ? SkillFinishType.REMOVED : SkillFinishType.NORMAL, SUPER_HASTE_ID);
					if ((val >= 1) && (val <= 4))
					{
						int time = 0;
						if (st.hasMoreTokens())
						{
							time = int.Parse(st.nextToken());
						}
						
						Skill superHasteSkill = SkillData.getInstance().getSkill(SUPER_HASTE_ID, val);
						superHasteSkill.applyEffects(player, player, true, TimeSpan.FromSeconds(time));
					}
				}
				catch (Exception e)
				{
					player.sendMessage("Usage: //superhaste <Effect level (0-4)> <Time in seconds>");
				}
				break;
			}
			case "admin_superhaste_menu":
			case "admin_speed_menu":
			{
				AdminHtml.showAdminHtml(player, "gm_menu.htm");
				break;
			}
		}
		
		return true;
	}
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
