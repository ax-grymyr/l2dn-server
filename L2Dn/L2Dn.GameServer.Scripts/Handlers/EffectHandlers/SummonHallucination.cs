using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class SummonHallucination: AbstractEffect
{
	private readonly int _despawnDelay;
	private readonly int _npcId;
	private readonly int _npcCount;
	
	public SummonHallucination(StatSet @params)
	{
		_despawnDelay = @params.getInt("despawnDelay", 20000);
		_npcId = @params.getInt("npcId", 0);
		_npcCount = @params.getInt("npcCount", 1);
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.SUMMON_NPC;
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isAlikeDead())
		{
			return;
		}
		
		if ((_npcId <= 0) || (_npcCount <= 0))
		{
			LOGGER.Warn(GetType().Name + ": Invalid NPC ID or count skill ID: " + skill.getId());
			return;
		}
		
		Player player = effector.getActingPlayer();
		if (player.isMounted())
		{
			return;
		}
		
		NpcTemplate npcTemplate = NpcData.getInstance().getTemplate(_npcId);
		if (npcTemplate == null)
		{
			LOGGER.Warn(GetType().Name + ": Spawn of the nonexisting NPC ID: " + _npcId + ", skill ID:" + skill.getId());
			return;
		}
		
		int x = effected.getX();
		int y = effected.getY();
		int z = effected.getZ();
		
		for (int i = 0; i < _npcCount; i++)
		{
			x += (Rnd.nextBoolean() ? Rnd.get(0, 20) : Rnd.get(-20, 0));
			y += (Rnd.nextBoolean() ? Rnd.get(0, 20) : Rnd.get(-20, 0));
			
			Doppelganger clone = new Doppelganger(npcTemplate, player);
			clone.setCurrentHp(clone.getMaxHp());
			clone.setCurrentMp(clone.getMaxMp());
			clone.setSummoner(player);
			clone.spawnMe(new Location3D(x, y, z));
			clone.scheduleDespawn(TimeSpan.FromMilliseconds(_despawnDelay));
			clone.startAttackTask(effected);
		}
	}
}