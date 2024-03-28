using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Sdw
 */
public class OpCheckResidenceSkillCondition: ISkillCondition
{
	private readonly Set<int> _residenceIds = new();
	private readonly bool _isWithin;
	
	public OpCheckResidenceSkillCondition(StatSet @params)
	{
		_residenceIds.addAll(@params.getList<int>("residenceIds"));
		_isWithin = @params.getBoolean("isWithin");
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		if (caster.isPlayer())
		{
			Clan clan = caster.getActingPlayer().getClan();
			if (clan != null)
			{
				ClanHall clanHall = ClanHallData.getInstance().getClanHallByClan(clan);
				if (clanHall != null)
				{
					return _isWithin ? _residenceIds.Contains(clanHall.getResidenceId()) : !_residenceIds.Contains(clanHall.getResidenceId());
				}
			}
		}
		return false;
	}
}