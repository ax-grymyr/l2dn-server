using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.CommunityBbs.Managers;

public abstract class BaseBBSManager
{
    public abstract void ParseCmd(string command, Player player);

    public abstract void ParseWrite(string ar1, string ar2, string ar3, string ar4, string ar5, Player player);

    protected void Send1001(string html, Player acha)
    {
        if (html.Length < 8192)
        {
            acha.sendPacket(new ShowBoardPacket(html, "1001"));
        }
    }

    protected void Send1002(Player acha)
    {
        Send1002(acha, " ", " ", "0");
    }

    protected void Send1002(Player player, string str, string string2, string string3)
    {
        string[] args =
        [
            "0", "0", "0", "0", "0", "0", player.getName(), player.ObjectId.ToString(), player.getAccountName(), "9",
            string2, // subject?
            string2, // subject?
            str, // text
            string3, // date?
            string3, // date?
            "0", "0"
        ];

        player.sendPacket(new ShowBoardPacket(args));
    }
}