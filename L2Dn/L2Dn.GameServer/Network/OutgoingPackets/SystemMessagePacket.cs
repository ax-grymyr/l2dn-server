using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal struct SystemMessagePacket: IOutgoingPacket
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(SystemMessageId));
	private readonly SystemMessageId _smId;
	private readonly SmParam[] _params;
	private int[]? _popupParam;
	private int _paramIndex;
	
	public SystemMessagePacket(SystemMessageId smId)
	{
		if (!Enum.IsDefined(smId))
			throw new ArgumentOutOfRangeException(nameof(smId), smId, "Invalid SystemMessageId");

		_smId = smId;
		int paramCount = smId.GetParamCount(); 
		_params = paramCount > 0 ? new SmParam[paramCount] : Array.Empty<SmParam>();
	}
	
	public SystemMessagePacket(string text)
	{
        ArgumentNullException.ThrowIfNull(text);

        _smId = SystemMessageId.S1_2;
		_params = new SmParam[1];
		addString(text);
	}

	private void append(SmParamType type, object value)
	{
		if (_paramIndex < _params.Length)
			_params[_paramIndex++] = new SmParam(type, value);
		else if (_paramIndex == _params.Length && type == SmParamType.TYPE_POPUP_ID)
			_popupParam = (int[])value;
		else
			_logger.Error($"Wrong parameter count '{(_paramIndex + 1)}' for SystemMessageId: {_smId}");
	}
	
	public SystemMessagePacket addString(String text)
	{
		append(SmParamType.TYPE_TEXT, text);
		return this;
	}
	
	/**
	 * Appends a Castle name parameter type, the name will be read from CastleName-e.dat.<br>
	 * <ul>
	 * <li>1-9 Castle names</li>
	 * <li>21 Fortress of Resistance</li>
	 * <li>22-33 Clan Hall names</li>
	 * <li>34 Devastated Castle</li>
	 * <li>35 Bandit Stronghold</li>
	 * <li>36-61 Clan Hall names</li>
	 * <li>62 Rainbow Springs</li>
	 * <li>63 Wild Beast Reserve</li>
	 * <li>64 Fortress of the Dead</li>
	 * <li>81-89 Territory names</li>
	 * <li>90-100 null</li>
	 * <li>101-121 Fortress names</li>
	 * </ul>
	 * @param number the conquerable entity
	 * @return the system message with the proper parameter
	 */
	public SystemMessagePacket addCastleId(int number)
	{
		append(SmParamType.TYPE_CASTLE_NAME, number);
		return this;
	}
	
	public SystemMessagePacket addInt(int number)
	{
		append(SmParamType.TYPE_INT_NUMBER, number);
		return this;
	}
	
	public SystemMessagePacket addLong(long number)
	{
		append(SmParamType.TYPE_LONG_NUMBER, number);
		return this;
	}
	
	public SystemMessagePacket addPcName(Player pc)
	{
		append(SmParamType.TYPE_PLAYER_NAME, pc.getAppearance().getVisibleName());
		return this;
	}
	
	/**
	 * ID from doorData.xml
	 * @param doorId
	 * @return
	 */
	public SystemMessagePacket addDoorName(int doorId)
	{
		append(SmParamType.TYPE_DOOR_NAME, doorId);
		return this;
	}
	
	public SystemMessagePacket addNpcName(Npc npc)
	{
		return addNpcName(npc.getTemplate());
	}
	
	public SystemMessagePacket addNpcName(Summon npc)
	{
		return addNpcName(npc.getId());
	}
	
	public SystemMessagePacket addNpcName(NpcTemplate template)
	{
		if (template.isUsingServerSideName())
		{
			return addString(template.getName());
		}
		
		return addNpcName(template.getId());
	}
	
	public SystemMessagePacket addNpcName(int id)
	{
		append(SmParamType.TYPE_NPC_NAME, 1000000 + id);
		return this;
	}
	
	public SystemMessagePacket addItemName(Item item)
	{
		return addItemName(item.getId());
	}
	
	public SystemMessagePacket addItemName(ItemTemplate item)
	{
		return addItemName(item.getId());
	}
	
	public SystemMessagePacket addItemName(int id)
	{
		ItemTemplate item = ItemData.getInstance().getTemplate(id);
		if (item.getDisplayId() != id)
		{
			return addString(item.getName());
		}
		
		append(SmParamType.TYPE_ITEM_NAME, id);
		return this;
	}
	
	public SystemMessagePacket addZoneName(int x, int y, int z)
	{
		append(SmParamType.TYPE_ZONE_NAME, new int[]
		{
			x,
			y,
			z
		});
		return this;
	}
	
	public SystemMessagePacket addSkillName(Skill skill)
	{
		if (skill.getId() != skill.getDisplayId())
		{
			return addString(skill.getName());
		}
		return addSkillName(skill.getId(), skill.getLevel(), skill.getSubLevel());
	}
	
	public SystemMessagePacket addSkillName(int id)
	{
		return addSkillName(id, 1, 0);
	}
	
	public SystemMessagePacket addSkillName(int id, int lvl, int subLevel)
	{
		append(SmParamType.TYPE_SKILL_NAME, new[]
		{
			id,
			lvl,
			subLevel
		});
		return this;
	}
	
	/**
	 * Elemental name - 0(Fire) ...
	 * @param type
	 * @return
	 */
	public SystemMessagePacket addAttribute(int type)
	{
		append(SmParamType.TYPE_ELEMENT_NAME, type);
		return this;
	}
	
	/**
	 * ID from sysstring-e.dat
	 * @param type
	 * @return
	 */
	public SystemMessagePacket addSystemString(int type)
	{
		append(SmParamType.TYPE_SYSTEM_STRING, type);
		return this;
	}
	
	/**
	 * ID from ClassInfo-e.dat
	 * @param type
	 * @return
	 */
	public SystemMessagePacket addClassId(int type)
	{
		append(SmParamType.TYPE_CLASS_ID, type);
		return this;
	}
	
	public SystemMessagePacket addFactionName(int factionId)
	{
		append(SmParamType.TYPE_FACTION_NAME, factionId);
		return this;
	}
	
	public SystemMessagePacket addPopup(int target, int attacker, int damage)
	{
		append(SmParamType.TYPE_POPUP_ID, new[]
		{
			target,
			attacker,
			damage
		});
		return this;
	}
	
	public SystemMessagePacket addByte(int time)
	{
		append(SmParamType.TYPE_BYTE, time);
		return this;
	}
	
	/**
	 * Instance name from instantzonedata-e.dat
	 * @param type id of instance
	 * @return
	 */
	public SystemMessagePacket addInstanceName(int type)
	{
		append(SmParamType.TYPE_INSTANCE_NAME, type);
		return this;
	}
	
	public SystemMessagePacket addElementalSpirit(int elementType)
	{
		append(SmParamType.TYPE_ELEMENTAL_SPIRIT, elementType);
		return this;
	}

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

		if (_paramIndex < _params.Length)
			_logger.Error($"Wrong parameter count '{(_paramIndex + 1)}' for SystemMessageId: {_smId}");
        
		writer.WriteEnum(_smId);
		writer.WriteByte((byte)_paramIndex);
		for (int index = 0, count = Math.Min(_paramIndex, _params.Length); index < count; index++)
		{
			SmParam param = _params[index];
			writer.WriteEnum(param.Type);
			switch (param.Type)
			{
				case SmParamType.TYPE_ELEMENT_NAME:
				case SmParamType.TYPE_BYTE:
				case SmParamType.TYPE_FACTION_NAME:
				case SmParamType.TYPE_ELEMENTAL_SPIRIT:
				{
					writer.WriteByte((byte)(int)param.Value);
					break;
				}
				case SmParamType.TYPE_CASTLE_NAME:
				case SmParamType.TYPE_SYSTEM_STRING:
				case SmParamType.TYPE_INSTANCE_NAME:
				case SmParamType.TYPE_CLASS_ID:
				{
					writer.WriteInt16((short)(int)param.Value);
					break;
				}
				case SmParamType.TYPE_ITEM_NAME:
				case SmParamType.TYPE_INT_NUMBER:
				case SmParamType.TYPE_NPC_NAME:
				case SmParamType.TYPE_DOOR_NAME:
				{
					writer.WriteInt32((int)param.Value);
					break;
				}
				case SmParamType.TYPE_LONG_NUMBER:
				{
					writer.WriteInt64((long)param.Value);
					break;
				}
				case SmParamType.TYPE_TEXT:
				case SmParamType.TYPE_PLAYER_NAME:
				{
					writer.WriteString((string)param.Value);
					break;
				}
				case SmParamType.TYPE_SKILL_NAME:
				{
					int[] array = (int[])param.Value;
					writer.WriteInt32(array[0]); // skill id
					writer.WriteInt16((short)array[1]); // skill level
					writer.WriteInt16((short)array[2]); // skill sub level
					break;
				}
				case SmParamType.TYPE_POPUP_ID:
				case SmParamType.TYPE_ZONE_NAME:
				{
					int[] array = (int[])param.Value;
					writer.WriteInt32(array[0]); // x
					writer.WriteInt32(array[1]); // y
					writer.WriteInt32(array[2]); // z
					break;
				}
				default:
				{
					_logger.Error($"Wrong parameter type for index '{index}' for SystemMessageId: {_smId}");
					break;
				}
			}
		}

		if (_popupParam is not null)
		{
			writer.WriteEnum(SmParamType.TYPE_POPUP_ID);
			writer.WriteInt32(_popupParam[0]);
			writer.WriteInt32(_popupParam[1]);
			writer.WriteInt32(_popupParam[2]);
		}
	}
	private readonly struct SmParam(SmParamType type, object value)
	{
		public SmParamType Type { get; } = type;
		public object Value { get; } = value; // TODO: remove boxing
	}

	private enum SmParamType: byte
	{
		TYPE_TEXT = 0,
		TYPE_INT_NUMBER = 1,
		TYPE_NPC_NAME = 2,
		TYPE_ITEM_NAME = 3,
		TYPE_SKILL_NAME = 4,
		TYPE_CASTLE_NAME = 5,
		TYPE_LONG_NUMBER = 6,
		TYPE_ZONE_NAME = 7,

		// id 8 - ddd
		TYPE_ELEMENT_NAME = 9,
		TYPE_INSTANCE_NAME = 10,
		TYPE_DOOR_NAME = 11,
		TYPE_PLAYER_NAME = 12,
		TYPE_SYSTEM_STRING = 13,

		// id 14 dSSSSS
		TYPE_CLASS_ID = 15,
		TYPE_POPUP_ID = 16,

		// id 17 shared with 1-3,17,22
		// id 18 Q (read same as 6)
		// id 19 c
		// id 20 c
		// id 21 h
		// id 22 d (shared with 1-3,17,22
		TYPE_BYTE = 20,
		TYPE_FACTION_NAME = 24, // c(short), faction id.
		TYPE_ELEMENTAL_SPIRIT = 26,
	}
}