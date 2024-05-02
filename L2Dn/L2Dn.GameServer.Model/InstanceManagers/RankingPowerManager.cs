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
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Serenitty
 */
public class RankingPowerManager
{
	private const int LEADER_STATUE = 18485;
	private static readonly TimeSpan COOLDOWN = TimeSpan.FromSeconds(43200);
	private static readonly SkillHolder LEADER_POWER = new SkillHolder(52018, 1);
	
	private Decoy _decoyInstance;
	private ScheduledFuture _decoyTask;
	
	protected RankingPowerManager()
	{
		reset();
	}
	
	public void activatePower(Player player)
	{
		Location location = player.getLocation();
		List<int> array = new();
		array.Add(location.getX());
		array.Add(location.getY());
		array.Add(location.getZ());
		GlobalVariablesManager.getInstance().setIntegerList(GlobalVariablesManager.RANKING_POWER_LOCATION, array);
		GlobalVariablesManager.getInstance().set(GlobalVariablesManager.RANKING_POWER_COOLDOWN, DateTime.UtcNow + COOLDOWN);
		createClone(player);
		cloneTask();
		SystemMessagePacket msg = new(SystemMessageId.A_RANKING_LEADER_C1_USED_LEADER_POWER_IN_S2);
		msg.Params.addString(player.getName());
		msg.Params.addZoneName(location.getX(), location.getY(), location.getZ());
		Broadcast.toAllOnlinePlayers(msg);
	}
	
	private void createClone(Player player)
	{
		Location location = player.getLocation();
		
		NpcTemplate template = NpcData.getInstance().getTemplate(LEADER_STATUE);
		_decoyInstance = new Decoy(template, player, COOLDOWN, false);
		_decoyInstance.setTargetable(false);
		_decoyInstance.setImmobilized(true);
		_decoyInstance.setInvul(true);
		_decoyInstance.spawnMe(location.getX(), location.getY(), location.getZ());
		_decoyInstance.setHeading(location.getHeading());
		_decoyInstance.broadcastStatusUpdate();
		
		AbstractScript.addSpawn(null, LEADER_STATUE, location.ToLocation3D(), location.Heading, false, COOLDOWN);
	}
	
	private void cloneTask()
	{
		_decoyTask = ThreadPool.scheduleAtFixedRate(() =>
		{
			World.getInstance().forEachVisibleObjectInRange<Player>(_decoyInstance, 300, nearby =>
			{
				BuffInfo info = nearby.getEffectList().getBuffInfoBySkillId(LEADER_POWER.getSkillId());
				if ((info == null) ||
				    (info.getTime() < (LEADER_POWER.getSkill().getAbnormalTime() - TimeSpan.FromSeconds(60))))
				{
					nearby.sendPacket(new MagicSkillUsePacket(_decoyInstance, nearby, LEADER_POWER.getSkillId(),
						LEADER_POWER.getSkillLevel(), TimeSpan.Zero, TimeSpan.Zero));
					
					LEADER_POWER.getSkill().applyEffects(_decoyInstance, nearby);
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
		GlobalVariablesManager.getInstance().remove(GlobalVariablesManager.RANKING_POWER_COOLDOWN);
		GlobalVariablesManager.getInstance().remove(GlobalVariablesManager.RANKING_POWER_LOCATION);
	}
	
	public static RankingPowerManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly RankingPowerManager INSTANCE = new RankingPowerManager();
	}
}