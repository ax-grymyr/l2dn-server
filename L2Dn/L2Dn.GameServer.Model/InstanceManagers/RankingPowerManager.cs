using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Serenitty
 */
public class RankingPowerManager
{
	private const int LEADER_STATUE = 18485;
    private static readonly Logger _logger = LogManager.GetLogger(nameof(RankingPowerManager));
	private static readonly TimeSpan COOLDOWN = TimeSpan.FromSeconds(43200);
	private static readonly SkillHolder LEADER_POWER = new(52018, 1);

	private Decoy? _decoyInstance;
	private ScheduledFuture? _decoyTask;

	protected RankingPowerManager()
	{
		reset();
	}

	public void activatePower(Player player)
	{
		Location3D location = player.Location.Location3D;
		GlobalVariablesManager.getInstance().Set(GlobalVariablesManager.RANKING_POWER_LOCATION, location);
		GlobalVariablesManager.getInstance().Set(GlobalVariablesManager.RANKING_POWER_COOLDOWN, DateTime.UtcNow + COOLDOWN);
		createClone(player);
		cloneTask();
		SystemMessagePacket msg = new(SystemMessageId.A_RANKING_LEADER_C1_USED_LEADER_POWER_IN_S2);
		msg.Params.addString(player.getName());
		msg.Params.addZoneName(location.X, location.Y, location.Z);
		Broadcast.toAllOnlinePlayers(msg);
	}

	private void createClone(Player player)
	{
		Location location = player.Location;

		NpcTemplate? template = NpcData.getInstance().getTemplate(LEADER_STATUE);
        if (template == null)
        {
            _logger.Error($"Leader statue template is missing (npcId={LEADER_STATUE}).");
            return;
        }

		_decoyInstance = new Decoy(template, player, COOLDOWN, false);
		_decoyInstance.setTargetable(false);
		_decoyInstance.setImmobilized(true);
		_decoyInstance.setInvul(true);
		_decoyInstance.spawnMe(location.Location3D);
		_decoyInstance.setHeading(location.Heading);
		_decoyInstance.broadcastStatusUpdate();

		AbstractScript.addSpawn(null, LEADER_STATUE, location.Location3D, location.Heading, false, COOLDOWN);
	}

	private void cloneTask()
	{
        if (_decoyInstance == null)
        {
            _logger.Error("Leader statue instance is missing.");
            return;
        }

		_decoyTask = ThreadPool.scheduleAtFixedRate(() =>
		{
			World.getInstance().forEachVisibleObjectInRange<Player>(_decoyInstance, 300, nearby =>
			{
				BuffInfo? info = nearby.getEffectList().getBuffInfoBySkillId(LEADER_POWER.getSkillId());
				if (info == null ||
				    info.getTime() < LEADER_POWER.getSkill().AbnormalTime - TimeSpan.FromSeconds(60))
				{
					nearby.sendPacket(new MagicSkillUsePacket(_decoyInstance, nearby, LEADER_POWER.getSkillId(),
						LEADER_POWER.getSkillLevel(), TimeSpan.Zero, TimeSpan.Zero));

					LEADER_POWER.getSkill().ApplyEffects(_decoyInstance, nearby);
				}
			});
			if (Rnd.nextBoolean()) // Add some randomness?
			{
				ThreadPool.schedule(() => _decoyInstance.broadcastSocialAction(2), 4500);
			}
		}, 1000, 10000);

		ThreadPool.schedule(reset, COOLDOWN);
	}

	public void reset()
	{
		if (_decoyTask != null)
		{
			_decoyTask.cancel(false);
			_decoyTask = null;
		}
		if (_decoyInstance != null)
		{
			_decoyInstance.deleteMe();
		}
		GlobalVariablesManager.getInstance().Remove(GlobalVariablesManager.RANKING_POWER_COOLDOWN);
		GlobalVariablesManager.getInstance().Remove(GlobalVariablesManager.RANKING_POWER_LOCATION);
	}

	public static RankingPowerManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly RankingPowerManager INSTANCE = new();
	}
}