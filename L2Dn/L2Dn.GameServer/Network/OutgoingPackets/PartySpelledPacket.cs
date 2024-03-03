using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PartySpelledPacket: IOutgoingPacket 
{
    private readonly List<BuffInfo> _effects;
    private readonly List<Skill> _effects2;
    private readonly Creature _creature;
	
    public PartySpelledPacket(Creature creature)
    {
        _effects = new List<BuffInfo>();
        _effects2 = new List<Skill>();
        _creature = creature;
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
        writer.WritePacketCode(OutgoingPacketCodes.PARTY_SPELLED);
        
        writer.WriteInt32(_creature.isServitor() ? 2 : _creature.isPet() ? 1 : 0);
        writer.WriteInt32(_creature.getObjectId());
        writer.WriteInt32(_effects.Count + _effects2.Count);
        foreach (BuffInfo info in _effects)
        {
            if ((info != null) && info.isInUse())
            {
                writer.WriteInt32(info.getSkill().getDisplayId());
                writer.WriteInt16((short)info.getSkill().getDisplayLevel());
                writer.WriteInt16(0); // Sub level
                writer.WriteInt32((int)info.getSkill().getAbnormalType());
                writer.WriteVariableInt((int)(info.getTime() ?? TimeSpan.Zero).TotalSeconds);
            }
        }
        foreach (Skill skill in _effects2)
        {
            if (skill != null)
            {
                writer.WriteInt32(skill.getDisplayId());
                writer.WriteInt16((short)skill.getDisplayLevel());
                writer.WriteInt16(0); // Sub level
                writer.WriteInt32((int)skill.getAbnormalType());
                writer.WriteInt16(-1);
            }
        }
    }
}