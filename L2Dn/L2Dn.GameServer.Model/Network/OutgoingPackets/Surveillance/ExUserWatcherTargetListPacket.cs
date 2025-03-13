using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Surveillance;

public readonly struct ExUserWatcherTargetListPacket: IOutgoingPacket
{
    private readonly List<TargetInfo> _info;

    public ExUserWatcherTargetListPacket(Player player)
    {
        _info = [];
        foreach (int objId in player.getSurveillanceList())
        {
            string name = CharInfoTable.getInstance().getNameById(objId) ?? string.Empty;
            Player? target = World.getInstance().getPlayer(objId);
            bool online = false;
            int level;
            CharacterClass classId;
            if (target != null)
            {
                online = true;
                level = target.getLevel();
                classId = target.getClassId();
            }
            else
            {
                level = CharInfoTable.getInstance().getLevelById(objId);
                classId = CharInfoTable.getInstance().getClassIdById(objId);
            }

            _info.Add(new TargetInfo(name, online, level, classId));
        }
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_USER_WATCHER_TARGET_LIST);
        writer.WriteInt32(_info.Count);
        foreach (TargetInfo info in _info)
        {
            writer.WriteSizedString(info.Name);
            writer.WriteInt32(0); // client.getProxyServerId()
            writer.WriteInt32(info.Level);
            writer.WriteInt32((int)info.Class);
            writer.WriteByte(info.Online);
        }
    }

    private record TargetInfo(string Name, bool Online, int Level, CharacterClass Class);
}