using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.UserCommandHandlers;

/**
 * Loc user command.
 */
public class Loc: IUserCommandHandler
{
    private static readonly int[] COMMAND_IDS = [0];

    public bool useUserCommand(int id, Player player)
    {
        RespawnZone? zone = ZoneManager.Instance.getZone<RespawnZone>(player.Location.Location3D);
        MapRegion? restartRegion = null;
        if (zone != null)
        {
            Map<Race, string> respawnPoints = zone.getAllRespawnPoints();
            string? respawnPoint = respawnPoints.get(player.getRace()) ?? respawnPoints.get(Race.HUMAN);
            if (respawnPoint != null)
                restartRegion = MapRegionManager.GetRestartRegion(player, respawnPoint);
        }

        SystemMessageId systemMessageId = restartRegion != null
            ? (SystemMessageId)restartRegion.LocationId
            : (SystemMessageId)MapRegionData.Instance.GetMapRegionLocationId(player);

        SystemMessagePacket sm;
        if (systemMessageId > 0 && systemMessageId.GetParamCount() == 3)
        {
            sm = new SystemMessagePacket(systemMessageId);
            sm.Params.addInt(player.getX());
            sm.Params.addInt(player.getY());
            sm.Params.addInt(player.getZ());
        }
        else
        {
            sm = new SystemMessagePacket(SystemMessageId.CURRENT_LOCATION_S1);
            sm.Params.addString($"{player.getX()}, {player.getY()}, {player.getZ()}");
        }

        player.sendPacket(sm);
        return true;
    }

    public int[] getUserCommandList()
    {
        return COMMAND_IDS;
    }
}