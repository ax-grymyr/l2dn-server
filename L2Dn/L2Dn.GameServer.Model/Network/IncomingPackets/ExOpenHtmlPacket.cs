using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct ExOpenHtmlPacket: IIncomingPacket<GameSession>
{
    private int _type;

    public void ReadContent(PacketBitReader reader)
    {
        _type = reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        switch (_type)
        {
            case 1:
            {
                if (Config.PC_CAFE_ENABLED)
                {
                    HtmlContent htmlContent = HtmlContent.LoadFromFile("html/pccafe.htm", player);
                    NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 0, htmlContent);
                    connection.Send(html);
                }

                break;
            }
            case 5:
            {
                if (Config.GAME_ASSISTANT_ENABLED)
                {
                    HtmlContent htmlContent = HtmlContent.LoadFromFile("scripts/ai/others/GameAssistant/32478.html", player);
                    connection.Send(new ExPremiumManagerShowHtmlPacket(htmlContent));
                }

                break;
            }
            // case 7:
            // {
            // if (Config.EINHASAD_STORE_ENABLED)
            // {
            // client.sendPacket(new ExPremiumManagerShowHtml(HtmCache.getInstance().getHtm(player, "data/scripts/ai/others/EinhasadStore/34487.html")));
            // }
            // break;
            // }
            default:
            {
                PacketLogger.Instance.Warn("Unknown ExOpenHtml type (" + _type + ")");
                break;
            }
        }

        return ValueTask.CompletedTask;
    }
}