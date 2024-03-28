using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Focus Souls effect implementation.
 * @author nBd, Adry_85
 */
public class FocusSouls: AbstractEffect
{
	private readonly int _charge;
	private readonly SoulType _type;
	
	public FocusSouls(StatSet @params)
	{
		_charge = @params.getInt("charge", 0);
		_type = @params.getEnum("type", SoulType.LIGHT);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (!effected.isPlayer() || effected.isAlikeDead())
		{
			return;
		}
		
		Player target = effected.getActingPlayer();
		int maxSouls = (int) target.getStat().getValue(Stat.MAX_SOULS, 0);
		if (maxSouls > 0)
		{
			int amount = _charge;
			if ((target.getChargedSouls(_type) < maxSouls))
			{
				int count = ((target.getChargedSouls(_type) + amount) <= maxSouls) ? amount : (maxSouls - target.getChargedSouls(_type));
				target.increaseSouls(count, _type);
			}
			else
			{
				target.sendPacket(SystemMessageId.SOUL_CANNOT_BE_INCREASED_ANYMORE);
			}
		}
	}
}