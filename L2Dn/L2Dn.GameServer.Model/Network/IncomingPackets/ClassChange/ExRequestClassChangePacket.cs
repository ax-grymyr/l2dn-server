using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.ClassChange;
using L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets.ClassChange;

public struct ExRequestClassChangePacket: IIncomingPacket<GameSession>
{
	private CharacterClass _classId;

	public void ReadContent(PacketBitReader reader)
    {
	    _classId = (CharacterClass)reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		// Check if class id is valid.
		bool canChange = false;
		foreach (CharacterClassInfo info in player.getClassId().GetClassInfo().getNextClassIds())
		{
			if (info.getId() == _classId)
			{
				canChange = true;
				break;
			}
		}

		if (!canChange)
		{
			PacketLogger.Instance.Warn(player + " tried to change class from " + player.getClassId() + " to " +
			                           _classId + "!");

			return ValueTask.CompletedTask;
		}

		// Check for player proper class group and level.
		canChange = false;
		int playerLevel = player.getLevel();
		if (player.isInCategory(CategoryType.FIRST_CLASS_GROUP) && playerLevel >= 20)
		{
			canChange = CategoryData.getInstance().isInCategory(CategoryType.SECOND_CLASS_GROUP, _classId);
		}
		else if (player.isInCategory(CategoryType.SECOND_CLASS_GROUP) && playerLevel >= 40)
		{
			canChange = CategoryData.getInstance().isInCategory(CategoryType.THIRD_CLASS_GROUP, _classId);
		}
		else if (player.isInCategory(CategoryType.THIRD_CLASS_GROUP) && playerLevel >= 76)
		{
			canChange = CategoryData.getInstance().isInCategory(CategoryType.FOURTH_CLASS_GROUP, _classId);
		}

		// Change class.
		if (canChange)
		{
			player.setClassId(_classId);
			if (player.isSubClassActive())
			{
				player.getSubClasses().get(player.getClassIndex())?.setClassId(player.getActiveClass());
			}
			else
			{
				player.setBaseClass(player.getActiveClass());
			}

			// Class change rewards.
			if (!Config.Character.DISABLE_TUTORIAL)
			{
				switch (player.getClassId())
				{
					case CharacterClass.KNIGHT:
					case CharacterClass.ELVEN_KNIGHT:
					case CharacterClass.PALUS_KNIGHT:
					case CharacterClass.DEATH_BLADE_HUMAN:
					case CharacterClass.DEATH_BLADE_ELF:
					case CharacterClass.DEATH_BLADE_DARK_ELF:
					{
						player.addItem("ExRequestClassChange", 93028, 1, player, true); // Aden Sword.
						player.addItem("ExRequestClassChange", 93493, 1, player, true); // Moon Armor Set.
						player.addItem("ExRequestClassChange", 93496, 1, player, true); // 1st Class Transfer Gift Box.
						break;
					}

					case CharacterClass.WARRIOR:
					{
						player.addItem("ExRequestClassChange", 93028, 1, player, true); // Aden Sword.
						player.addItem("ExRequestClassChange", 93034, 1, player, true); // Aden Spear.
						player.addItem("ExRequestClassChange", 93493, 1, player, true); // Moon Armor Set.
						player.addItem("ExRequestClassChange", 93496, 1, player, true); // 1st Class Transfer Gift Box.
						break;
					}

					case CharacterClass.ROGUE:
					case CharacterClass.ELVEN_SCOUT:
					case CharacterClass.ASSASSIN:
					{
						player.addItem("ExRequestClassChange", 93029, 1, player, true); // Aden Dagger.
						player.addItem("ExRequestClassChange", 93030, 1, player, true); // Aden Bow.
						player.addItem("ExRequestClassChange", 1341, 2000, player, true); // Bone Arrow.
						player.addItem("ExRequestClassChange", 93494, 1, player, true); // Moon Shell Set.
						player.addItem("ExRequestClassChange", 93496, 1, player, true); // 1st Class Transfer Gift Box.
						break;
					}

					case CharacterClass.WIZARD:
					case CharacterClass.CLERIC:
					case CharacterClass.ELVEN_WIZARD:
					case CharacterClass.ORACLE:
					case CharacterClass.DARK_WIZARD:
					case CharacterClass.SHILLIEN_ORACLE:
					case CharacterClass.ORC_SHAMAN:
					{
						player.addItem("ExRequestClassChange", 93033, 1, player, true); // Two-Handed Blunt Weapon of Aden.
						player.addItem("ExRequestClassChange", 93495, 1, player, true); // Moon Cape Set.
						player.addItem("ExRequestClassChange", 93496, 1, player, true); // 1st Class Transfer Gift Box.
						break;
					}

					case CharacterClass.ORC_RAIDER:
					{
						player.addItem("ExRequestClassChange", 93032, 1, player, true); // Two-handed Sword of Aden.
						player.addItem("ExRequestClassChange", 93493, 1, player, true); // Moon Armor Set.
						player.addItem("ExRequestClassChange", 93497, 1, player, true); // 1st Class Transfer Gift Box.
						break;
					}

					case CharacterClass.ORC_MONK:
					{
						player.addItem("ExRequestClassChange", 93035, 1, player, true); // Aden Fist.
						player.addItem("ExRequestClassChange", 93493, 1, player, true); // Moon Armor Set.
						player.addItem("ExRequestClassChange", 93497, 1, player, true); // 1st Class Transfer Gift Box.
						break;
					}

					case CharacterClass.ARTISAN:
					case CharacterClass.SCAVENGER:
					{
						player.addItem("ExRequestClassChange", 93031, 1, player, true); // Aden Club.
						player.addItem("ExRequestClassChange", 93034, 1, player, true); // Aden Spear.
						player.addItem("ExRequestClassChange", 93493, 1, player, true); // Moon Armor Set.
						player.addItem("ExRequestClassChange", 93496, 1, player, true); // 1st Class Transfer Gift Box.
						break;
					}

					case CharacterClass.TROOPER:
					{
						player.addItem("ExRequestClassChange", 93037, 1, player, true); // Aden Ancient Sword.
						player.addItem("ExRequestClassChange", 93494, 1, player, true); // Moon Shell Set.
						player.addItem("ExRequestClassChange", 93496, 1, player, true); // 1st Class Transfer Gift Box.
						break;
					}

					case CharacterClass.WARDER:
					{
						player.addItem("ExRequestClassChange", 93030, 1, player, true); // Aden Bow.
						player.addItem("ExRequestClassChange", 1341, 2000, player, true); // Bone Arrow.
						player.addItem("ExRequestClassChange", 93494, 1, player, true); // Moon Shell Set.
						player.addItem("ExRequestClassChange", 93496, 1, player, true); // 1st Class Transfer Gift Box.
						break;
					}

					case CharacterClass.SOUL_FINDER:
					{
						player.addItem("ExRequestClassChange", 93036, 1, player, true); // Aden Rapier.
						player.addItem("ExRequestClassChange", 93494, 1, player, true); // Moon Shell Set.
						player.addItem("ExRequestClassChange", 93496, 1, player, true); // 1st Class Transfer Gift Box.
						break;
					}

					case CharacterClass.SHARPSHOOTER:
					{
						player.addItem("ExRequestClassChange", 94892, 1, player, true); // D-Grade Elemental Orb Sealed.
						player.addItem("ExRequestClassChange", 94897, 1, player, true); // Aden Pistols
						player.addItem("ExRequestClassChange", 93494, 1, player, true); // Moon Shell Set.
						player.addItem("ExRequestClassChange", 93496, 1, player, true); // 1st Class Transfer Gift Box.
						break;
					}

					case CharacterClass.RIDER:
					{
						player.addItem("ExRequestClassChange", 93034, 1, player, true); // Aden Spear.
						player.addItem("ExRequestClassChange", 93493, 1, player, true); // Moon Armor Set.
						player.addItem("ExRequestClassChange", 93496, 1, player, true); // 1st Class Transfer Gift Box.
						break;
					}

					case CharacterClass.ASSASSIN_MALE_1:
					case CharacterClass.ASSASSIN_FEMALE_1:
					{
						player.addItem("ExRequestClassChange", 94998, 1, player, true); // Maingauche.
						player.addItem("ExRequestClassChange", 93494, 1, player, true); // Moon Shell Set.
						player.addItem("ExRequestClassChange", 93496, 1, player, true); // 1st Class Transfer Gift Box.
						break;
					}

					case CharacterClass.ASSASSIN_FEMALE_3:
					case CharacterClass.ASSASSIN_MALE_3:
					{
						player.setAssassinationPoints(1);
						break;
					}
				}
			}

			// Elemental Spirits.
			if (player.isInCategory(CategoryType.THIRD_CLASS_GROUP))
            {
                ElementalSpirit[]? spirits = player.getSpirits();
				if (spirits == null)
				{
					player.initElementalSpirits();
                    spirits = player.getSpirits() ?? throw new InvalidOperationException("Cannot initialize spirits");
                }

                foreach (ElementalSpirit spirit in spirits)
				{
					if (spirit.getStage() == 0)
						spirit.upgrade();
				}

                if (!player.isSubclassLocked())
                {
                    UserInfoPacket userInfo = new UserInfoPacket(player);
                    userInfo.AddComponentType(UserInfoType.ATT_SPIRITS);
                    player.sendPacket(userInfo);
                }

                player.sendPacket(new ElementalSpiritInfoPacket(player, 0));
				player.sendPacket(new ExElementalSpiritAttackTypePacket(player));
			}

			if (Config.Character.AUTO_LEARN_SKILLS)
			{
				player.giveAvailableSkills(Config.Character.AUTO_LEARN_FS_SKILLS, true, Config.Character.AUTO_LEARN_SKILLS_WITHOUT_ITEMS);
			}

			player.store(false); // Save player cause if server crashes before this char is saved, he will lose class.
			player.broadcastUserInfo();
			player.sendSkillList();
			player.sendPacket(new PlaySoundPacket("ItemSound.quest_fanfare_2"));

			if (Config.Character.DISABLE_TUTORIAL && !player.isInCategory(CategoryType.FOURTH_CLASS_GROUP) //
				&& ((player.isInCategory(CategoryType.SECOND_CLASS_GROUP) && playerLevel >= 38) //
					|| (player.isInCategory(CategoryType.THIRD_CLASS_GROUP) && playerLevel >= 76)))
			{
				player.sendPacket(ExClassChangeSetAlarmPacket.STATIC_PACKET);
			}
		}

		return ValueTask.CompletedTask;
    }
}