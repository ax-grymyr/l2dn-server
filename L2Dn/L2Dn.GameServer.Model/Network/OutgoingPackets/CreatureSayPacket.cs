using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public struct CreatureSayPacket: IOutgoingPacket
{
    private readonly Creature? _sender;
    private readonly ChatType _chatType;
    private readonly string? _senderName = null;
    private readonly string? _text = null;
    private readonly int _charId = 0;
    private readonly int _messageId = -1;
    private readonly int _mask;
    private List<string> _parameters;
    private readonly bool _shareLocation;

    public CreatureSayPacket(Player sender, Player receiver, string name, ChatType chatType, string text)
        : this(sender, receiver, name, chatType, text, false)
    {
    }
    public CreatureSayPacket(Player sender, Player receiver, string name, ChatType chatType, string text, bool shareLocation)
    {
        _sender = sender;
        _senderName = name;
        _chatType = chatType;
        _text = text;
        _shareLocation = shareLocation;
        if (receiver != null)
        {
            if (receiver.getFriendList().Contains(sender.ObjectId))
            {
                _mask |= 0x01;
            }
            if ((receiver.getClanId() > 0) && (receiver.getClanId() == sender.getClanId()))
            {
                _mask |= 0x02;
            }
            if ((MentorManager.getInstance().getMentee(receiver.ObjectId, sender.ObjectId) != null) || (MentorManager.getInstance().getMentee(sender.ObjectId, receiver.ObjectId) != null))
            {
                _mask |= 0x04;
            }
            if ((receiver.getAllyId() > 0) && (receiver.getAllyId() == sender.getAllyId()))
            {
                _mask |= 0x08;
            }
        }
        // Does not shows level
        if (sender.isGM())
        {
            _mask |= 0x10;
        }
    }
	
    public CreatureSayPacket(Creature? sender, ChatType chatType, string senderName, string text)
        : this(sender, chatType, senderName, text, false)
    {
    }
	
    public CreatureSayPacket(Creature? sender, ChatType chatType, string senderName, string text, bool shareLocation)
    {
        _sender = sender;
        _chatType = chatType;
        _senderName = senderName;
        _text = text;
        _shareLocation = shareLocation;
    }
	
    public CreatureSayPacket(Creature? sender, ChatType chatType, NpcStringId npcStringId)
    {
        _sender = sender;
        _chatType = chatType;
        _messageId = (int)npcStringId;
        if (sender != null)
        {
            _senderName = sender.getName();
        }
    }
	
    public CreatureSayPacket(ChatType chatType, int charId, SystemMessageId systemMessageId)
    {
        _sender = null;
        _chatType = chatType;
        _charId = charId;
        _messageId = (int)systemMessageId;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SAY2);
        writer.WriteInt32(_sender?.ObjectId ?? 0);
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
            foreach (string s in _parameters)
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
                writer.WriteByte((byte)clan.getCastleId());
            }
            else
            {
                writer.WriteByte(0);
            }

            if (_shareLocation)
            {
                writer.WriteByte(1);
                writer.WriteInt16((short)SharedTeleportManager.getInstance().nextId(_sender));
            }
        }
        else
        {
            writer.WriteByte(0);
        }
    }
}