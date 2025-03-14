using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestRejectPostAttachmentPacket: IIncomingPacket<GameSession>
{
    private int _msgId;

    public void ReadContent(PacketBitReader reader)
    {
        _msgId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (!Config.General.ALLOW_MAIL || !Config.General.ALLOW_ATTACHMENTS)
            return ValueTask.CompletedTask;

        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

	    // TODO: flood protection
        // if (!client.getFloodProtectors().canPerformTransaction())
        //     return ValueTask.CompletedTask;

        // if (!player.isInsideZone(ZoneId.PEACE))
        // {
        // player.sendPacket(SystemMessageId.THE_MAILBOX_FUNCTIONS_CAN_BE_USED_ONLY_IN_PEACE_ZONES_OUTSIDE_OF_THEM_YOU_CAN_ONLY_CHECK_ITS_CONTENTS);
        // return;
        // }

        Message? msg = MailManager.getInstance().getMessage(_msgId);
        if (msg == null)
            return ValueTask.CompletedTask;

        if (msg.getReceiverId() != player.ObjectId)
        {
            Util.handleIllegalPlayerAction(player, player + " tried to reject not own attachment!", Config.General.DEFAULT_PUNISH);
            return ValueTask.CompletedTask;
        }

        if (!msg.hasAttachments() || msg.getMailType() != MailType.REGULAR)
            return ValueTask.CompletedTask;

        MailManager.getInstance().sendMessage(new Message(msg));
        player.sendPacket(SystemMessageId.MAIL_SUCCESSFULLY_RETURNED);
        player.sendPacket(new ExChangePostStatePacket(true, _msgId, Message.REJECTED));

        Player? sender = World.getInstance().getPlayer(msg.getSenderId());
        if (sender != null)
        {
            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_RETURNED_THE_MAIL);
            sm.Params.addString(player.getName());
            sender.sendPacket(sm);
        }

        return ValueTask.CompletedTask;
    }
}