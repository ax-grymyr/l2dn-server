using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.TaskManagers;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Resurrection effect implementation.
 * @author Adry_85
 */
public class Resurrection: AbstractEffect
{
	private readonly int _power;
	private readonly int _hpPercent;
	private readonly int _mpPercent;
	private readonly int _cpPercent;

	public Resurrection(StatSet @params)
	{
		_power = @params.getInt("power", 0);
		_hpPercent = @params.getInt("hpPercent", 0);
		_mpPercent = @params.getInt("mpPercent", 0);
		_cpPercent = @params.getInt("cpPercent", 0);
	}

	public override EffectType getEffectType()
	{
		return EffectType.RESURRECTION;
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        Player? effectedPlayer = effected.getActingPlayer();

        if (effector.isPlayer() && player != null)
		{
            if (effectedPlayer == null)
                return;

            if (!effectedPlayer.isResurrectionBlocked() && !effectedPlayer.isReviveRequested())
			{
                effectedPlayer.reviveRequest(player, effected.isPet(), _power, _hpPercent, _mpPercent, _cpPercent);
			}
		}
		else
		{
			DecayTaskManager.getInstance().cancel(effected);
			effected.doRevive(Formulas.calculateSkillResurrectRestorePercent(_power, effector));
		}
	}
}