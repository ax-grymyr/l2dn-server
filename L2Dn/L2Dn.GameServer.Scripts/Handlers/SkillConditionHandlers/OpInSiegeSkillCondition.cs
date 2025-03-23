using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpInSiege")]
public sealed class OpInSiegeSkillCondition: ISkillCondition
{
    private readonly FrozenSet<int> _residenceIds;

    public OpInSiegeSkillCondition(SkillConditionParameterSet parameters)
    {
        List<int>? residenceIds = parameters.GetInt32ListOptional(XmlSkillConditionParameterType.ResidenceIds);
        _residenceIds = residenceIds is null ? FrozenSet<int>.Empty : residenceIds.ToFrozenSet();
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        foreach (int id in _residenceIds)
        {
            if (Valid(caster, id))
            {
                return true;
            }
        }

        return false;
    }

    private static bool Valid(Creature caster, int id)
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

    public override int GetHashCode() => _residenceIds.GetSetHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._residenceIds.GetSetComparable());
}