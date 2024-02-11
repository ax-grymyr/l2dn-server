using System.Text;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct ShowBoardPacket(bool showBoard, string content): IOutgoingPacket
{
    public ShowBoardPacket(string content, string id): this(id + '\b' + content)
    {
    }
    
    public ShowBoardPacket(string content): this(true, content)
    {
    }
    
    public ShowBoardPacket(ReadOnlySpan<string> args): this(BuildContent(args))
    {
    }
    
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SHOW_BOARD);
        writer.WriteBoolean(showBoard); // c4 1 to show community 00 to hide
        writer.WriteString("bypass _bbshome"); // top
        writer.WriteString("bypass _bbsgetfav"); // favorite
        writer.WriteString("bypass _bbsloc"); // region
        writer.WriteString("bypass _bbsclan"); // clan
        writer.WriteString("bypass _bbsmemo"); // memo
        writer.WriteString("bypass _bbsmail"); // mail
        writer.WriteString("bypass _bbsfriends"); // friends
        writer.WriteString("bypass bbs_add_fav"); // add fav.
        writer.WriteString(content);
    }

    private static string BuildContent(ReadOnlySpan<string> args)
    {
        StringBuilder builder = new();
        builder.Append("1002\b");
        foreach (string str in args)
            builder.Append(str).Append('\b');
        
        return builder.ToString();
    }
}