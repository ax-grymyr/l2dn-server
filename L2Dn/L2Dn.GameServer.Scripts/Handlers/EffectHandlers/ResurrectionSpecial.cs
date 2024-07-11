using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Resurrection Special effect implementation.
 * @author Zealar
 */
public class ResurrectionSpecial: AbstractEffect
{
	private readonly int _power;
	private readonly int _hpPercent;
	private readonly int _mpPercent;
	private readonly int _cpPercent;
	private readonly Set<int> _instanceId;
	
	public ResurrectionSpecial(StatSet @params)
	{
		_power = @params.getInt("power", 0);
		_hpPercent = @params.getInt("hpPercent", 0);
		_mpPercent = @params.getInt("mpPercent", 0);
		_cpPercent = @params.getInt("cpPercent", 0);
		
		string instanceIds = @params.getString("instanceId", null);
		if (!string.IsNullOrEmpty(instanceIds))
		{
			_instanceId = new();
			foreach (string id in instanceIds.Split(";"))
			{
				_instanceId.add(int.Parse(id));
			}
		}
		else
		{
			_instanceId = new();
		}
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.RESURRECTION_SPECIAL;
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.RESURRECTION_SPECIAL.getMask();
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		if (!effected.isPlayer() && !effected.isPet())
		{
			return;
		}
		
		if (effected.getActingPlayer().isInOlympiadMode())
		{
			return;
		}
		
		Player caster = effector.getActingPlayer();
		Instance instance = caster.getInstanceWorld();
		if (!_instanceId.isEmpty() && ((instance == null) || !_instanceId.Contains(instance.getTemplateId())))
		{
			return;
		}
		
		if (effected.isPlayer())
		{
			effected.getActingPlayer().reviveRequest(caster, false, _power, _hpPercent, _mpPercent, _cpPercent);
		}
		else if (effected.isPet())
		{
			Pet pet = (Pet) effected;
			effected.getActingPlayer().reviveRequest(pet.getActingPlayer(), true, _power, _hpPercent, _mpPercent, _cpPercent);
		}
	}
}