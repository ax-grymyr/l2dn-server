using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Bluff effect implementation.
 * @author decad
 */
public class Bluff: AbstractEffect
{
	private readonly int _chance;
	
	public Bluff(StatSet @params)
	{
		_chance = @params.getInt("chance", 100);
	}
	
	public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
	{
		return Formulas.calcProbability(_chance, effector, effected, skill);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		// Headquarters NPC should not rotate
		if ((effected.getId() == 35062) || effected.isRaid() || effected.isRaidMinion())
		{
			return;
		}
		
		effected.broadcastPacket(new StartRotationPacket(effected.getObjectId(), effected.getHeading(), 1, 65535));
		effected.broadcastPacket(new StopRotationPacket(effected.getObjectId(), effector.getHeading(), 65535));
		effected.setHeading(effector.getHeading());
	}
}