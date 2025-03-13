using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestDeleteReceivedPostPacket: IIncomingPacket<GameSession>
{
    private const int BATCH_LENGTH = 4; // length of the one item

    private int[]? _msgIds;

    public void ReadContent(PacketBitReader reader)
    {
        int count = reader.ReadInt32();
        if (count <= 0 || count > Config.Character.MAX_ITEM_IN_PACKET || count * BATCH_LENGTH != reader.Length)
        {
            return;
        }

        _msgIds = new int[count];
        for (int i = 0; i < count; i++)
            _msgIds[i] = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_msgIds == null || !Config.ALLOW_MAIL)
            return ValueTask.CompletedTask;

        // if (!player.isInsideZone(ZoneId.PEACE))
        // {
        // player.sendPacket(SystemMessageId.THE_MAILBOX_FUNCTIONS_CAN_BE_USED_ONLY_IN_PEACE_ZONES_OUTSIDE_OF_THEM_YOU_CAN_ONLY_CHECK_ITS_CONTENTS);
        // return;
        // }

        foreach (int msgId in _msgIds)
        {
            Message? msg = MailManager.getInstance().getMessage(msgId);
            if (msg == null)
            {
                continue;
            }
            if (msg.getReceiverId() != player.ObjectId)
            {
                Util.handleIllegalPlayerAction(player, player + " tried to delete not own post!",
                    Config.DEFAULT_PUNISH);

                return ValueTask.CompletedTask;
            }

            if (msg.hasAttachments() || msg.isDeletedByReceiver())
                return ValueTask.CompletedTask;

            msg.setDeletedByReceiver();
        }

        player.sendPacket(new ExChangePostStatePacket(true, _msgIds, Message.DELETED));

        return ValueTask.CompletedTask;
    }
}