using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExMagicSkillUseGroundPacket: IIncomingPacket<GameSession>
{
    private Location3D _location;
    private int _skillId;
    private bool _ctrlPressed;
    private bool _shiftPressed;

    public void ReadContent(PacketBitReader reader)
    {
        _location = reader.ReadLocation3D();
        _skillId = reader.ReadInt32();
        _ctrlPressed = reader.ReadInt32() != 0;
        _shiftPressed = reader.ReadByte() != 0;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        // Get the level of the used skill
        int level = player.getSkillLevel(_skillId);
        if (level <= 0)
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        // Get the Skill template corresponding to the skillID received from the client
        Skill? skill = SkillData.Instance.GetSkill(_skillId, level);

        // Check the validity of the skill
        if (skill != null)
        {
            player.setCurrentSkillWorldPosition(_location);

            // normally MagicSkillUsePacket turns char client side but for these skills, it doesn't (even with correct target)
            player.setHeading(new Location2D(player.getX(), player.getY()).HeadingTo(_location));
            Broadcast.toKnownPlayers(player, new ValidateLocationPacket(player));
            player.useMagic(skill, null, _ctrlPressed, _shiftPressed);
        }
        else
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            PacketLogger.Instance.Warn("No skill found with id " + _skillId + " and level " + level + " !!");
        }

        return ValueTask.CompletedTask;
    }
}