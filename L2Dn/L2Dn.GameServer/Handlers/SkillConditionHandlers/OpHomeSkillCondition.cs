using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.SkillConditionHandlers;

/**
 * @author Sdw
 */
public class OpHomeSkillCondition: ISkillCondition
{
	private readonly ResidenceType _type;
	
	public OpHomeSkillCondition(StatSet @params)
	{
		_type = @params.getEnum<ResidenceType>("type");
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		if (caster.isPlayer())
		{
			Clan clan = caster.getActingPlayer().getClan();
			if (clan != null)
			{
				switch (_type)
				{
					case ResidenceType.CASTLE:
					{
						return CastleManager.getInstance().getCastleByOwner(clan) != null;
					}
					case ResidenceType.FORTRESS:
					{
						return FortManager.getInstance().getFortByOwner(clan) != null;
					}
					case ResidenceType.CLANHALL:
					{
						return ClanHallData.getInstance().getClanHallByClan(clan) != null;
					}
				}
			}
		}
		return false;
	}
}