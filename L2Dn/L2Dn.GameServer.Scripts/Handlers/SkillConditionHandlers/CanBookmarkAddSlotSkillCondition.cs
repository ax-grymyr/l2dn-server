using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("CanBookmarkAddSlot")]
public sealed class CanBookmarkAddSlotSkillCondition: ISkillCondition
{
    private readonly int _teleportBookmarkSlots;

    public CanBookmarkAddSlotSkillCondition(SkillConditionParameterSet parameters)
    {
        _teleportBookmarkSlots = parameters.GetInt32(XmlSkillConditionParameterType.TeleportBookmarkSlots);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Player? player = caster.getActingPlayer();
        if (player == null)
            return false;

        if (player.getBookMarkSlot() + _teleportBookmarkSlots > 30)
        {
            player.sendPacket(SystemMessageId.
                YOU_HAVE_REACHED_THE_MAXIMUM_NUMBER_OF_MY_TELEPORT_SLOTS_OR_USE_CONDITIONS_ARE_NOT_OBSERVED);

            return false;
        }

        return true;
    }
}