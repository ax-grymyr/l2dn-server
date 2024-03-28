using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Appearance;
using L2Dn.GameServer.Model.Actor.Stats;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;
using L2Dn.Network;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct CharacterCreatePacket: IIncomingPacket<GameSession>
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(CharacterCreatePacket)); 
    private string? _name;
    private Sex _sex;
    private Race _race;
    private CharacterClass _class;
    private int _hairStyle;
    private int _hairColor;
    private int _face;

    public void ReadContent(PacketBitReader reader)
    {
        _name = reader.ReadString();
        _race = (Race)reader.ReadInt32();
        _sex = (Sex)reader.ReadInt32();
        _class = (CharacterClass)reader.ReadInt32();

        reader.Skip(24); // int, str, con, men, dex, wit
        _hairStyle = reader.ReadInt32();
        _hairColor = reader.ReadInt32();
        _face = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		// Last Verified: May 30, 2009 - Gracia Final - Players are able to create characters with names consisting of as little as 1,2,3 letter/number combinations.
		if (string.IsNullOrEmpty(_name) || _name.Length > 16)
		{
			CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.LengthExceeded);
			connection.Send(ref createFailPacket);
			return ValueTask.CompletedTask;
		}
		
		if (Config.FORBIDDEN_NAMES.Count > 0)
		{
			foreach (string st in Config.FORBIDDEN_NAMES)
			{
				if (_name.ToLower().Contains(st.ToLower()))
				{
					CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.IncorrectName);
					connection.Send(ref createFailPacket);
					return ValueTask.CompletedTask;
				}
			}
		}
		
		if (FakePlayerData.getInstance().getProperName(_name) != null)
		{
			CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.IncorrectName);
			connection.Send(ref createFailPacket);
			return ValueTask.CompletedTask;
		}
		
		// Last Verified: May 30, 2009 - Gracia Final
		if (!Util.isAlphaNumeric(_name) || !IsValidName(_name))
		{
			CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.IncorrectName);
			connection.Send(ref createFailPacket);
			return ValueTask.CompletedTask;
		}
		
		if (_face > 4 || _face < 0)
		{
			CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
			connection.Send(ref createFailPacket);
			return ValueTask.CompletedTask;
		}
		
		if ((_hairStyle < 0) || (_sex != Sex.Female && _hairStyle > 8) || (_sex == Sex.Female && _hairStyle > 11))
		{
			CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
			connection.Send(ref createFailPacket);
			return ValueTask.CompletedTask;
		}
		
		if (_hairColor > 3 || _hairColor < 0)
		{
			CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
			connection.Send(ref createFailPacket);
			return ValueTask.CompletedTask;
		}
		
		/*
		 * DrHouse: Since checks for duplicate names are done using SQL, lock must be held until data is written to DB as well.
		 */
		Player newChar;
		lock (CharInfoTable.getInstance())
		{
			if ((CharInfoTable.getInstance().getAccountCharacterCount(session.AccountName) >=
			     Config.MAX_CHARACTERS_NUMBER_PER_ACCOUNT) && (Config.MAX_CHARACTERS_NUMBER_PER_ACCOUNT != 0))
			{
				CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.TooManyCharacters);
				connection.Send(ref createFailPacket);
				return ValueTask.CompletedTask;
			}
			
			if (CharInfoTable.getInstance().doesCharNameExist(_name))
			{
				CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.NameAlreadyExists);
				connection.Send(ref createFailPacket);
				return ValueTask.CompletedTask;
			}
			
			PlayerTemplate template = PlayerTemplateData.getInstance().getTemplate(_class);
			if (template == null || _class.GetLevel() > 0)
			{
				CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
				connection.Send(ref createFailPacket);
				return ValueTask.CompletedTask;
			}
			
			// Custom Feature: Disallow a race to be created.
			// Example: Humans can not be created if AllowHuman = False in Custom.properties
			switch (template.getRace())
			{
				case Race.HUMAN:
				{
					if (!Config.ALLOW_HUMAN)
					{
						CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
						connection.Send(ref createFailPacket);
						return ValueTask.CompletedTask;
					}
					
					if (CategoryData.getInstance().isInCategory(CategoryType.DEATH_KNIGHT_ALL_CLASS, _class) && _sex == Sex.Female)
					{
						CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
						connection.Send(ref createFailPacket);
						return ValueTask.CompletedTask;
					}
					
					if (CategoryData.getInstance().isInCategory(CategoryType.ASSASSIN_ALL_CLASS, _class) && _sex == Sex.Female)
					{
						CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
						connection.Send(ref createFailPacket);
						return ValueTask.CompletedTask;
					}
					
					break;
				}
				case Race.ELF:
				{
					if (!Config.ALLOW_ELF)
					{
						CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
						connection.Send(ref createFailPacket);
						return ValueTask.CompletedTask;
					}

					if (CategoryData.getInstance().isInCategory(CategoryType.DEATH_KNIGHT_ALL_CLASS, _class) && _sex == Sex.Female)
					{
						CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
						connection.Send(ref createFailPacket);
						return ValueTask.CompletedTask;
					}
					
					break;
				}
				case Race.DARK_ELF:
				{
					if (!Config.ALLOW_DARKELF)
					{
						CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
						connection.Send(ref createFailPacket);
						return ValueTask.CompletedTask;
					}
					
					if (CategoryData.getInstance().isInCategory(CategoryType.DEATH_KNIGHT_ALL_CLASS, _class) && _sex == Sex.Female)
					{
						CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
						connection.Send(ref createFailPacket);
						return ValueTask.CompletedTask;
					}
					
					if (CategoryData.getInstance().isInCategory(CategoryType.ASSASSIN_ALL_CLASS, _class) && _sex != Sex.Female)
					{
						CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
						connection.Send(ref createFailPacket);
						return ValueTask.CompletedTask;
					}
					
					break;
				}
				case Race.ORC:
				{
					if (!Config.ALLOW_ORC)
					{
						CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
						connection.Send(ref createFailPacket);
						return ValueTask.CompletedTask;
					}
					
					if (CategoryData.getInstance().isInCategory(CategoryType.VANGUARD_ALL_CLASS, _class) && _sex == Sex.Female)
					{
						CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
						connection.Send(ref createFailPacket);
						return ValueTask.CompletedTask;
					}
					
					break;
				}
				case Race.DWARF:
				{
					if (!Config.ALLOW_DWARF)
					{
						CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
						connection.Send(ref createFailPacket);
						return ValueTask.CompletedTask;
					}
					
					break;
				}
				case Race.KAMAEL:
				{
					if (!Config.ALLOW_KAMAEL)
					{
						CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
						connection.Send(ref createFailPacket);
						return ValueTask.CompletedTask;
					}
					
					break;
				}
				case Race.SYLPH:
				{
					if (!Config.ALLOW_SYLPH)
					{
						CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
						connection.Send(ref createFailPacket);
						return ValueTask.CompletedTask;
					}
					
					break;
				}
			}
			
			if (!Config.ALLOW_DEATH_KNIGHT && CategoryData.getInstance().isInCategory(CategoryType.DEATH_KNIGHT_ALL_CLASS, _class))
			{
				CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
				connection.Send(ref createFailPacket);
				return ValueTask.CompletedTask;
			}
			
			if (!Config.ALLOW_VANGUARD && CategoryData.getInstance().isInCategory(CategoryType.VANGUARD_ALL_CLASS, _class))
			{
				CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
				connection.Send(ref createFailPacket);
				return ValueTask.CompletedTask;
			}

			newChar = Player.create(template, session.AccountId, session.AccountName, _name,
				new PlayerAppearance((byte)_face, (byte)_hairColor, (byte)_hairStyle, _sex));
		}
		
		// HP and MP are at maximum and CP is zero by default.
		newChar.setCurrentHp(newChar.getMaxHp());
		newChar.setCurrentMp(newChar.getMaxMp());
		// newChar.setMaxLoad(template.getBaseLoad());
		
		CharacterCreateSuccessPacket createSuccessPacket = new();
		connection.Send(ref createSuccessPacket);
		
		InitNewChar(connection, session, newChar);
		return ValueTask.CompletedTask;
	}
	
	private static void InitNewChar(Connection connection, GameSession session, Player newChar)
	{
		World.getInstance().addObject(newChar);
		
		if (Config.STARTING_ADENA > 0)
		{
			newChar.addAdena("Init", Config.STARTING_ADENA, null, false);
		}
		
		PlayerTemplate template = newChar.getTemplate();
		if (Config.CUSTOM_STARTING_LOC)
		{
			Location createLoc = new Location(Config.CUSTOM_STARTING_LOC_X, Config.CUSTOM_STARTING_LOC_Y, Config.CUSTOM_STARTING_LOC_Z);
			newChar.setXYZInvisible(createLoc.getX(), createLoc.getY(), createLoc.getZ());
		}
		else if (Config.FACTION_SYSTEM_ENABLED)
		{
			newChar.setXYZInvisible(Config.FACTION_STARTING_LOCATION.getX(), Config.FACTION_STARTING_LOCATION.getY(), Config.FACTION_STARTING_LOCATION.getZ());
		}
		else
		{
			Location createLoc = template.getCreationPoint();
			newChar.setXYZInvisible(createLoc.getX(), createLoc.getY(), createLoc.getZ());
		}
		
		newChar.setTitle("");
		
		if (Config.ENABLE_VITALITY)
		{
			newChar.setVitalityPoints(Math.Min(Config.STARTING_VITALITY_POINTS, PlayerStat.MAX_VITALITY_POINTS), true);
		}
		
		if (Config.STARTING_LEVEL > 1)
		{
			newChar.getStat().addLevel(Config.STARTING_LEVEL - 1);
		}
		
		if (Config.STARTING_SP > 0)
		{
			newChar.getStat().addSp(Config.STARTING_SP);
		}
		
		List<PlayerItemTemplate> initialItems = InitialEquipmentData.getInstance().getEquipmentList(newChar.getClassId());
		if (initialItems != null)
		{
			foreach (PlayerItemTemplate ie in initialItems)
			{
				Item item = newChar.getInventory().addItem("Init", ie.getId(), ie.getCount(), newChar, null);
				if (item == null)
				{
					_logger.Error("Could not create item during char creation: itemId " + ie.getId() + ", amount " + ie.getCount() + ".");
					continue;
				}
				
				if (item.isEquipable() && ie.isEquipped())
				{
					newChar.getInventory().equipItem(item);
				}
			}
		}
		
		foreach (SkillLearn skill in SkillTreeData.getInstance().getAvailableSkills(newChar, newChar.getClassId(), false, true))
		{
			newChar.addSkill(SkillData.getInstance().getSkill(skill.getSkillId(), skill.getSkillLevel()), true);
		}
		
		// Register all shortcuts for actions, skills and items for this new character.
		InitialShortcutData.getInstance().registerAllShortcuts(newChar);
		
		if (GlobalEvents.Players.HasSubscribers<OnPlayerCreate>())
		{
			GlobalEvents.Players.Notify(new OnPlayerCreate(newChar, newChar.getObjectId(), newChar.getName(), session));
		}
		
		newChar.setOnlineStatus(true, false);
		Disconnection.of(session, newChar).storeMe().deleteMe();

		session.Characters = CharacterPacketHelper.LoadCharacterSelectInfo(session.AccountId);
		CharacterListPacket characterListPacket = new(session.PlayKey1, session.AccountName, session.Characters);
		connection.Send(ref characterListPacket);
    }

    private static bool IsValidName(string name)
    {
        if (name.Length is < 1 or > 16)
            return false;

        foreach (char c in name)
        {
            // TODO: make config parameter to allow/disallow russian nicknames
            if (!char.IsAsciiLetterOrDigit(c) &&
                !char.IsBetween(c, 'А', 'Я') &&
                !char.IsBetween(c, 'а', 'я'))
                return false;
        }

        return true;
    }
}