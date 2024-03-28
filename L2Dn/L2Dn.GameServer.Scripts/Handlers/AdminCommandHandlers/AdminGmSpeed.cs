using System.Globalization;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * A retail-like implementation of //gmspeed builder command.
 * @author lord_rex
 */
public class AdminGmSpeed: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_gmspeed",
	};

	private static Set<Stat> SPEED_STATS =
	[
		Stat.RUN_SPEED, Stat.WALK_SPEED, Stat.SWIM_RUN_SPEED, Stat.SWIM_WALK_SPEED, Stat.FLY_RUN_SPEED,
		Stat.FLY_WALK_SPEED
	];
	
	public bool useAdminCommand(String command, Player player)
	{
		StringTokenizer st = new StringTokenizer(command);
		String cmd = st.nextToken();
		if (cmd.equals("admin_gmspeed"))
		{
			if (!st.hasMoreTokens())
			{
				BuilderUtil.sendSysMessage(player, "//gmspeed [0...10]");
				return false;
			}
			String token = st.nextToken();
			
			// Rollback feature for old custom way, in order to make everyone happy.
			if (Config.USE_SUPER_HASTE_AS_GM_SPEED)
			{
				AdminCommandHandler.getInstance().useAdminCommand(player, AdminSuperHaste.ADMIN_COMMANDS[0] + " " + token, false);
				return true;
			}
			
			if (!Util.isDouble(token))
			{
				BuilderUtil.sendSysMessage(player, "//gmspeed [0...10]");
				return false;
			}
			double runSpeedBoost = double.Parse(token, CultureInfo.InvariantCulture);
			if ((runSpeedBoost < 0) || (runSpeedBoost > 10))
			{
				// Custom limit according to SDW's request - real retail limit is unknown.
				BuilderUtil.sendSysMessage(player, "//gmspeed [0...10]");
				return false;
			}
			
			Creature targetCharacter;
			WorldObject target = player.getTarget();
			if ((target != null) && target.isCreature())
			{
				targetCharacter = (Creature) target;
			}
			else
			{
				// If there is no target, let's use the command executer.
				targetCharacter = player;
			}
			
			SPEED_STATS.forEach(speedStat => targetCharacter.getStat().removeFixedValue(speedStat));
			if (runSpeedBoost > 0)
			{
				SPEED_STATS.forEach(speedStat => targetCharacter.getStat().addFixedValue(speedStat, targetCharacter.getTemplate().getBaseValue(speedStat, 120) * runSpeedBoost));
			}
			
			targetCharacter.getStat().recalculateStats(false);
			if (targetCharacter.isPlayer())
			{
				((Player) targetCharacter).broadcastUserInfo();
			}
			else
			{
				targetCharacter.broadcastInfo();
			}
			
			BuilderUtil.sendSysMessage(player, "[" + targetCharacter.getName() + "] speed is [" + (runSpeedBoost * 100) + "0]% fast.");
		}
		return true;
	}
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
