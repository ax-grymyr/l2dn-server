using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct AllianceInfoPacket: IOutgoingPacket
{
    private readonly string _name;
    private readonly int _total;
    private readonly int _online;
    private readonly string _leaderC;
    private readonly string _leaderP;
    private readonly ClanInfo[] _allies;
	
    public AllianceInfoPacket(int allianceId)
    {
        Clan leader = ClanTable.getInstance().getClan(allianceId);
        _name = leader.getAllyName() ?? string.Empty;
        _leaderC = leader.getName();
        _leaderP = leader.getLeaderName();
        ICollection<Clan> allies = ClanTable.getInstance().getClanAllies(allianceId);
        _allies = new ClanInfo[allies.Count];
        int idx = 0;
        int total = 0;
        int online = 0;
        foreach (Clan clan in allies)
        {
            ClanInfo ci = new ClanInfo(clan);
            _allies[idx++] = ci;
            total += ci.getTotal();
            online += ci.getOnline();
        }
        
        _total = total;
        _online = online;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.ALLIANCE_INFO);
        
        writer.WriteString(_name);
        writer.WriteInt32(_total);
        writer.WriteInt32(_online);
        writer.WriteString(_leaderC);
        writer.WriteString(_leaderP);
        writer.WriteInt32(_allies.Length);
        foreach (ClanInfo aci in _allies)
        {
            writer.WriteString(aci.getClan().getName());
            writer.WriteInt32(0);
            writer.WriteInt32(aci.getClan().getLevel());
            writer.WriteString(aci.getClan().getLeaderName());
            writer.WriteInt32(aci.getTotal());
            writer.WriteInt32(aci.getOnline());
        }
    }
	
    public string getName()
    {
        return _name;
    }
	
    public int getTotal()
    {
        return _total;
    }
	
    public int getOnline()
    {
        return _online;
    }
	
    public string getLeaderC()
    {
        return _leaderC;
    }
	
    public string getLeaderP()
    {
        return _leaderP;
    }
	
    public ClanInfo[] getAllies()
    {
        return _allies;
    }
}