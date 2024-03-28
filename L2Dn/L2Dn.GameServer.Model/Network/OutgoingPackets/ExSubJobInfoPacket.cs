using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExSubJobInfoPacket: IOutgoingPacket
{
    private record struct SubInfo(int Index, CharacterClass Class, int Level, SubclassType Type);
    private readonly CharacterClass _currClassId;
    private readonly Race _currRace;
    private readonly SubclassInfoType _type;
    private readonly List<SubInfo> _subs;
	
    public ExSubJobInfoPacket(Player player, SubclassInfoType type)
    {
        _currClassId = player.getClassId();
        _currRace = player.getRace();
        _type = type;
        _subs = new();
        _subs.Add(new SubInfo(0, player.getBaseClass(), player.getStat().getBaseLevel(), SubclassType.BASECLASS));
        foreach (SubClassHolder sub in player.getSubClasses().values())
        {
            _subs.Add(new SubInfo(sub.getClassIndex(), sub.getClassDefinition(), sub.getLevel(),
                sub.isDualClass() ? SubclassType.DUALCLASS : SubclassType.SUBCLASS));
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SUBJOB_INFO);
        
        writer.WriteByte((byte)_type);
        writer.WriteInt32((int)_currClassId);
        writer.WriteInt32((int)_currRace);
        writer.WriteInt32(_subs.Count);
        foreach (SubInfo sub in _subs)
        {
            writer.WriteInt32(sub.Index);
            writer.WriteInt32((int)sub.Class);
            writer.WriteInt32(sub.Level);
            writer.WriteByte((byte)sub.Type);
        }
    }
}