using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class SendSystemMessageToClan: AbstractEffect
{
    private readonly SystemMessageId _messageId;
    private readonly SystemMessagePacket _message;

    public SendSystemMessageToClan(EffectParameterSet parameters)
    {
        int id = parameters.GetInt32(XmlSkillEffectParameterType.Id, 0);
        _messageId = (SystemMessageId)id;
        _message = new SystemMessagePacket((SystemMessageId)id);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (player == null)
            return;

        Clan? clan = player.getClan();
        if (clan != null)
            clan.broadcastToOnlineMembers(_message);
    }

    public override int GetHashCode() => HashCode.Combine(_messageId);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._messageId);
}