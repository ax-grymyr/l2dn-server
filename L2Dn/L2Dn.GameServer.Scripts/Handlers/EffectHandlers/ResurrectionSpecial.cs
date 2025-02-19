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

		string instanceIds = @params.getString("instanceId", string.Empty);
		if (!string.IsNullOrEmpty(instanceIds))
		{
			_instanceId = [];
			foreach (string id in instanceIds.Split(";"))
			{
				_instanceId.add(int.Parse(id));
			}
		}
		else
		{
			_instanceId = [];
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
        Player? effectedPlayer = effected.getActingPlayer();
		if (!effected.isPlayer() && !effected.isPet() || effectedPlayer == null)
		{
			return;
		}

		if (effectedPlayer.isInOlympiadMode())
		{
			return;
		}

		Player? caster = effector.getActingPlayer();
		Instance? instance = caster?.getInstanceWorld();
		if (!_instanceId.isEmpty() && (instance == null || !_instanceId.Contains(instance.getTemplateId())))
		{
			return;
		}

		if (effected.isPlayer() && caster != null)
		{
            effectedPlayer.reviveRequest(caster, false, _power, _hpPercent, _mpPercent, _cpPercent);
		}
		else if (effected.isPet())
		{
			Pet pet = (Pet) effected;
            effectedPlayer.reviveRequest(pet.getActingPlayer(), true, _power, _hpPercent, _mpPercent, _cpPercent);
		}
	}
}