using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Dispel By Slot Probability effect implementation.
 * @author Adry_85, Zoey76
 */
public class DispelBySlotProbability: AbstractEffect
{
	private readonly Set<AbnormalType> _dispelAbnormals;
	private readonly int _rate;
	
	public DispelBySlotProbability(StatSet @params)
	{
		String[] dispelEffects = @params.getString("dispel").Split(";");
		_rate = @params.getInt("rate", 100);
		_dispelAbnormals = new();
		foreach (String slot in dispelEffects)
		{
			_dispelAbnormals.add(Enum.Parse<AbnormalType>(slot));
		}
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.DISPEL;
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected == null)
		{
			return;
		}
		
		// The effectlist should already check if it has buff with this abnormal type or not.
		effected.getEffectList()
			.stopEffects(
				info => !info.getSkill().isIrreplacableBuff() && (Rnd.get(100) < _rate) &&
				        _dispelAbnormals.Contains(info.getSkill().getAbnormalType()), true, true);
	}
}