using L2Dn.GameServer.Model;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

internal struct SendBypassBuildCmdPacket: IIncomingPacket<GameSession>
{
    private string? _command;

    public void ReadContent(PacketBitReader reader)
    {
        _command = reader.ReadString();
    }

    public async ValueTask ProcessAsync(Connection<GameSession> connection)
    {
        if (string.IsNullOrWhiteSpace(_command))
            return;

        string command = _command.Trim();

        // hack for now
        if (command.StartsWith("cur"))
        {
            
        }
        else if (command.StartsWith("loc "))
        {
            string locationName = command.Substring(4).ToLower();
            Location location = locationName switch
            {
                "aden" => new(146856, 25803, -2008),
                "darkelf" => new(9800, 15596, -4568),
                "dion" => new(15689, 142953, -2696),
                "dwarf" => new(115190, -178176, -896),
                "elf" => new(46916, 51420, -2976),
                "giran" => new(83386, 148014, -3400),
                "gludin" => new(-80684, 149770, -3040),
                "gludio" => new(-14424, 123972, -3120),
                "goddard" => new(147961, -55309, -2728),
                "hunters" => new(117051, 76854, -2704),
                "orc" => new(-45158, -112583, -240),
                "oren" => new(82927, 53255, -1488),
                "talking" => new(-84191, 244531, -3728),
                _ => new()
            };

            Character? character = connection.Session.SelectedCharacter;
            if (location != default && character is not null)
            {
                int objectId = connection.Session.ObjectId;
                // Location oldLocation = connection.Session.Location;
                //
                // StopMovePacket stopMovePacket = new(objectId, oldLocation, 0);
                //
                // connection.Send(ref stopMovePacket);
                //
                // MagicSkillUsePacket magicSkillUsePacket = new(objectId, oldLocation);
                // connection.Send(ref magicSkillUsePacket);
                //
                // SetupGaugePacket setupGaugePacket = new(objectId, 0, 1000, 1000);
                // connection.Send(ref setupGaugePacket);
                //
                // MagicSkillLaunchedPacket magicSkillLaunchedPacket = new(objectId);
                // connection.Send(ref magicSkillLaunchedPacket);
                //
                // await Task.Delay(1000);
                //
                // ActionFailedPacket actionFailedPacket = new(0);
                // connection.Send(ref actionFailedPacket);
                //
                // actionFailedPacket = new(1);
                // connection.Send(ref actionFailedPacket);
                //
                // MagicSkillCancelledPacket magicSkillCancelledPacket = new(objectId);
                // connection.Send(ref magicSkillCancelledPacket);
                //
                // actionFailedPacket = new(0);
                // connection.Send(ref actionFailedPacket);

                character.LocationX = location.X;
                character.LocationY = location.Y;
                character.LocationZ = location.Z;
                connection.Session.Location = location;

                TeleportToLocationPacket teleportToLocationPacket = new(objectId, location);
                connection.Send(ref teleportToLocationPacket);

                TeleportToLocationActivatePacket teleportToLocationActivatePacket = new(objectId, location);
                connection.Send(ref teleportToLocationActivatePacket);

                // UserInfoPacket userInfoPacket = new(objectId, character);
                // connection.Send(ref userInfoPacket);
            }
        }
    }
}
