﻿using L2Dn.Extensions;
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
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.NetworkAuthServer;
using L2Dn.GameServer.NetworkAuthServer.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct CharacterCreatePacket: IIncomingPacket<GameSession>
{
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
	    if (session.Characters is null)
	    {
		    // characters were not loaded in AuthLoginPacket
		    connection.Close();
		    return ValueTask.CompletedTask;
	    }

		// Last Verified: May 30, 2009 - Gracia Final - Players are able to create characters with
		// names consisting of as little as 1,2,3 letter/number combinations.
		if (string.IsNullOrEmpty(_name) || _name.Length > 16)
		{
			CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.LengthExceeded);
			connection.Send(ref createFailPacket);
			return ValueTask.CompletedTask;
		}

		if (Config.Character.FORBIDDEN_NAMES.Count > 0)
		{
			foreach (string st in Config.Character.FORBIDDEN_NAMES)
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
		if (string.IsNullOrEmpty(_name) || !_name.ContainsAlphaNumericOnly() || !IsValidName(_name))
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

		if (_hairStyle < 0 || (_sex != Sex.Female && _hairStyle > 8) || (_sex == Sex.Female && _hairStyle > 11))
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
			if (Config.Server.MAX_CHARACTERS_NUMBER_PER_ACCOUNT != 0 &&
			    (session.Characters.Count >= Config.Server.MAX_CHARACTERS_NUMBER_PER_ACCOUNT ||
			     CharInfoTable.getInstance().getAccountCharacterCount(session.AccountName) >=
			     Config.Server.MAX_CHARACTERS_NUMBER_PER_ACCOUNT))
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

			PlayerTemplate? template = PlayerTemplateData.getInstance().getTemplate(_class);
			if (template == null || _class.GetLevel() > 0)
			{
				CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
				connection.Send(ref createFailPacket);
				return ValueTask.CompletedTask;
			}

			if (!CheckRaceAndClass(_race, _sex, _class))
			{
				CharacterCreateFailPacket createFailPacket = new(CharacterCreateFailReason.CreationFailed);
				connection.Send(ref createFailPacket);
				return ValueTask.CompletedTask;
			}

            newChar = Player.Create(template, session.AccountId, session.AccountName, _name, _sex, (byte)_face,
                (byte)_hairColor, (byte)_hairStyle);
        }

		// HP and MP are at maximum and CP is zero by default.
		newChar.setCurrentHp(newChar.getMaxHp());
		newChar.setCurrentMp(newChar.getMaxMp());

		InitNewChar(session, newChar);

		CharacterCreateSuccessPacket createSuccessPacket = new();
		connection.Send(ref createSuccessPacket);

		session.Characters.AddNewChar(newChar);
		CharacterListPacket characterListPacket = new(session.PlayKey1, session.AccountName, session.Characters);
		connection.Send(ref characterListPacket);

		// Update character count on AuthServer
		AccountStatusPacket accountStatusPacket = new(session.AccountId, (byte)session.Characters.Count);
		AuthServerSession.Send(ref accountStatusPacket);

		return ValueTask.CompletedTask;
	}

	private static void InitNewChar(GameSession session, Player newChar)
	{
		World.getInstance().addObject(newChar);

		if (Config.Character.STARTING_ADENA > 0)
		{
			newChar.addAdena("Init", Config.Character.STARTING_ADENA, null, false);
		}

		PlayerTemplate template = newChar.getTemplate();
		if (Config.StartingLocation.CUSTOM_STARTING_LOC)
		{
			Location3D createLoc = new(Config.StartingLocation.CUSTOM_STARTING_LOC_X, Config.StartingLocation.CUSTOM_STARTING_LOC_Y, Config.StartingLocation.CUSTOM_STARTING_LOC_Z);
			newChar.setXYZInvisible(createLoc);
		}
		else if (Config.FactionSystem.FACTION_SYSTEM_ENABLED)
		{
			newChar.setXYZInvisible(Config.FactionSystem.FACTION_STARTING_LOCATION.Location3D);
		}
		else
		{
			Location3D createLoc = template.getCreationPoint();
			newChar.setXYZInvisible(createLoc);
		}

		newChar.setTitle("");

		if (Config.Character.ENABLE_VITALITY)
		{
			newChar.setVitalityPoints(Math.Min(Config.Character.STARTING_VITALITY_POINTS, PlayerStat.MAX_VITALITY_POINTS), true);
		}

		if (Config.Character.STARTING_LEVEL > 1)
		{
			newChar.getStat().addLevel(Config.Character.STARTING_LEVEL - 1);
		}

		if (Config.Character.STARTING_SP > 0)
		{
			newChar.getStat().addSp(Config.Character.STARTING_SP);
		}

		List<PlayerItemTemplate>? initialItems = InitialEquipmentData.getInstance().getEquipmentList(newChar.getClassId());
		if (initialItems != null)
		{
			foreach (PlayerItemTemplate ie in initialItems)
			{
				Item? item = newChar.getInventory().addItem("Init", ie.getId(), ie.getCount(), newChar, null);
				if (item == null)
				{
					PacketLogger.Instance.Error("Could not create item during char creation: itemId " + ie.getId() +
					                            ", amount " + ie.getCount() + ".");

					continue;
				}

				if (item.isEquipable() && ie.isEquipped())
				{
					newChar.getInventory().equipItem(item);
				}
			}
		}

		foreach (SkillLearn skill in SkillTreeData.getInstance().getAvailableSkills(newChar, newChar.getClassId(), false, true, false))
        {
            Skill? skillTemplate = SkillData.getInstance().getSkill(skill.getSkillId(), skill.getSkillLevel());
            if (skillTemplate != null)
			    newChar.addSkill(skillTemplate, true);
		}

		// Register all shortcuts for actions, skills and items for this new character.
		InitialShortcutData.getInstance().registerAllShortcuts(newChar);

		if (GlobalEvents.Players.HasSubscribers<OnPlayerCreate>())
		{
			GlobalEvents.Players.Notify(new OnPlayerCreate(newChar, newChar.ObjectId, newChar.getName(), session));
		}

		newChar.setOnlineStatus(true, false);
		Disconnection.of(session, newChar).storeMe().deleteMe();
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

    private static bool CheckRaceAndClass(Race race, Sex sex, CharacterClass classId)
    {
	    switch (race)
	    {
		    case Race.HUMAN:
		    {
			    // Custom Feature: Disallow a race to be created.
			    // Example: Humans can not be created if AllowHuman = False in Custom.properties
			    if (!Config.AllowedPlayerRaces.ALLOW_HUMAN)
				    return false;

			    if (CategoryData.getInstance().isInCategory(CategoryType.DEATH_KNIGHT_ALL_CLASS, classId) &&
			        sex == Sex.Female)
				    return false;

			    if (CategoryData.getInstance().isInCategory(CategoryType.ASSASSIN_ALL_CLASS, classId) &&
			        sex == Sex.Female)
				    return false;

			    break;
		    }
		    case Race.ELF:
		    {
			    if (!Config.AllowedPlayerRaces.ALLOW_ELF)
				    return false;

			    if (CategoryData.getInstance().isInCategory(CategoryType.DEATH_KNIGHT_ALL_CLASS, classId) &&
			        sex == Sex.Female)
				    return false;

			    break;
		    }
		    case Race.DARK_ELF:
		    {
			    if (!Config.AllowedPlayerRaces.ALLOW_DARKELF)
				    return false;

			    if (CategoryData.getInstance().isInCategory(CategoryType.DEATH_KNIGHT_ALL_CLASS, classId) &&
			        sex == Sex.Female)
				    return false;

			    if (CategoryData.getInstance().isInCategory(CategoryType.ASSASSIN_ALL_CLASS, classId) &&
			        sex != Sex.Female)
				    return false;

			    break;
		    }
		    case Race.ORC:
		    {
			    if (!Config.AllowedPlayerRaces.ALLOW_ORC)
				    return false;

			    if (CategoryData.getInstance().isInCategory(CategoryType.VANGUARD_ALL_CLASS, classId) &&
			        sex == Sex.Female)
				    return false;

			    break;
		    }
		    case Race.DWARF:
		    {
			    if (!Config.AllowedPlayerRaces.ALLOW_DWARF)
				    return false;

			    break;
		    }
		    case Race.KAMAEL:
		    {
			    if (!Config.AllowedPlayerRaces.ALLOW_KAMAEL)
				    return false;

			    break;
		    }
		    case Race.SYLPH:
		    {
			    if (!Config.AllowedPlayerRaces.ALLOW_SYLPH)
				    return false;

			    break;
		    }
	    }

	    if (!Config.AllowedPlayerRaces.ALLOW_DEATH_KNIGHT &&
	        CategoryData.getInstance().isInCategory(CategoryType.DEATH_KNIGHT_ALL_CLASS, classId))
		    return false;

	    if (!Config.AllowedPlayerRaces.ALLOW_VANGUARD &&
	        CategoryData.getInstance().isInCategory(CategoryType.VANGUARD_ALL_CLASS, classId))
		    return false;

	    return true;
    }
}