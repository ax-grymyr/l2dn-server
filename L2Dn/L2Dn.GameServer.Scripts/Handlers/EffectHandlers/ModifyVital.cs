using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Modify vital effect implementation.
 * @author malyelfik
 */
public class ModifyVital: AbstractEffect
{
	// Modify types
	private enum ModifyType
	{
		DIFF,
		SET,
		PER
	}
	
	// Effect parameters
	private readonly ModifyType _type;
	private readonly int _hp;
	private readonly int _mp;
	private readonly int _cp;
	
	public ModifyVital(StatSet @params)
	{
		_type = @params.getEnum<ModifyType>("type");
		if (_type != ModifyType.SET)
		{
			_hp = @params.getInt("hp", 0);
			_mp = @params.getInt("mp", 0);
			_cp = @params.getInt("cp", 0);
		}
		else
		{
			_hp = @params.getInt("hp", -1);
			_mp = @params.getInt("mp", -1);
			_cp = @params.getInt("cp", -1);
		}
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
	{
		return !effected.isRaid() && !effected.isRaidMinion();
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isDead())
		{
			return;
		}
		
		if (effector.isPlayer() && effected.isPlayer() && effected.isAffected(EffectFlag.DUELIST_FURY) && !effector.isAffected(EffectFlag.DUELIST_FURY))
		{
			return;
		}
		
		switch (_type)
		{
			case ModifyType.DIFF:
			{
				effected.setCurrentCp(effected.getCurrentCp() + _cp);
				effected.setCurrentHp(effected.getCurrentHp() + _hp);
				effected.setCurrentMp(effected.getCurrentMp() + _mp);
				break;
			}
			case ModifyType.SET:
			{
				if (_cp >= 0)
				{
					effected.setCurrentCp(_cp);
				}
				if (_hp >= 0)
				{
					effected.setCurrentHp(_hp);
				}
				if (_mp >= 0)
				{
					effected.setCurrentMp(_mp);
				}
				break;
			}
			case ModifyType.PER:
			{
				effected.setCurrentCp(effected.getCurrentCp() + effected.getMaxCp() * (_cp / 100.0));
				effected.setCurrentHp(effected.getCurrentHp() + effected.getMaxHp() * (_hp / 100.0));
				effected.setCurrentMp(effected.getCurrentMp() + effected.getMaxMp() * (_mp / 100.0));
				break;
			}
		}
	}
}