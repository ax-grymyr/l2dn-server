using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct SystemMessagePacket: IOutgoingPacket
{
	private readonly SystemMessageParams _params;
	
	public SystemMessagePacket(SystemMessageParams parameters)
	{
		_params = parameters;
	}
	
	public SystemMessagePacket(SystemMessageId smId)
	{
		_params = new SystemMessageParams(smId);
	}
	
	public SystemMessagePacket(string text)
	{
		ArgumentNullException.ThrowIfNull(text);
		_params = new SystemMessageParams(text);
	}

	public SystemMessageParams Params => _params;

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.SYSTEM_MESSAGE);
		
		// Localisation related.
		// if (Config.MULTILANG_ENABLE)
		// {
		// 	Player player = getPlayer();
		// 	if (player != null)
		// 	{
		// 		String lang = player.getLang();
		// 		if ((lang != null) && !lang.Equals("en"))
		// 		{
		// 			SMLocalisation sml = _smId.getLocalisation(lang);
		// 			if (sml != null)
		// 			{
		// 				Object[] parameters = new Object[_paramIndex];
		// 				for (int i = 0; i < _paramIndex; i++)
		// 				{
		// 					parameters[i] = _params[i].getValue();
		// 				}
		// 				writer.WriteInt16(SystemMessageId.S1_2.getId());
		// 				writer.WriteByte(1);
		// 				writer.WriteByte(TYPE_TEXT);
		// 				writeString(sml.getLocalisation(params));
		// 				return;
		// 			}
		// 		}
		// 	}
		// }

		writer.WriteInt16((short)_params.MessageId);
		writer.WriteByte((byte)_params.Count);
		foreach (SystemMessageParam param in _params.Params)
		{
			writer.WriteByte((byte)param.Type);
			writer.WriteSystemMessageParam(param);
		}
	}
}