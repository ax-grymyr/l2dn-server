using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Friends;

/**
 * Support for "Chat with Friends" dialog. <br />
 * Add new friend or delete.
 * @author JIV
 */
public readonly struct L2FriendPacket: IOutgoingPacket
{
	private readonly bool _action;
	private readonly bool _online;
	private readonly int _objid;
	private readonly string _name;

	/**
	 * @param action - true for adding, false for remove
	 * @param objId
	 */
	public L2FriendPacket(bool action, int objId)
	{
		_action = action;
		_objid = objId;
		_name = CharInfoTable.getInstance().getNameById(objId) ?? string.Empty;
		_online = World.getInstance().getPlayer(objId) != null;
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.L2_FRIEND);

		writer.WriteInt32(_action ? 1 : 3); // 1-add 3-remove
		writer.WriteInt32(_objid);
		writer.WriteString(_name);
		writer.WriteInt32(_online);
		writer.WriteInt32(_online ? _objid : 0);
	}
}