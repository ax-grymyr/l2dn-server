using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Annotations;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Network.OutgoingPackets.ClassChange;
using L2Dn.GameServer.StaticData;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.AI.Players;

public sealed class PlayerClassChange: AbstractScript
{
    private const int FirstClassMinLevel = 20;
    private const int SecondClassMinLevel = 40;
    
    [SubscribeEvent(SubscriptionType.GlobalPlayers)]
    public void OnPlayerLevelChanged(OnPlayerLevelChanged ev) => CheckRequirementAndNotify(ev.getPlayer());

    [SubscribeEvent(SubscriptionType.GlobalPlayers)]
    public void OnPlayerLogin(OnPlayerLogin ev) => CheckRequirementAndNotify(ev.getPlayer());

    private void CheckRequirementAndNotify(Player player)
    {
        if (player == null)
            return;

        if (player.getLevel() >= FirstClassMinLevel && CategoryData.Instance
                .IsInCategory(CategoryType.FIRST_CLASS_GROUP, player.getClassId()) || 
            player.getLevel() >= SecondClassMinLevel && CategoryData.Instance
                .IsInCategory(CategoryType.SECOND_CLASS_GROUP, player.getClassId()))
        {
            player.sendPacket(ExClassChangeSetAlarmPacket.STATIC_PACKET);
        }
    }
}