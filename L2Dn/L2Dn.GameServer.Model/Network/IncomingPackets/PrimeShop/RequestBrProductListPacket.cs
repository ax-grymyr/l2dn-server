using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.PrimeShop;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.PrimeShop;

public struct RequestBrProductListPacket: IIncomingPacket<GameSession>
{
    private int _type;

    public void ReadContent(PacketBitReader reader)
    {
        _type = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        switch (_type)
        {
            case 0: // Home page
            {
                player.sendPacket(new ExBRProductListPacket(player, 0, PrimeShopData.getInstance().getPrimeItems().values()));
                break;
            }
            case 1: // History
            {
                break;
            }
            case 2: // Favorites
            {
                break;
            }
            default:
            {
                PacketLogger.Instance.Warn(player + " send unhandled product list type: " + _type);
                break;
            }
        }
        
        return ValueTask.CompletedTask;
    }
}