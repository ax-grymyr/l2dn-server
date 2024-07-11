using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Dispel By Slot effect implementation.
 * @author Gnacik, Zoey76, Adry_85
 */
public class DispelBySlotMyself: AbstractEffect
{
	private readonly Set<AbnormalType> _dispelAbnormals;
	
	public DispelBySlotMyself(StatSet @params)
	{
		string dispel = @params.getString("dispel");
		if (!string.IsNullOrEmpty(dispel))
		{
			_dispelAbnormals = new();
			foreach (string slot in dispel.Split(";"))
			{
				_dispelAbnormals.add(Enum.Parse<AbnormalType>(slot));
			}
		}
		else
		{
			_dispelAbnormals = new();
		}
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.DISPEL_BY_SLOT;
	}
	
	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (_dispelAbnormals.isEmpty())
		{
			return;
		}

		// The effectlist should already check if it has buff with this abnormal type or not.
		effected.getEffectList()
			.stopEffects(
				info => !info.getSkill().isIrreplacableBuff() &&
				        _dispelAbnormals.Contains(info.getSkill().getAbnormalType()), true, true);
	}
}