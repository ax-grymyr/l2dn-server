using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestSetAllyCrestPacket: IIncomingPacket<GameSession>
{
    private int _length;
    private byte[] _data;

    public void ReadContent(PacketBitReader reader)
    {
        _length = reader.ReadInt32();
        if (_length > 192 || _length < 0)
            return;

        _data = reader.ReadBytes(_length).ToArray();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_length < 0)
        {
            player.sendMessage("File transfer error.");
            return ValueTask.CompletedTask;
        }

        if (_length > 192)
        {
            player.sendPacket(SystemMessageId.PLEASE_ADJUST_THE_IMAGE_SIZE_TO_8X12);
            return ValueTask.CompletedTask;
        }

        int? allyId = player.getAllyId();
        if (allyId is null)
        {
            player.sendPacket(SystemMessageId.ACCESS_ONLY_FOR_THE_CHANNEL_FOUNDER);
            return ValueTask.CompletedTask;
        }

        Clan? leaderClan = ClanTable.getInstance().getClan(allyId.Value);
        if (leaderClan == null || player.getClanId() != leaderClan.getId() || !player.isClanLeader())
        {
            player.sendPacket(SystemMessageId.ACCESS_ONLY_FOR_THE_CHANNEL_FOUNDER);
            return ValueTask.CompletedTask;
        }

        if (_length == 0)
        {
            if (leaderClan.getAllyCrestId() != 0)
            {
                leaderClan.changeAllyCrest(0, false);
            }
        }
        else
        {
            Crest? crest = CrestTable.getInstance().createCrest(_data, CrestType.ALLY);
            if (crest != null)
            {
                leaderClan.changeAllyCrest(crest.getId(), false);
                player.sendPacket(SystemMessageId.THE_CREST_WAS_SUCCESSFULLY_REGISTERED);
            }
        }

        return ValueTask.CompletedTask;
    }
}