using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Appearance;

public readonly struct ExCuriousHouseMemberUpdatePacket: IOutgoingPacket
{
    public readonly int _objId;
    public readonly int _maxHp;
    public readonly int _maxCp;
    public readonly int _currentHp;
    public readonly int _currentCp;

    public ExCuriousHouseMemberUpdatePacket(Player player)
    {
        _objId = player.getObjectId();
        _maxHp = player.getMaxHp();
        _maxCp = player.getMaxCp();
        _currentHp = (int)player.getCurrentHp();
        _currentCp = (int)player.getCurrentCp();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CURIOUS_HOUSE_MEMBER_UPDATE);
        
        writer.WriteInt32(_objId);
        writer.WriteInt32(_maxHp);
        writer.WriteInt32(_maxCp);
        writer.WriteInt32(_currentHp);
        writer.WriteInt32(_currentCp);
    }
}