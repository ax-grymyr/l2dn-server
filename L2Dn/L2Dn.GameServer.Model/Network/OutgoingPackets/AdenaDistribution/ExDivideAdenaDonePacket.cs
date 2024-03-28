using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.AdenaDistribution;

/**
 * @author Sdw
 */
public readonly struct ExDivideAdenaDonePacket: IOutgoingPacket
{
	private readonly bool _isPartyLeader;
	private readonly bool _isCCLeader;
	private readonly long _adenaCount;
	private readonly long _distributedAdenaCount;
	private readonly int _memberCount;
	private readonly string _distributorName;

	public ExDivideAdenaDonePacket(bool isPartyLeader, bool isCCLeader, long adenaCount, long distributedAdenaCount,
		int memberCount, string distributorName)
	{
		_isPartyLeader = isPartyLeader;
		_isCCLeader = isCCLeader;
		_adenaCount = adenaCount;
		_distributedAdenaCount = distributedAdenaCount;
		_memberCount = memberCount;
		_distributorName = distributorName;
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_DIVIDE_ADENA_DONE);

		writer.WriteByte(_isPartyLeader);
		writer.WriteByte(_isCCLeader);
		writer.WriteInt32(_memberCount);
		writer.WriteInt64(_distributedAdenaCount);
		writer.WriteInt64(_adenaCount);
		writer.WriteString(_distributorName);
	}
}