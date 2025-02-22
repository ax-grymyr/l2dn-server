﻿using L2Dn.GameServer.AI;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct MoveToLocationAirShipPacket: IIncomingPacket<GameSession>
{
    private const int _stepZ = 300;

    private int _command;
    private int _param1;
    private int _param2;

    public void ReadContent(PacketBitReader reader)
    {
        _command = reader.ReadInt32();
        _param1 = reader.ReadInt32();
        if (reader.Length >= 4)
        {
            _param2 = reader.ReadInt32();
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        AirShip? ship = player.getAirShip();
        if (!player.isInAirShip() || ship == null)
            return ValueTask.CompletedTask;

        if (!ship.isCaptain(player))
            return ValueTask.CompletedTask;

        int z = ship.getZ();

        switch (_command)
        {
            case 0:
            {
                if (!ship.canBeControlled())
                    return ValueTask.CompletedTask;

                if (_param1 < WorldMap.GraciaMaxX)
                    ship.getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, new Location3D(_param1, _param2, z));

                break;
            }
            case 1:
            {
                if (!ship.canBeControlled())
                    return ValueTask.CompletedTask;

                ship.getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
                break;
            }
            case 2:
            {
                if (!ship.canBeControlled())
                    return ValueTask.CompletedTask;

                if (z < WorldMap.GraciaMaxZ)
                {
                    z = Math.Min(z + _stepZ, WorldMap.GraciaMaxZ);
                    ship.getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, new Location3D(ship.getX(), ship.getY(), z));
                }

                break;
            }
            case 3:
            {
                if (!ship.canBeControlled())
                    return ValueTask.CompletedTask;

                if (z > WorldMap.GraciaMinZ)
                {
                    z = Math.Max(z - _stepZ, WorldMap.GraciaMinZ);
                    ship.getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, new Location3D(ship.getX(), ship.getY(), z));
                }

                break;
            }
            case 4:
            {
                if (!ship.isInDock() || ship.isMoving())
                    return ValueTask.CompletedTask;

                VehiclePathPoint[]? dst = AirShipManager.getInstance().getTeleportDestination(ship.getDockId(), _param1);
                if (dst == null)
                    return ValueTask.CompletedTask;

                // Consume fuel, if needed
                int fuelConsumption = AirShipManager.getInstance().getFuelConsumption(ship.getDockId(), _param1);
                if (fuelConsumption > 0)
                {
                    if (fuelConsumption > ship.getFuel())
                    {
                        player.sendPacket(SystemMessageId.NOT_ENOUGH_FUEL_FOR_TELEPORTATION);
                        return ValueTask.CompletedTask;
                    }

                    ship.setFuel(ship.getFuel() - fuelConsumption);
                }

                ship.executePath(dst);
                break;
            }
        }

        return ValueTask.CompletedTask;
    }
}