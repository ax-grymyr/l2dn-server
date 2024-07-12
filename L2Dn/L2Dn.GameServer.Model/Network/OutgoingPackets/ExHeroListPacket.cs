using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExHeroListPacket: IOutgoingPacket
{
    private readonly Map<int, StatSet> _heroList;
	
    public ExHeroListPacket()
    {
        _heroList = Hero.getInstance().getHeroes();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_HERO_LIST);
        
        writer.WriteInt32(_heroList.Count);
        foreach (StatSet hero in _heroList.Values)
        {
            writer.WriteString(hero.getString(Olympiad.CHAR_NAME));
            writer.WriteInt32(hero.getInt(Olympiad.CLASS_ID));
            writer.WriteString(hero.getString(Hero.CLAN_NAME, ""));
            writer.WriteInt32(0); // hero.getInt(Hero.CLAN_CREST, 0)
            writer.WriteString(hero.getString(Hero.ALLY_NAME, ""));
            writer.WriteInt32(0); // hero.getInt(Hero.ALLY_CREST, 0)
            writer.WriteInt32(hero.getInt(Hero.COUNT));
            writer.WriteInt32(Config.SERVER_ID);
            writer.WriteByte(0); // 272
        }
    }
}