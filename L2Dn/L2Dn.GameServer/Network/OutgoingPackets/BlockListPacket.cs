using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct BlockListPacket: IOutgoingPacket
{
	private readonly Set<int> _playerIds;
	
	public BlockListPacket(Set<int> playerIds)
	{
		_playerIds = playerIds;
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.BLOCK_LIST);
		writer.WriteInt32(_playerIds.size());
		foreach (int playerId in _playerIds)
		{
			writer.WriteString(CharInfoTable.getInstance().getNameById(playerId)); // TODO: slow!!!
			writer.WriteString(""); // memo ?
		}
	}
}