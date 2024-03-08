using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Events.Listeners;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author NasSeKa
 */
public class ReuseSkillIdByDamage: AbstractEffect
{
	private readonly int _minAttackerLevel;
	private readonly int _maxAttackerLevel;
	private readonly int _minDamage;
	private readonly int _chance;
	private readonly int _hpPercent;
	private readonly int _skillId;
	private readonly int _amount;
	private readonly InstanceType _attackerType;
	
	public ReuseSkillIdByDamage(StatSet @params)
	{
		_minAttackerLevel = @params.getInt("minAttackerLevel", 1);
		_maxAttackerLevel = @params.getInt("maxAttackerLevel", int.MaxValue);
		_minDamage = @params.getInt("minDamage", 1);
		_chance = @params.getInt("chance", 100);
		_hpPercent = @params.getInt("hpPercent", 100);
		_skillId = @params.getInt("skillId", 0);
		_amount = @params.getInt("amount", 0);
		_attackerType = @params.getEnum("attackerType", InstanceType.Creature);
	}
	
	private void onDamageReceivedEvent(OnCreatureDamageReceived @event)
	{
		if (@event.isDamageOverTime() || (_chance == 0))
		{
			return;
		}
		
		if (@event.getAttacker() == @event.getTarget())
		{
			return;
		}
		
		if ((@event.getAttacker().getLevel() < _minAttackerLevel) || (@event.getAttacker().getLevel() > _maxAttackerLevel))
		{
			return;
		}
		
		if (@event.getDamage() < _minDamage)
		{
			return;
		}
		
		if ((_chance < 100) && (Rnd.get(100) > _chance))
		{
			return;
		}
		
		if ((_hpPercent < 100) && (@event.getAttacker().getCurrentHpPercent() > _hpPercent))
		{
			return;
		}
		
		if (!@event.getAttacker().getInstanceType().IsType(_attackerType))
		{
			return;
		}
		
		Player player = (Player) @event.getTarget();
		Skill s = player.getKnownSkill(_skillId);
		if (s != null)
		{
			if (_amount > 0)
			{
				TimeSpan reuse = player.getSkillRemainingReuseTime(s.getReuseHashCode());
				if (reuse > TimeSpan.Zero)
				{
					TimeSpan diff = reuse - TimeSpan.FromMilliseconds(_amount);
					diff = Algorithms.Max(diff, TimeSpan.Zero);

					player.removeTimeStamp(s);
					player.addTimeStamp(s, diff);
					player.sendPacket(new SkillCoolTimePacket(player));
				}
			}
			else
			{
				player.removeTimeStamp(s);
				player.enableSkill(s);
				player.sendPacket(new SkillCoolTimePacket(player));
			}
		}
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.removeListenerIf(EventType.ON_CREATURE_DAMAGE_RECEIVED, listener => listener.getOwner() == this);
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		effected.addListener(new ConsumerEventListener(effected, EventType.ON_CREATURE_DAMAGE_RECEIVED,
			@event => onDamageReceivedEvent((OnCreatureDamageReceived)@event), this));
	}
}
