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
public class DispelBySlot: AbstractEffect
{
	private readonly string _dispel;
	private readonly Map<AbnormalType, short> _dispelAbnormals;
	
	public DispelBySlot(StatSet @params)
	{
		_dispel = @params.getString("dispel");
		if (!string.IsNullOrEmpty(_dispel))
		{
			_dispelAbnormals = new();
			foreach (string ngtStack in _dispel.Split(";"))
			{
				string[] ngt = ngtStack.Split(",");
				_dispelAbnormals.put(Enum.Parse<AbnormalType>(ngt[0]), short.Parse(ngt[1]));
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

		// Continue only if target has any of the abnormals. Save useless cycles.
		if (effected.getEffectList().hasAbnormalType(_dispelAbnormals.Keys))
		{
			// Dispel transformations (buff and by GM)
			short transformToDispel = _dispelAbnormals.get(AbnormalType.TRANSFORM);
			if ((transformToDispel != null) &&
			    ((transformToDispel == effected.getTransformationId()) || (transformToDispel < 0)))
			{
				effected.stopTransformation(true);
			}

			effected.getEffectList().stopEffects(info =>
			{
				// We have already dealt with transformation from above.
				if (info.isAbnormalType(AbnormalType.TRANSFORM))
				{
					return false;
				}

				short abnormalLevel = _dispelAbnormals.get(info.getSkill().getAbnormalType());
				return (abnormalLevel != null) &&
				       ((abnormalLevel < 0) || (abnormalLevel >= info.getSkill().getAbnormalLevel()));
			}, true, true);
		}
	}
}
