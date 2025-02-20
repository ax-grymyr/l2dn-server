using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExEnchantSkillInfoDetailPacket: IIncomingPacket<GameSession>
{
    private SkillEnchantType _type;
    private int _skillId;
    private int _skillLevel;
    private int _skillSubLevel;

    public void ReadContent(PacketBitReader reader)
    {
        _type = (SkillEnchantType)reader.ReadInt32();
        _skillId = reader.ReadInt32();
        _skillLevel = reader.ReadInt16();
        _skillSubLevel = reader.ReadInt16();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (_skillId <= 0 || _skillLevel <= 0 || _skillSubLevel < 0)
            return ValueTask.CompletedTask;

        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new ExEnchantSkillInfoDetailPacket(_type, _skillId, _skillLevel, _skillSubLevel));

        return ValueTask.CompletedTask;
    }
}