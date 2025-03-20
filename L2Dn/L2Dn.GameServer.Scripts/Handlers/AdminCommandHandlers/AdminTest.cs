using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @version $Revision: 1.2 $ $Date: 2004/06/27 08:12:59 $
 */
public class AdminTest: IAdminCommandHandler
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AdminTest));
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_stats",
		"admin_skill_test",
    ];

	public bool useAdminCommand(string command, Player activeChar)
	{
		if (command.equals("admin_stats"))
		{
			foreach (string line in ThreadPool.getStats())
			{
				activeChar.sendMessage(line);
			}
		}
		else if (command.startsWith("admin_skill_test"))
		{
			try
			{
				StringTokenizer st = new StringTokenizer(command);
				st.nextToken();
				int id = int.Parse(st.nextToken());
				adminTestSkill(activeChar, id, command.startsWith("admin_skill_test"));
			}
			catch (Exception e)
			{
                _logger.Error(e);
				BuilderUtil.sendSysMessage(activeChar, "Command format is //skill_test <ID>");
			}
		}
		return true;
	}

	/**
	 * @param activeChar
	 * @param id
	 * @param msu
	 */
	private void adminTestSkill(Player activeChar, int id, bool msu)
	{
		Creature caster;
		WorldObject? target = activeChar.getTarget();
		if (target == null || !target.isCreature())
		{
			caster = activeChar;
		}
		else
		{
			caster = (Creature) target;
		}

		Skill? skill = SkillData.Instance.GetSkill(id, 1);
		if (skill != null)
		{
			caster.setTarget(activeChar);
			if (msu)
			{
				caster.broadcastPacket(new MagicSkillUsePacket(caster, activeChar, id, 1, skill.HitTime, skill.ReuseDelay));
			}
			else
			{
				caster.doCast(skill);
			}
		}
	}

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}