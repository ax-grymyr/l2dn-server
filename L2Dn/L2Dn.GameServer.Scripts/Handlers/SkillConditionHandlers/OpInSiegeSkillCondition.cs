using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author dontknowdontcare
 */
public class OpInSiegeSkillCondition: ISkillCondition
{
	private readonly Set<int> _residenceIds = new();

	public OpInSiegeSkillCondition(StatSet @params)
	{
		_residenceIds.addAll(@params.getList<int>("residenceIds"));
	}

	public bool canUse(Creature caster, Skill skill, WorldObject? target)
	{
		foreach (int id in _residenceIds)
		{
			if (valid(caster, id))
			{
				return true;
			}
		}
		return false;
	}

	private bool valid(Creature caster, int id)
	{
		FortSiege? fortSiege = FortSiegeManager.getInstance().getSiege(id);
		if (fortSiege != null)
		{
			return fortSiege.isInProgress() && fortSiege.getFort().getZone().isInsideZone(caster);
		}

		Siege? castleSiege = SiegeManager.getInstance().getSiege(id);
		if (castleSiege != null)
		{
			return castleSiege.isInProgress() && castleSiege.getCastle().getZone().isInsideZone(caster);
		}

		// TODO: Check for clan hall siege

		return false;
	}
}