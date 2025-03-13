using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Stats;
using L2Dn.GameServer.Utilities;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author Psychokiller1888
 */
public class AdminVitality: IAdminCommandHandler
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AdminVitality));
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_set_vitality",
		"admin_full_vitality",
		"admin_empty_vitality",
		"admin_get_vitality",
    ];

	public bool useAdminCommand(string command, Player activeChar)
	{
		if (activeChar == null)
		{
			return false;
		}

		if (!Config.Character.ENABLE_VITALITY)
		{
			BuilderUtil.sendSysMessage(activeChar, "Vitality is not enabled on the server!");
			return false;
		}

		int vitality = 0;

		StringTokenizer st = new StringTokenizer(command, " ");
		string cmd = st.nextToken();
        WorldObject? target = activeChar.getTarget();
        Player? player = target?.getActingPlayer();
		if (target != null && target.isPlayer() && player != null)
		{
			if (cmd.equals("admin_set_vitality"))
			{
				try
				{
					vitality = int.Parse(st.nextToken());
				}
				catch (Exception e)
				{
                    _logger.Error(e);
					BuilderUtil.sendSysMessage(activeChar, "Incorrect vitality");
				}

                player.setVitalityPoints(vitality, true);
                player.sendMessage("Admin set your Vitality points to " + vitality);
			}
			else if (cmd.equals("admin_full_vitality"))
			{
                player.setVitalityPoints(PlayerStat.MAX_VITALITY_POINTS, true);
                player.sendMessage("Admin completly recharged your Vitality");
			}
			else if (cmd.equals("admin_empty_vitality"))
			{
                player.setVitalityPoints(PlayerStat.MIN_VITALITY_POINTS, true);
                player.sendMessage("Admin completly emptied your Vitality");
			}
			else if (cmd.equals("admin_get_vitality"))
			{
				vitality = player.getVitalityPoints();
				BuilderUtil.sendSysMessage(activeChar, "Player vitality points: " + vitality);
			}
			return true;
		}
		BuilderUtil.sendSysMessage(activeChar, "Target not found or not a player");
		return false;
	}

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}