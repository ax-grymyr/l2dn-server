using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerName("OpEnergyMax")]
public sealed class OpEnergyMaxSkillCondition: ISkillCondition
{
    private readonly int _amount;

    public OpEnergyMaxSkillCondition(SkillConditionParameterSet parameters)
    {
        _amount = parameters.GetInt32(XmlSkillConditionParameterType.Amount);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Player? player = caster.getActingPlayer();
        if (player != null && player.getCharges() >= _amount)
        {
            caster.sendPacket(SystemMessageId.YOUR_FORCE_HAS_REACHED_MAXIMUM_CAPACITY);
            return false;
        }

        return true;
    }
}