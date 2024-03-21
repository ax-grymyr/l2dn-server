using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct ExBookmarkPacket: IIncomingPacket<GameSession>
{
    private int _bookmarkType;
    private int _id;
    private int _icon;
    private string _name;
    private string _tag;
    
    public void ReadContent(PacketBitReader reader)
    {
        _bookmarkType = reader.ReadInt32();
        switch (_bookmarkType)
        {
            case 1:
                _name = reader.ReadString();
                _icon = reader.ReadInt32();
                _tag = reader.ReadString();
                break;
            
            case 2:
                _id = reader.ReadInt32();
                _name = reader.ReadString();
                _icon = reader.ReadInt32();
                _tag = reader.ReadString();
                break;

            case 3:
            case 4:
                _id = reader.ReadInt32();
                break;
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
       
        switch (_bookmarkType)
        {
            case 0:
                player.sendPacket(new ExGetBookMarkInfoPacket(player));
                break;
            
            case 1:
                if (player.isInsideZone(ZoneId.TIMED_HUNTING))
                {
                    player.sendMessage("You cannot bookmark this location.");
                    return ValueTask.CompletedTask;
                }
		
                player.teleportBookmarkAdd(player.getX(), player.getY(), player.getZ(), _icon, _tag, _name);
                break;
            
            case 2:
                player.teleportBookmarkModify(_id, _icon, _tag, _name);
                break;

            case 3:
                player.teleportBookmarkDelete(_id);
                break;
            
            case 4:            
                player.teleportBookmarkGo(_id);
                break;
            
            default:
                PacketLogger.Instance.Info($"Unknown bookmark action type: {_bookmarkType}");
                break;
        }

        return ValueTask.CompletedTask;
    }
}