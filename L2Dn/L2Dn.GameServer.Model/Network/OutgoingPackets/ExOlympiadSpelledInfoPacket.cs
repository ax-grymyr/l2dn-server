using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExOlympiadSpelledInfoPacket: IOutgoingPacket
{
    private readonly int _playerId;
    private readonly List<BuffInfo> _effects;
    private readonly List<Skill> _effects2;
	
    public ExOlympiadSpelledInfoPacket(Player player)
    {
        _effects = new List<BuffInfo>();
        _effects2 = new List<Skill>();
        _playerId = player.ObjectId;
    }
	
    public void addSkill(BuffInfo info)
    {
        _effects.Add(info);
    }
	
    public void addSkill(Skill skill)
    {
        _effects2.Add(skill);
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_OLYMPIAD_SPELLED_INFO);
        
        writer.WriteInt32(_playerId);
        writer.WriteInt32(_effects.Count + _effects2.Count);
        foreach (BuffInfo info in _effects)
        {
            if (info != null && info.isInUse())
            {
                writer.WriteInt32(info.getSkill().DisplayId);
                writer.WriteInt16((short)info.getSkill().DisplayLevel);
                writer.WriteInt16(0); // Sub level
                writer.WriteInt32((int)info.getSkill().AbnormalType);
                writer.WriteVariableInt(info.getSkill().IsAura
                    ? -1
                    : (int)(info.getTime() ?? TimeSpan.Zero).TotalSeconds);
            }
        }
        
        foreach (Skill skill in _effects2)
        {
            if (skill != null)
            {
                writer.WriteInt32(skill.DisplayId);
                writer.WriteInt16((short)skill.DisplayLevel);
                writer.WriteInt16(0); // Sub level
                writer.WriteInt32((int)skill.AbnormalType);
                writer.WriteInt16(-1);
            }
        }
    }
}