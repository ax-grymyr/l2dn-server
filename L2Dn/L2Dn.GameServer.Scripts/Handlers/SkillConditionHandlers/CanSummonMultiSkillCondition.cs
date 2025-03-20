using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerName("CanSummonMulti")]
public sealed class CanSummonMultiSkillCondition: ISkillCondition
{
    private readonly int _summonPoints;

    public CanSummonMultiSkillCondition(SkillConditionParameterSet parameters)
    {
        _summonPoints = parameters.GetInt32(XmlSkillConditionParameterType.SummonPoints);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Player? player = caster.getActingPlayer();
        if (player == null || player.isSpawnProtected() || player.isTeleportProtected())
        {
            return false;
        }

        bool canSummon = true;

        int servitorSize = player.getServitors().Count;
        if (servitorSize == 1 && player.getSummonPoints() == 0)
        {
            canSummon = false;
        }
        else if (servitorSize > 4)
        {
            canSummon = false;
        }
        else if (player.isFlyingMounted() || player.isMounted() || player.inObserverMode() || player.isTeleporting())
        {
            canSummon = false;
        }
        else if (player.isInAirShip())
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_SUMMON_A_SERVITOR_WHILE_MOUNTED);
            canSummon = false;
        }
        else if (player.getSummonPoints() + _summonPoints > player.getMaxSummonPoints())
        {
            canSummon = false;
        }

        return canSummon;
    }
}