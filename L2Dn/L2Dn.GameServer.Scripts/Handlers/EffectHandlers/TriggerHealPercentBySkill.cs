using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Trigger heal percent by skill effect implementation.
 * @author NasSeKa
 */
public class TriggerHealPercentBySkill: AbstractEffect
{
	private readonly int _castSkillId;
	private readonly int _chance;
	private readonly int _power;
	
	public TriggerHealPercentBySkill(StatSet @params)
	{
		_castSkillId = @params.getInt("castSkillId");
		_chance = @params.getInt("chance", 100);
		_power = @params.getInt("power", 0);
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		if ((_chance == 0) || (_castSkillId == 0))
		{
			return;
		}

		effected.Events.Subscribe<OnCreatureSkillFinishCast>(this, onSkillUseEvent);
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.Events.Unsubscribe<OnCreatureSkillFinishCast>(onSkillUseEvent);
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.HEAL;
	}
	
	private void onSkillUseEvent(OnCreatureSkillFinishCast @event)
	{
		if (_castSkillId != @event.getSkill().getId())
		{
			return;
		}
		
		WorldObject target = @event.getTarget();
		if (target == null)
		{
			return;
		}
		
		Player player = target.getActingPlayer();
		if ((player == null) || player.isDead() || player.isHpBlocked())
		{
			return;
		}
		
		if ((_chance < 100) && (Rnd.get(100) > _chance))
		{
			return;
		}
		
		double amount = 0;
		double power = _power;
		bool full = (power == 100.0);
		
		amount = full ? player.getMaxHp() : (player.getMaxHp() * power) / 100.0;
		
		// Prevents overheal.
		amount = Math.Min(amount, Math.Max(0, player.getMaxRecoverableHp() - player.getCurrentHp()));
		if (amount >= 0)
		{
			if (amount != 0)
			{
				player.setCurrentHp(amount + player.getCurrentHp(), false);
				player.broadcastStatusUpdate(player);
			}

			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_VE_RECOVERED_S1_HP);
			sm.Params.addInt((int) amount);
			player.sendPacket(sm);
		}
	}
}