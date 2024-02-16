using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ConfirmDialogPacket: IOutgoingPacket
{
	private readonly int _time;
	private readonly int _requesterId;
	private readonly SystemMessageParams _params;
	
	public ConfirmDialogPacket(SystemMessageParams parameters)
	{
		_params = parameters;
	}
	
	public ConfirmDialogPacket(SystemMessageId smId)
	{
		_params = new SystemMessageParams(smId);
	}
	
	public ConfirmDialogPacket(string text)
	{
		_params = new SystemMessageParams(SystemMessageId.S1_3);
		_params.addString(text);
	}

	public ConfirmDialogPacket(int time, int requesterId, SystemMessageParams parameters)
	{
		_time = time;
		_requesterId = requesterId;
		_params = parameters;
	}

	public ConfirmDialogPacket(int time, int requesterId, SystemMessageId smId)
	{
		_time = time;
		_requesterId = requesterId;
		_params = new SystemMessageParams(smId);
	}
	
	public ConfirmDialogPacket(int time, int requesterId, string text)
	{
		_time = time;
		_requesterId = requesterId;
		_params = new SystemMessageParams(SystemMessageId.S1_3);
		_params.addString(text);
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.CONFIRM_DLG);
		writer.WriteInt32((int)_params.MessageId);
		writer.WriteInt32(_params.Count);
		foreach (SystemMessageParam param in _params.Params)
		{
			writer.WriteInt32((int)param.Type);
			writer.WriteSystemMessageParam(param);
		}
		
		writer.WriteInt32(_time);
		writer.WriteInt32(_requesterId);
	}
}