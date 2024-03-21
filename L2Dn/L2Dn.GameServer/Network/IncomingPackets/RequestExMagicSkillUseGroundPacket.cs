using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExMagicSkillUseGroundPacket: IIncomingPacket<GameSession>
{
    private int _x;
    private int _y;
    private int _z;
    private int _skillId;
    private bool _ctrlPressed;
    private bool _shiftPressed;

    public void ReadContent(PacketBitReader reader)
    {
        _x = reader.ReadInt32();
        _y = reader.ReadInt32();
        _z = reader.ReadInt32();
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
        Skill skill = SkillData.getInstance().getSkill(_skillId, level);
		
        // Check the validity of the skill
        if (skill != null)
        {
            player.setCurrentSkillWorldPosition(new Location(_x, _y, _z));
			
            // normally magicskilluse packet turns char client side but for these skills, it doesn't (even with correct target)
            player.setHeading(Util.calculateHeadingFrom(player.getX(), player.getY(), _x, _y));
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