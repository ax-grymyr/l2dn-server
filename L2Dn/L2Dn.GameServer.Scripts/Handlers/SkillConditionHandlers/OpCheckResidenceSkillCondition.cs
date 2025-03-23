using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpCheckResidence")]
public sealed class OpCheckResidenceSkillCondition: ISkillCondition
{
    private readonly FrozenSet<int> _residenceIds;
    private readonly bool _isWithin;

    public OpCheckResidenceSkillCondition(SkillConditionParameterSet parameters)
    {
        List<int>? residenceIds = parameters.GetInt32ListOptional(XmlSkillConditionParameterType.ResidenceIds);
        _residenceIds = residenceIds is null ? FrozenSet<int>.Empty : residenceIds.ToFrozenSet();
        _isWithin = parameters.GetBoolean(XmlSkillConditionParameterType.IsWithin);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        if (caster.isPlayer())
        {
            Clan? clan = caster.getActingPlayer()?.getClan();
            if (clan != null)
            {
                ClanHall? clanHall = ClanHallData.getInstance().getClanHallByClan(clan);
                if (clanHall != null)
                {
                    return _isWithin
                        ? _residenceIds.Contains(clanHall.getResidenceId())
                        : !_residenceIds.Contains(clanHall.getResidenceId());
                }
            }
        }

        return false;
    }

    public override int GetHashCode() => HashCode.Combine(_residenceIds.GetSetHashCode(), _isWithin);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._residenceIds.GetSetComparable(), x._isWithin));
}