using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

public sealed class OpHomeSkillCondition: ISkillCondition
{
    private readonly ResidenceType _type;

    public OpHomeSkillCondition(SkillConditionParameterSet parameters)
    {
        _type = parameters.GetEnum<ResidenceType>(XmlSkillConditionParameterType.Type);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Player? player = caster.getActingPlayer();
        if (caster.isPlayer() && player != null)
        {
            Clan? clan = player.getClan();
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