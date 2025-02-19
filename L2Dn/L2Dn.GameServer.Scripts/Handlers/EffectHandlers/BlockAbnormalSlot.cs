using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Block Buff Slot effect implementation.
 * @author Zoey76
 */
public class BlockAbnormalSlot: AbstractEffect
{
	private readonly Set<AbnormalType> _blockAbnormalSlots = [];

	public BlockAbnormalSlot(StatSet @params)
	{
		foreach (string slot in @params.getString("slot").Split(";"))
		{
			if (!string.IsNullOrEmpty(slot))
			{
				_blockAbnormalSlots.add(Enum.Parse<AbnormalType>(slot));
			}
		}
	}

	public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
	{
		effected.getEffectList().addBlockedAbnormalTypes(_blockAbnormalSlots);
	}

	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.getEffectList().removeBlockedAbnormalTypes(_blockAbnormalSlots);
	}
}