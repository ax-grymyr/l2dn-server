using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Teleports;

public struct ExRequestTeleportFavoritesAddDelPacket: IIncomingPacket<GameSession>
{
    private bool _enable;
    private int _teleportId;

    public void ReadContent(PacketBitReader reader)
    {
        _enable = reader.ReadByte() == 1;
        _teleportId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (TeleportData.Instance.GetTeleport(_teleportId) == null)
        {
            PacketLogger.Instance.Warn("No registered teleport location for id: " + _teleportId);
            return ValueTask.CompletedTask;
        }

        List<int>? favoriteTeleports = player.getVariables().Get<List<int>>(PlayerVariables.FAVORITE_TELEPORTS);

        List<int> favorites = new();
        if (favoriteTeleports != null)
        {
            foreach (int id in favoriteTeleports)
            {
                if (TeleportData.Instance.GetTeleport(_teleportId) == null)
                    PacketLogger.Instance.Warn("No registered teleport location for id: " + _teleportId);
                else
                    favorites.Add(id);
            }
        }

        if (_enable)
        {
            if (!favorites.Contains(_teleportId))
                favorites.Add(_teleportId);
        }
        else
        {
            favorites.Remove(_teleportId);
        }

        player.getVariables().Set(PlayerVariables.FAVORITE_TELEPORTS, favorites);

        return ValueTask.CompletedTask;
    }
}