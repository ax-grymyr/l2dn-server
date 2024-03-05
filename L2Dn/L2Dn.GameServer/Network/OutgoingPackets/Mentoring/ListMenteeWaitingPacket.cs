using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Mentoring;

public readonly struct ListMenteeWaitingPacket: IOutgoingPacket
{
    private const int PLAYERS_PER_PAGE = 64;
	
    private readonly List<Player> _possibleCandiates;
    private readonly int _page;
	
    public ListMenteeWaitingPacket(int page, int minLevel, int maxLevel)
    {
        _page = page;
        _possibleCandiates = new List<Player>();
        // for (Player player : World.getInstance().getPlayers())
        // {
        // if ((player.getLevel() >= minLevel) && (player.getLevel() <= maxLevel) && !player.isMentee() && !player.isMentor() && !player.isInCategory(CategoryType.SIXTH_CLASS_GROUP))
        // {
        // _possibleCandiates.add(player);
        // }
        // }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.LIST_MENTEE_WAITING);
        
        writer.WriteInt32(1); // always 1 in retail
        if (_possibleCandiates.Count == 0)
        {
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            return;
        }
		
        writer.WriteInt32(_possibleCandiates.Count);
        writer.WriteInt32(_possibleCandiates.Count % PLAYERS_PER_PAGE);
        foreach (Player player in _possibleCandiates)
        {
            if ((1 <= (PLAYERS_PER_PAGE * _page)) && (1 > (PLAYERS_PER_PAGE * (_page - 1))))
            {
                writer.WriteString(player.getName());
                writer.WriteInt32((int)player.getActiveClass());
                writer.WriteInt32(player.getLevel());
            }
        }
    }
}