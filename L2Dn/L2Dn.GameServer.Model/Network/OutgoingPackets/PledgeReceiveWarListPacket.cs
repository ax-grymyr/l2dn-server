using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PledgeReceiveWarListPacket: IOutgoingPacket
{
    private readonly Clan _clan;
    private readonly int _tab;
    private readonly ICollection<ClanWar> _clanList;
	
    public PledgeReceiveWarListPacket(Clan clan, int tab)
    {
        _clan = clan;
        _tab = tab;
        _clanList = clan.getWarList().Values;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PLEDGE_RECEIVE_WAR_LIST);
        
        writer.WriteInt32(_tab); // page
        writer.WriteInt32(_clanList.Count);
        foreach (ClanWar clanWar in _clanList)
        {
            Clan clan = clanWar.getOpposingClan(_clan);
            if (clan == null)
            {
                continue;
            }
            
            writer.WriteString(clan.getName());
            writer.WriteInt32((int)clanWar.getState()); // type: 0 = Declaration, 1 = Blood Declaration, 2 = In War, 3 = Victory, 4 = Defeat, 5 = Tie, 6 = Error
            writer.WriteInt32((int)clanWar.getRemainingTime().TotalSeconds); // Time if friends to start remaining
            writer.WriteInt32(clanWar.getKillDifference(_clan)); // Score
            writer.WriteInt32(0); // @TODO: Recent change in points
            writer.WriteInt32(clanWar.getKillToStart()); // Friends to start war left
        }
    }
}