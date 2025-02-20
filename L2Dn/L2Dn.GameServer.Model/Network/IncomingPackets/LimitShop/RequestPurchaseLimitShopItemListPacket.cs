using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.OutgoingPackets.LimitShop;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.LimitShop;

public struct RequestPurchaseLimitShopItemListPacket: IIncomingPacket<GameSession>
{
    private const int MAX_PAGE_SIZE = 350;
    private int _shopType;
    
    public void ReadContent(PacketBitReader reader)
    {
        _shopType = reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        List<LimitShopProductHolder> products;
        switch (_shopType)
        {
            case 3: // Normal Lcoin Shop
            {
                products = LimitShopData.getInstance().getProducts();
                break;
            }
            case 4: // Lcoin Special Craft
            {
                products = LimitShopCraftData.getInstance().getProducts();
                break;
            }
            case 100: // Clan Shop
            {
                products = LimitShopClanData.getInstance().getProducts();
                break;
            }
            default:
            {
                return ValueTask.CompletedTask;
            }
        }
		
        // Calculate the number of pages.
        int totalPages = products.Count / MAX_PAGE_SIZE + (products.Count % MAX_PAGE_SIZE == 0 ? 0 : 1);
		
        // Iterate over pages.
        for (int page = 0; page < totalPages; page++)
        {
            // Calculate start and end indices for each page.
            int start = page * MAX_PAGE_SIZE;
            int end = Math.Min(start + MAX_PAGE_SIZE, products.Count);
			
            // Get the subList for current page.
            List<LimitShopProductHolder> productList = products.Slice(start, end - start);
			
            // Send the packet.
            ExPurchaseLimitShopItemListNewPacket packet = new(player, _shopType, page + 1, totalPages, productList);
            connection.Send(ref packet);
        }

        return ValueTask.CompletedTask;
    }
}