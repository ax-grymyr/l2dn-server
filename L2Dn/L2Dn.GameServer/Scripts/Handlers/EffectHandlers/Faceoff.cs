using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class Faceoff: AbstractEffect
{
	public Faceoff(StatSet @params)
	{
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.FACEOFF.getMask();
	}
	
	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return effected.isPlayer();
	}
}