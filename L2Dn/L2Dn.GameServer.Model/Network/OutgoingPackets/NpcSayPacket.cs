using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct NpcSayPacket: IOutgoingPacket
{
	private readonly int _objectId;
	private readonly ChatType _textType;
	private readonly int _npcId;
	private readonly string _text;
	private readonly NpcStringId? _npcString;
	private readonly List<string> _parameters;

	/**
	 * @param objectId
	 * @param messageType
	 * @param npcId
	 * @param text
	 */
	public NpcSayPacket(int objectId, ChatType messageType, int npcId, string text)
	{
		_objectId = objectId;
		_textType = messageType;
		_npcId = 1000000 + npcId;
		_text = text;
        _parameters = [];
    }

	public NpcSayPacket(Npc npc, ChatType messageType, string text)
	{
		_objectId = npc.ObjectId;
		_textType = messageType;
		_npcId = 1000000 + npc.getTemplate().getDisplayId();
		_text = text;
        _parameters = [];
	}

	public NpcSayPacket(int objectId, ChatType messageType, int npcId, NpcStringId npcString)
	{
		_objectId = objectId;
		_textType = messageType;
		_npcId = 1000000 + npcId;
		_npcString = npcString;
		_parameters = [];
        _text = string.Empty;
    }

	public NpcSayPacket(Npc npc, ChatType messageType, NpcStringId npcString)
	{
		_objectId = npc.ObjectId;
		_textType = messageType;
		_npcId = 1000000 + npc.getTemplate().getDisplayId();
		_npcString = npcString;
		_parameters = [];
        _text = string.Empty;
}

	/**
	 * @param text the text to add as a parameter for this packet's message (replaces S1, S2 etc.)
	 * @return this NpcSay packet object
	 */
	public NpcSayPacket addStringParameter(string text)
	{
		_parameters.Add(text);
		return this;
	}

	/**
	 * @param params a list of strings to add as parameters for this packet's message (replaces S1, S2 etc.)
	 * @return this NpcSay packet object
	 */
	public NpcSayPacket addStringParameters(params string[] @params)
	{
		foreach (string item in @params)
		{
			if (!string.IsNullOrEmpty(item))
				_parameters.Add(item);
		}

		return this;
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.NPC_SAY);

		writer.WriteInt32(_objectId);
		writer.WriteInt32((int)_textType);
		writer.WriteInt32(_npcId);

		// // Localisation related.
		// if (Config.MULTILANG_ENABLE)
		// {
		// 	final Player player = getPlayer();
		// 	if (player != null)
		// 	{
		// 		final String lang = player.getLang();
		// 		if ((lang != null) && !lang.equals("en"))
		// 		{
		// 			final NpcStringId ns = NpcStringId.getNpcStringId(_npcString);
		// 			if (ns != null)
		// 			{
		// 				final NSLocalisation nsl = ns.getLocalisation(lang);
		// 				if (nsl != null)
		// 				{
		// 					writer.WriteInt32(-1);
		// 					writeString(nsl.getLocalisation(_parameters != null ? _parameters : Collections.emptyList()));
		// 					return;
		// 				}
		// 			}
		// 		}
		// 	}
		// }

		if (_npcString == null)
		{
			writer.WriteInt32(-1);
			writer.WriteString(_text);
		}
		else
		{
			writer.WriteInt32((int)_npcString.Value);
			if (_parameters != null)
			{
				foreach (string s in _parameters)
					writer.WriteString(s);
			}
		}
	}
}