using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Settings;

public struct ExInteractModifyPacket: IIncomingPacket<GameSession>
{
    private int _type;
    private int _settings;

    public void ReadContent(PacketBitReader reader)
    {
        _type = reader.ReadByte();
        _settings = reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        ClientSettings clientSettings = player.getClientSettings();
        switch (_type)
        {
            case 0:
            {
                clientSettings.setPartyRequestRestrictedFromOthers((_settings & 1) == 1);
                clientSettings.setPartyRequestRestrictedFromClan((_settings & 2) == 2);
                clientSettings.setPartyRequestRestrictedFromFriends((_settings & 4) == 4);
                clientSettings.storeSettings();
                break;
            }
            case 1:
            {
                clientSettings.setFriendRequestRestrictedFromOthers((_settings & 1) == 1);
                clientSettings.setFriendRequestRestrictionFromClan((_settings & 2) == 2);
                clientSettings.storeSettings();
                break;
            }
        }
        
        return ValueTask.CompletedTask;
    }
}