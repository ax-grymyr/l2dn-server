using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExSendUiEventPacket: IOutgoingPacket
{
	// UI Types
	public const int TYPE_COUNT_DOWN = 0;
	public const int TYPE_REMOVE = 1;
	public const int TYPE_ISTINA = 2;
	public const int TYPE_COUNTER = 3;
	public const int TYPE_GP_TIMER = 4;
	public const int TYPE_NORNIL = 5;
	public const int TYPE_DRACO_INCUBATION_1 = 6;
	public const int TYPE_DRACO_INCUBATION_2 = 7;
	public const int TYPE_CLAN_PROGRESS_BAR = 8;
	
	private readonly int _objectId;
	private readonly int _type;
	private readonly int _countUp;
	private readonly int _startTime;
	private readonly int _startTime2;
	private readonly int _endTime;
	private readonly int _endTime2;
	private readonly NpcStringId _npcstringId;
	private readonly List<string> _params;
	
	/**
	 * Remove UI
	 * @param player
	 */
	public ExSendUiEventPacket(Player player): this(player, TYPE_REMOVE, 0, 0, 0, 0, 0, (NpcStringId)(-1))
	{
	}

	/**
	 * @param player
	 * @param uiType
	 * @param currentPoints
	 * @param maxPoints
	 * @param npcString
	 * @param params
	 */
	public ExSendUiEventPacket(Player player, int uiType, int currentPoints, int maxPoints, NpcStringId npcString,
		params string[] @params)
		: this(player, uiType, -1, currentPoints, maxPoints, -1, -1, npcString, @params)
	{
	}

	/**
	 * @param player
	 * @param hide
	 * @param countUp
	 * @param startTime
	 * @param endTime
	 * @param text
	 */
	public ExSendUiEventPacket(Player player, bool hide, bool countUp, int startTime, int endTime, string text)
		: this(player, hide ? 1 : 0, countUp ? 1 : 0, startTime / 60, startTime % 60, endTime / 60, endTime % 60, (NpcStringId)(-1),
			text)
	{
	}

	/**
	 * @param player
	 * @param hide
	 * @param countUp
	 * @param startTime
	 * @param endTime
	 * @param npcString
	 * @param params
	 */
	public ExSendUiEventPacket(Player player, bool hide, bool countUp, int startTime, int endTime,
		NpcStringId npcString, params string[] @params)
		: this(player, hide ? 1 : 0, countUp ? 1 : 0, startTime / 60, startTime % 60, endTime / 60, endTime % 60,
			npcString, @params)
	{
	}

	/**
	 * @param player
	 * @param type
	 * @param countUp
	 * @param startTime
	 * @param startTime2
	 * @param endTime
	 * @param endTime2
	 * @param npcstringId
	 * @param params
	 */
	public ExSendUiEventPacket(Player player, int type, int countUp, int startTime, int startTime2, int endTime,
		int endTime2, NpcStringId npcstringId, params string[] @params)
	{
		_objectId = player.ObjectId;
		_type = type;
		_countUp = countUp;
		_startTime = startTime;
		_startTime2 = startTime2;
		_endTime = endTime;
		_endTime2 = endTime2;
		_npcstringId = npcstringId;
		_params = @params.ToList();
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_SEND_UIEVENT);
		
		writer.WriteInt32(_objectId);
		writer.WriteInt32(_type); // 0 = show, 1 = hide (there is 2 = pause and 3 = resume also but they don't work well you can only pause count down and you cannot resume it because resume hides the counter).
		writer.WriteInt32(0); // unknown
		writer.WriteInt32(0); // unknown
		writer.WriteString(_countUp.ToString()); // 0 = count down, 1 = count up timer always disappears 10 seconds before end
		writer.WriteString(_startTime.ToString());
		writer.WriteString(_startTime2.ToString());
		writer.WriteString(_endTime.ToString());
		writer.WriteString(_endTime2.ToString());
		writer.WriteInt32((int)_npcstringId);
		if (_params != null)
		{
			foreach (string param in _params)
			{
				writer.WriteString(param);
			}
		}
	}
}