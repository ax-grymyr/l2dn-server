using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExEnchantSkillInfoPacket: IOutgoingPacket
{
    private readonly Set<int> _routes;
    private readonly int _skillId;
    private readonly int _skillLevel;
    private readonly int _skillSubLevel;
    private readonly int _currentSubLevel;
	
    public ExEnchantSkillInfoPacket(int skillId, int skillLevel, int skillSubLevel, int currentSubLevel)
    {
        _skillId = skillId;
        _skillLevel = skillLevel;
        _skillSubLevel = skillSubLevel;
        _currentSubLevel = currentSubLevel;
        _routes = EnchantSkillGroupsData.getInstance().getRouteForSkill(_skillId, _skillLevel);
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ENCHANT_SKILL_INFO);
        
        writer.WriteInt32(_skillId);
        writer.WriteInt16((short)_skillLevel);
        writer.WriteInt16((short)_skillSubLevel);
        writer.WriteInt32((_skillSubLevel % 1000) != EnchantSkillGroupsData.MAX_ENCHANT_LEVEL);
        writer.WriteInt32(_skillSubLevel > 1000);
        writer.WriteInt32(_routes.size());
        foreach (int route in _routes)
        {
            int routeId = route / 1000;
            int currentRouteId = _skillSubLevel / 1000;
            int subLevel = _currentSubLevel > 0 ? (route + (_currentSubLevel % 1000)) - 1 : route;
            writer.WriteInt16((short)_skillLevel);
            writer.WriteInt16((short)(currentRouteId != routeId ? subLevel : Math.Min(subLevel + 1, route + (EnchantSkillGroupsData.MAX_ENCHANT_LEVEL - 1))));
        }
    }
}