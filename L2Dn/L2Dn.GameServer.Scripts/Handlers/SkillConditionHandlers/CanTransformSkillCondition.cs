using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Transforms;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerName("CanTransform")]
public sealed class CanTransformSkillCondition: ISkillCondition
{
    private readonly int _transformId;

    public CanTransformSkillCondition(SkillConditionParameterSet parameters)
    {
        _transformId = parameters.GetInt32(XmlSkillConditionParameterType.TransformId);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        bool canTransform = true;
        Player? player = caster.getActingPlayer();
        if (player == null || player.isAlikeDead() || player.isCursedWeaponEquipped())
        {
            canTransform = false;
        }
        else if (player.isSitting())
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_TRANSFORM_WHILE_SITTING);
            canTransform = false;
        }
        else if (player.isTransformed())
        {
            player.sendPacket(SystemMessageId.YOU_ALREADY_POLYMORPHED_AND_CANNOT_POLYMORPH_AGAIN);
            canTransform = false;
        }
        else if (player.isInWater())
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_POLYMORPH_INTO_THE_DESIRED_FORM_IN_WATER);
            canTransform = false;
        }
        else if (player.isFlyingMounted() || player.isMounted())
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_TRANSFORM_WHILE_RIDING_A_PET);
            canTransform = false;
        }
        else if (CategoryData.getInstance().isInCategory(CategoryType.VANGUARD_ALL_CLASS, player.getClassId()))
        {
            Transform? transform = TransformData.getInstance().getTransform(_transformId);
            if (transform != null && transform.isRiding())
            {
                player.sendPacket(SystemMessageId.YOU_CANNOT_TRANSFORM_WHILE_RIDING_A_PET);
                canTransform = false;
            }
        }

        return canTransform;
    }
}