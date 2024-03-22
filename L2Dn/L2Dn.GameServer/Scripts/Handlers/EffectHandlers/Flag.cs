using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Flag effect implementation.
 * @author BiggBoss
 */
public class Flag: AbstractEffect
{
	public Flag(StatSet @params)
	{
	}
	
	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return (effected != null) && effected.isPlayer();
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		effected.updatePvPFlag(PvpFlagStatus.Enabled);
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.getActingPlayer().updatePvPFlag(PvpFlagStatus.None);
	}
}