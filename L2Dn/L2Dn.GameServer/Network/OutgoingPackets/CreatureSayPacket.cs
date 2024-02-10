using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public struct CreatureSayPacket: IOutgoingPacket
{
    private readonly Creature _sender;
    private readonly ChatType _chatType;
    private String _senderName = null;
    private String _text = null;
    private int _charId = 0;
    private int _messageId = -1;
    private int _mask;
    private List<String> _parameters;
    private bool _shareLocation;

    public CreatureSayPacket()
    {
        
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(ServerPacketCode.SAY2);
        writer.WriteInt32(_sender == null ? 0 : _sender.getObjectId());
        writer.WriteInt32((int)_chatType);
        if (_senderName != null)
        {
            writer.WriteString(_senderName);
        }
        else
        {
            writer.WriteInt32(_charId);
        }

        writer.WriteInt32(_messageId); // High Five NPCString ID

        if (_text != null)
        {
            writer.WriteString(_text);
            if ((_sender != null) && (_sender.isPlayer() || _sender.isFakePlayer()) && (_chatType == ChatType.WHISPER))
            {
                writer.WriteByte((byte)_mask);
                if ((_mask & 0x10) == 0)
                {
                    writer.WriteByte((byte)_sender.getLevel());
                }
            }
        }
        else if (_parameters != null)
        {
            foreach (String s in _parameters)
            {
                writer.WriteString(s);
            }
        }

        // Rank
        if ((_sender != null) && _sender.isPlayer())
        {
            Clan clan = _sender.getClan();
            if ((clan != null) && ((_chatType == ChatType.CLAN) || (_chatType == ChatType.ALLIANCE)))
            {
                writer.WriteByte(0); // unknown clan byte
            }

            int rank = RankManager.getInstance().getPlayerGlobalRank(_sender.getActingPlayer());
            if ((rank == 0) || (rank > 100))
            {
                writer.WriteByte(0);
            }
            else if (rank <= 10)
            {
                writer.WriteByte(1);
            }
            else if (rank <= 50)
            {
                writer.WriteByte(2);
            }
            else if (rank <= 100)
            {
                writer.WriteByte(3);
            }

            if (clan != null)
            {
                writer.WriteByte(clan.getCastleId());
            }
            else
            {
                writer.WriteByte(0);
            }

            if (_shareLocation)
            {
                writer.WriteByte(1);
                writer.WriteInt16(SharedTeleportManager.getInstance().nextId(_sender));
            }
        }
        else
        {
            writer.WriteByte(0);
        }
    }
}