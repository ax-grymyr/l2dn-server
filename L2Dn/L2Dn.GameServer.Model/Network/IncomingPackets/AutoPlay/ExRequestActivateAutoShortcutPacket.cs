using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Templates;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets.AutoPlay;

public struct ExRequestActivateAutoShortcutPacket: IIncomingPacket<GameSession>
{
	private int _slot;
	private int _page;
	private bool _active;

	public void ReadContent(PacketBitReader reader)
	{
		int position = reader.ReadInt16();
		_slot = position % ShortCuts.MAX_SHORTCUTS_PER_BAR;
		_page = position / ShortCuts.MAX_SHORTCUTS_PER_BAR;
		_active = reader.ReadByte() == 1;
	}

	public ValueTask ProcessAsync(Connection connection, GameSession session)
	{
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		Shortcut? shortcut = player.getShortCut(_slot, _page);
		if (shortcut == null)
			return ValueTask.CompletedTask;

		if (_active)
			player.addAutoShortcut(_slot, _page);
		else
			player.removeAutoShortcut(_slot, _page);

		Item? item = null;
		Skill? skill = null;
		if (shortcut.getType() == ShortcutType.SKILL)
		{
			int skillId = player.getReplacementSkill(shortcut.getId());
			skill = player.getKnownSkill(skillId);
			if (skill == null)
			{
				if (player.hasServitors())
				{
					foreach (Summon summon in player.getServitors().Values)
					{
						skill = summon.getKnownSkill(skillId);
						if (skill != null)
						{
							break;
						}
					}
				}

                Pet? pet = player.getPet();
				if (skill == null && player.hasPet() && pet != null)
				{
					skill = pet.getKnownSkill(skillId);
				}
			}
		}
		else
		{
			item = player.getInventory().getItemByObjectId(shortcut.getId());
		}

		// stop
		if (!_active)
		{
			if (item != null)
			{
				// auto supply
				if (!item.isPotion())
				{
					AutoUseTaskManager.getInstance().removeAutoSupplyItem(player, item.Id);
				}
				else // auto potion
				{
					AutoUseTaskManager.getInstance().removeAutoPotionItem(player);
					AutoUseTaskManager.getInstance().removeAutoPetPotionItem(player);
				}
			}

			// auto skill
			if (skill != null)
			{
				if (skill.IsBad)
				{
					AutoUseTaskManager.getInstance().removeAutoSkill(player, skill.Id);
				}
				else
				{
					AutoUseTaskManager.getInstance().removeAutoBuff(player, skill.Id);
				}
			}
			else // action
			{
				AutoUseTaskManager.getInstance().removeAutoAction(player, shortcut.getId());
			}

			return ValueTask.CompletedTask;
		}

		// start
		if (item != null && !item.isPotion())
		{
			// auto supply
			if (Config.General.ENABLE_AUTO_ITEM)
			{
				AutoUseTaskManager.getInstance().addAutoSupplyItem(player, item.Id);
			}
		}
		else
		{
			// auto potion
			if (_page == 23)
			{
				if (_slot == 1)
				{
					if (Config.General.ENABLE_AUTO_POTION && item != null && item.isPotion())
					{
						AutoUseTaskManager.getInstance().setAutoPotionItem(player, item.Id);
						return ValueTask.CompletedTask;
					}
				}
				else if (_slot == 2)
				{
					if (Config.General.ENABLE_AUTO_PET_POTION && item != null && item.isPotion())
					{
						AutoUseTaskManager.getInstance().setAutoPetPotionItem(player, item.Id);
						return ValueTask.CompletedTask;
					}
				}
			}

			// auto skill
			if (Config.General.ENABLE_AUTO_SKILL && skill != null)
			{
				if (skill.IsBad)
				{
					AutoUseTaskManager.getInstance().addAutoSkill(player, skill.Id);
				}
				else
				{
					AutoUseTaskManager.getInstance().addAutoBuff(player, skill.Id);
				}

				return ValueTask.CompletedTask;
			}

			// action
			AutoUseTaskManager.getInstance().addAutoAction(player, shortcut.getId());
		}

		return ValueTask.CompletedTask;
	}
}