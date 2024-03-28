using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Enchanting;

public readonly struct ExChangedEnchantTargetItemProbListPacket: IOutgoingPacket
{
    private readonly List<EnchantProbInfo> _probList;
	
    public ExChangedEnchantTargetItemProbListPacket(List<EnchantProbInfo> probList)
    {
        _probList = probList;
    }
	
    public ExChangedEnchantTargetItemProbListPacket(EnchantProbInfo probInfo)
    {
        _probList = [probInfo];
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CHANGED_ENCHANT_TARGET_ITEM_PROB_LIST);
        
        writer.WriteInt32(_probList.Count); // vProbList;
        foreach (EnchantProbInfo info in _probList)
        {
            writer.WriteInt32(info.itemObjId); // nItemServerId;
            writer.WriteInt32(info.totalSuccessProb);// nTotalSuccessProbPermyriad;
            writer.WriteInt32(info.baseProb);// nBaseProbPermyriad;
            writer.WriteInt32(info.supportProb);// nSupportProbPermyriad;
            writer.WriteInt32(info.itemSkillProb);// nItemSkillProbPermyriad;
        }
    }
	
    public class EnchantProbInfo
    {
        public int itemObjId;
        public int totalSuccessProb;
        public int baseProb;
        public int supportProb;
        public int itemSkillProb;
		
        public EnchantProbInfo(int itemObjId, int totalSuccessProb, int baseProb, int supportProb, int itemSkillProb)
        {
            this.itemObjId = itemObjId;
            this.totalSuccessProb = Math.Min(10000, totalSuccessProb);
            this.baseProb = baseProb;
            this.supportProb = supportProb;
            this.itemSkillProb = itemSkillProb;
        }
    }
}