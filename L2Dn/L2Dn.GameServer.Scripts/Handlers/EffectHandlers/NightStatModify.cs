using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class NightStatModify: AbstractEffect
{
	private static readonly Set<Creature> NIGHT_STAT_CHARACTERS = new();
	private static readonly int SHADOW_SENSE = 294;
	
	private static bool START_LISTENER = true;
	
	private readonly Stat _stat;
	private readonly double _amount;
	protected StatModifierType _mode;
	
	public NightStatModify(StatSet @params)
	{
		_stat = @params.getEnum<Stat>("stat");
		_amount = @params.getDouble("amount");
		_mode = @params.getEnum("mode", StatModifierType.DIFF);
		
		// Run only once per daytime change.
		if (START_LISTENER)
		{
			START_LISTENER = false;
			
			// Init a global day-night change listener.
			GlobalEvents.Global.Subscribe<OnDayNightChange>(this, onDayNightChange);
		}
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		NIGHT_STAT_CHARACTERS.add(effected);
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		NIGHT_STAT_CHARACTERS.remove(effected);
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		// Not night.
		if (!GameTimeTaskManager.getInstance().isNight())
		{
			return;
		}
		
		// Apply stat.
		switch (_mode)
		{
			case StatModifierType.DIFF:
			{
				effected.getStat().mergeAdd(_stat, _amount);
				break;
			}
			case StatModifierType.PER:
			{
				effected.getStat().mergeMul(_stat, (_amount / 100) + 1);
				break;
			}
		}
	}
	
	public void onDayNightChange(OnDayNightChange @event)
	{
		// System message for Shadow Sense.
		SystemMessagePacket msg = new SystemMessagePacket(@event.isNight()
			? SystemMessageId.IT_IS_NOW_MIDNIGHT_AND_THE_EFFECT_OF_S1_CAN_BE_FELT
			: SystemMessageId.IT_IS_DAWN_AND_THE_EFFECT_OF_S1_WILL_NOW_DISAPPEAR);
		
		msg.Params.addSkillName(SHADOW_SENSE);
		
		foreach (Creature creature in NIGHT_STAT_CHARACTERS)
		{
			// Pump again.
			creature.getStat().recalculateStats(true);
			
			// Send Shadow Sense message when player has skill.
			if (creature.getKnownSkill(SHADOW_SENSE) != null)
			{
				creature.sendPacket(msg);
			}
		}
	}
}