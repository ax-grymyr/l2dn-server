using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Spoil effect implementation.
 * @author _drunk_, Ahmed, Zoey76
 */
public class Spoil: AbstractEffect
{
	public Spoil(StatSet @params)
	{
	}

	public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
	{
		return Formulas.calcMagicSuccess(effector, effected, skill);
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
	{
		if (!effected.isMonster() || effected.isDead())
		{
			effector.sendPacket(SystemMessageId.INVALID_TARGET);
			return;
		}

		Monster target = (Monster) effected;
		if (target.isSpoiled())
		{
			effector.sendPacket(SystemMessageId.IT_HAS_ALREADY_BEEN_SPOILED);
			return;
		}

		target.setSpoilerObjectId(effector.ObjectId);
		effector.sendPacket(SystemMessageId.THE_SPOIL_CONDITION_HAS_BEEN_ACTIVATED);
		target.getAI().notifyEvent(CtrlEvent.EVT_ATTACKED, effector);
	}
}