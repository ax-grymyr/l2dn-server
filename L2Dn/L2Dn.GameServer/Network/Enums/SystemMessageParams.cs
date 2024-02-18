using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Network.Enums;

public sealed class SystemMessageParams
{
    private readonly SystemMessageId _smId;
    private readonly List<SystemMessageParam> _params;

    public SystemMessageParams(SystemMessageId smId)
    {
        if (!Enum.IsDefined(smId))
            throw new ArgumentOutOfRangeException(nameof(smId), smId, "Invalid SystemMessageId");

        _smId = smId;
    }
	
	public SystemMessageParams(string text)
	{
        ArgumentNullException.ThrowIfNull(text);

        _smId = SystemMessageId.S1_2;
		addString(text);
	}

	public List<SystemMessageParam> Params => _params;
	public int Count => _params.Count;
	public SystemMessageId MessageId => _smId; 
	
	private void append(SystemMessageParamType type, object value)
	{
		int smIdParamCount = _smId.GetParamCount(); 
		if (_params.Count < smIdParamCount)
			_params.Add(new SystemMessageParam(type, value));
		else if (_params.Count == smIdParamCount && type == SystemMessageParamType.TYPE_POPUP_ID)
			_params.Add(new SystemMessageParam(SystemMessageParamType.TYPE_POPUP_ID, value));
		else
			throw new InvalidOperationException(
				$"Wrong parameter count '{_params.Count + 1}' for SystemMessageId: {_smId}");
	}
	
	public SystemMessageParams addString(string text)
	{
		append(SystemMessageParamType.TYPE_TEXT, text);
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
	public SystemMessageParams addCastleId(int number)
	{
		append(SystemMessageParamType.TYPE_CASTLE_NAME, number);
		return this;
	}
	
	public SystemMessageParams addInt(int number)
	{
		append(SystemMessageParamType.TYPE_INT_NUMBER, number);
		return this;
	}
	
	public SystemMessageParams addLong(long number)
	{
		append(SystemMessageParamType.TYPE_LONG_NUMBER, number);
		return this;
	}
	
	public SystemMessageParams addPcName(Player pc)
	{
		append(SystemMessageParamType.TYPE_PLAYER_NAME, pc.getAppearance().getVisibleName());
		return this;
	}
	
	/**
	 * ID from doorData.xml
	 * @param doorId
	 * @return
	 */
	public SystemMessageParams addDoorName(int doorId)
	{
		append(SystemMessageParamType.TYPE_DOOR_NAME, doorId);
		return this;
	}
	
	public SystemMessageParams addNpcName(Npc npc)
	{
		return addNpcName(npc.getTemplate());
	}
	
	public SystemMessageParams addNpcName(Summon npc)
	{
		return addNpcName(npc.getId());
	}
	
	public SystemMessageParams addNpcName(NpcTemplate template)
	{
		if (template.isUsingServerSideName())
		{
			return addString(template.getName());
		}
		
		return addNpcName(template.getId());
	}
	
	public SystemMessageParams addNpcName(int id)
	{
		append(SystemMessageParamType.TYPE_NPC_NAME, 1000000 + id);
		return this;
	}
	
	public SystemMessageParams addItemName(Item item)
	{
		return addItemName(item.getId());
	}
	
	public SystemMessageParams addItemName(ItemTemplate item)
	{
		return addItemName(item.getId());
	}
	
	public SystemMessageParams addItemName(int id)
	{
		ItemTemplate item = ItemData.getInstance().getTemplate(id);
		if (item.getDisplayId() != id)
		{
			return addString(item.getName());
		}
		
		append(SystemMessageParamType.TYPE_ITEM_NAME, id);
		return this;
	}
	
	public SystemMessageParams addZoneName(int x, int y, int z)
	{
		append(SystemMessageParamType.TYPE_ZONE_NAME, new int[]
		{
			x,
			y,
			z
		});
		return this;
	}
	
	public SystemMessageParams addSkillName(Skill skill)
	{
		if (skill.getId() != skill.getDisplayId())
		{
			return addString(skill.getName());
		}
		return addSkillName(skill.getId(), skill.getLevel(), skill.getSubLevel());
	}
	
	public SystemMessageParams addSkillName(int id)
	{
		return addSkillName(id, 1, 0);
	}
	
	public SystemMessageParams addSkillName(int id, int lvl, int subLevel)
	{
		append(SystemMessageParamType.TYPE_SKILL_NAME, new[]
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
	public SystemMessageParams addAttribute(int type)
	{
		append(SystemMessageParamType.TYPE_ELEMENT_NAME, type);
		return this;
	}
	
	/**
	 * ID from sysstring-e.dat
	 * @param type
	 * @return
	 */
	public SystemMessageParams addSystemString(int type)
	{
		append(SystemMessageParamType.TYPE_SYSTEM_STRING, type);
		return this;
	}
	
	/**
	 * ID from ClassInfo-e.dat
	 * @param type
	 * @return
	 */
	public SystemMessageParams addClassId(CharacterClass type)
	{
		append(SystemMessageParamType.TYPE_CLASS_ID, (int)type);
		return this;
	}
	
	public SystemMessageParams addFactionName(int factionId)
	{
		append(SystemMessageParamType.TYPE_FACTION_NAME, factionId);
		return this;
	}
	
	public SystemMessageParams addPopup(int target, int attacker, int damage)
	{
		append(SystemMessageParamType.TYPE_POPUP_ID, new[]
		{
			target,
			attacker,
			damage
		});
		return this;
	}
	
	public SystemMessageParams addByte(int time)
	{
		append(SystemMessageParamType.TYPE_BYTE, time);
		return this;
	}
	
	/**
	 * Instance name from instantzonedata-e.dat
	 * @param type id of instance
	 * @return
	 */
	public SystemMessageParams addInstanceName(int type)
	{
		append(SystemMessageParamType.TYPE_INSTANCE_NAME, type);
		return this;
	}
	
	public SystemMessageParams addElementalSpirit(int elementType)
	{
		append(SystemMessageParamType.TYPE_ELEMENTAL_SPIRIT, elementType);
		return this;
	}
}