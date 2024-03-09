using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Change Face effect implementation.
 * @author Zoey76
 */
public class ChangeFace: AbstractEffect
{
	private readonly int _value;
	
	public ChangeFace(StatSet @params)
	{
		_value = @params.getInt("value", 0);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (!effected.isPlayer())
		{
			return;
		}
		
		Player player = effected.getActingPlayer();
		player.getAppearance().setFace(_value);
		player.broadcastUserInfo();
	}
}