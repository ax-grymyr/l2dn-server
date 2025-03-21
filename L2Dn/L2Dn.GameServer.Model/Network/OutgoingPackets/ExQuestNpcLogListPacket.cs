using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExQuestNpcLogListPacket: IOutgoingPacket
{
    private readonly int _questId;
    private readonly List<NpcLogListHolder> _npcLogList;
	
    public ExQuestNpcLogListPacket(int questId)
    {
        _questId = questId;
        _npcLogList = new List<NpcLogListHolder>();
    }
	
    public void addNpc(int npcId, int count)
    {
        _npcLogList.Add(new NpcLogListHolder((NpcStringId)npcId, false, count));
    }
	
    public void addNpcString(NpcStringId npcStringId, int count)
    {
        _npcLogList.Add(new NpcLogListHolder(npcStringId, true, count));
    }
	
    public void add(NpcLogListHolder holder)
    {
        _npcLogList.Add(holder);
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_QUEST_NPC_LOG_LIST);
        
        writer.WriteInt32(_questId);
        writer.WriteByte((byte)_npcLogList.Count);
        foreach (NpcLogListHolder holder in _npcLogList)
        {
            writer.WriteInt32((int)(holder.isNpcString() ? holder.getId() : holder.getId() + 1000000));
            writer.WriteByte(holder.isNpcString());
            writer.WriteInt32(holder.getCount());
        }
    }
}