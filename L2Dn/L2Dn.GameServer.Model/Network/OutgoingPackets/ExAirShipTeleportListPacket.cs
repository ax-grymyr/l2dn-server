using L2Dn.GameServer.Model;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExAirShipTeleportListPacket: IOutgoingPacket
{
    private readonly int _dockId;
    private readonly VehiclePathPoint[][] _teleports;
    private readonly int[] _fuelConsumption;

    public ExAirShipTeleportListPacket(int dockId, VehiclePathPoint[][] teleports, int[] fuelConsumption)
    {
        _dockId = dockId;
        _teleports = teleports;
        _fuelConsumption = fuelConsumption;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_AIR_SHIP_TELEPORT_LIST);

        writer.WriteInt32(_dockId);
        if (_teleports != null)
        {
            writer.WriteInt32(_teleports.Length);
            for (int i = 0; i < _teleports.Length; i++)
            {
                writer.WriteInt32(i - 1);
                writer.WriteInt32(_fuelConsumption[i]);
                VehiclePathPoint[] path = _teleports[i];
                VehiclePathPoint dst = path[^1];
                writer.WriteLocation3D(dst.Location);
            }
        }
        else
        {
            writer.WriteInt32(0);
        }
    }
}