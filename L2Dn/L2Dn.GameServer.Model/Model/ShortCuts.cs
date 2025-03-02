using System.Runtime.CompilerServices;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.Model;

public class ShortCuts : IRestorable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ShortCuts));

	public const int MAX_SHORTCUTS_PER_BAR = 12;

	private readonly Player _owner;
	private readonly Map<int, Shortcut> _shortCuts = new();

	public ShortCuts(Player owner)
	{
		_owner = owner;
	}

	public ICollection<Shortcut> getAllShortCuts()
	{
		return _shortCuts.Values;
	}

	public Shortcut? getShortCut(int slot, int page)
	{
		Shortcut? sc = _shortCuts.get(slot + page * MAX_SHORTCUTS_PER_BAR);
		// Verify shortcut
		if (sc != null && sc.getType() == ShortcutType.ITEM && _owner.getInventory().getItemByObjectId(sc.getId()) == null)
		{
			deleteShortCut(sc.getSlot(), sc.getPage());
			sc = null;
		}

		return sc;
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void registerShortCut(Shortcut shortcut)
	{
		// Verify shortcut
		if (shortcut.getType() == ShortcutType.ITEM)
		{
			Item? item = _owner.getInventory().getItemByObjectId(shortcut.getId());
			if (item == null)
				return;

            shortcut.setSharedReuseGroup(item.getSharedReuseGroup());
		}

		registerShortCutInDb(shortcut, _shortCuts.put(shortcut.getSlot() + shortcut.getPage() * MAX_SHORTCUTS_PER_BAR, shortcut));
	}

	private void registerShortCutInDb(Shortcut shortcut, Shortcut? oldShortCut)
	{
		if (oldShortCut != null)
		{
			deleteShortCutFromDb(oldShortCut);
		}

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = _owner.ObjectId;
			int classIndex = _owner.getClassIndex();
			int slot = shortcut.getSlot();
			int page = shortcut.getPage();
			var record = ctx.CharacterShortCuts.SingleOrDefault(r =>
				r.CharacterId == characterId && r.ClassIndex == classIndex && r.Page == page && r.Slot == slot);
			if (record == null)
			{
				record = new DbCharacterShortCut();
				record.CharacterId = characterId;
				record.ClassIndex = (byte)classIndex;
				record.Slot = (byte)slot;
				record.Page = (byte)page;
				ctx.CharacterShortCuts.Add(record);
			}

			record.Type = (byte)shortcut.getType();
			record.ShortCutId = shortcut.getId();
			record.SkillLevel = (short)shortcut.getLevel();
			record.SkillSubLevel = (short)shortcut.getSubLevel();
			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not store character shortcut: " + e);
		}
	}

	/**
	 * @param slot
	 * @param page
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void deleteShortCut(int slot, int page)
	{
		Shortcut? old = _shortCuts.remove(slot + page * MAX_SHORTCUTS_PER_BAR);
		if (old == null || _owner == null)
			return;

        deleteShortCutFromDb(old);
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void deleteShortCutByObjectId(int objectId)
	{
		foreach (Shortcut shortcut in _shortCuts.Values)
		{
			if (shortcut.getType() == ShortcutType.ITEM && shortcut.getId() == objectId)
			{
				deleteShortCut(shortcut.getSlot(), shortcut.getPage());
				break;
			}
		}
	}

	/**
	 * @param shortcut
	 */
	private void deleteShortCutFromDb(Shortcut shortcut)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = _owner.ObjectId;
			int slot = shortcut.getSlot();
			int page = shortcut.getPage();
			int classIndex = _owner.getClassIndex();
			ctx.CharacterShortCuts.Where(r =>
					r.CharacterId == characterId && r.Slot == slot && r.Page == page && r.ClassIndex == classIndex)
				.ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not delete character shortcut: " + e);
		}
	}

	public bool restoreMe()
	{
		_shortCuts.Clear();
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = _owner.ObjectId;
			int classIndex = _owner.getClassIndex();
			var query = ctx.CharacterShortCuts.Where(r => r.CharacterId == characterId && r.ClassIndex == classIndex);
			foreach (var record in query)
			{
				int slot = record.Slot;
				int page = record.Page;
				ShortcutType type = (ShortcutType)record.Type;
				int id = record.ShortCutId;
				int level = record.SkillLevel;
				int subLevel = record.SkillSubLevel;
				_shortCuts.put(slot + page * MAX_SHORTCUTS_PER_BAR, new Shortcut(slot, page, type, id, level, subLevel, 1));
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not restore character shortcuts: " + e);
			return false;
		}

		// Verify shortcuts
		foreach (Shortcut sc in getAllShortCuts())
		{
			if (sc.getType() == ShortcutType.ITEM)
			{
				Item? item = _owner.getInventory().getItemByObjectId(sc.getId());
				if (item == null)
				{
					deleteShortCut(sc.getSlot(), sc.getPage());
				}
				else if (item.getEtcItem() is {} etcItem)
				{
					sc.setSharedReuseGroup(etcItem.getSharedReuseGroup());
				}
			}
		}

		return true;
	}

	/**
	 * Updates the shortcut bars with the new skill.
	 * @param skillId the skill Id to search and update.
	 * @param skillLevel the skill level to update.
	 * @param skillSubLevel the skill sub level to update.
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void updateShortCuts(int skillId, int skillLevel, int skillSubLevel)
	{
		// Update all the shortcuts for this skill
		foreach (Shortcut sc in _shortCuts.Values)
		{
			if (sc.getId() == skillId && sc.getType() == ShortcutType.SKILL)
			{
				Shortcut newsc = new Shortcut(sc.getSlot(), sc.getPage(), sc.getType(), sc.getId(), skillLevel, skillSubLevel, 1);
				newsc.setAutoUse(sc.isAutoUse());
				_owner.sendPacket(new ShortCutRegisterPacket(newsc, _owner));
				_owner.registerShortCut(newsc);
			}
		}
	}
}