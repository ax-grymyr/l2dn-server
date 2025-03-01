using System.Text;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.Model;

public class MacroList(Player owner): IRestorable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(MacroList));

    private int _macroId = 1000;
	private readonly Map<int, Macro> _macroses = new();

    public Map<int, Macro> getAllMacroses()
	{
		return _macroses;
	}

	public void registerMacro(Macro macro)
	{
		MacroUpdateType updateType = MacroUpdateType.ADD;
		if (macro.getId() == 0)
		{
			macro.setId(_macroId++);
			while (_macroses.ContainsKey(macro.getId()))
			{
				macro.setId(_macroId++);
			}
			_macroses.put(macro.getId(), macro);
			registerMacroInDb(macro);
		}
		else
		{
			updateType = MacroUpdateType.MODIFY;
			Macro? old = _macroses.put(macro.getId(), macro);
			if (old != null)
			{
				deleteMacroFromDb(old);
			}
			registerMacroInDb(macro);
		}
		owner.sendPacket(new SendMacroListPacket(1, macro, updateType));
	}

	public void deleteMacro(int id)
	{
		Macro? removed = _macroses.remove(id);
		if (removed != null)
		{
			deleteMacroFromDb(removed);
		}

		foreach (Shortcut sc in owner.getAllShortCuts())
		{
			if (sc.getId() == id && sc.getType() == ShortcutType.MACRO)
			{
				owner.deleteShortCut(sc.getSlot(), sc.getPage());
			}
		}
		owner.sendPacket(new SendMacroListPacket(0, removed, MacroUpdateType.DELETE));
	}

	public void sendAllMacros()
	{
		ICollection<Macro> allMacros = _macroses.Values;
		int count = allMacros.Count;

		lock (_macroses)
		{
			if (allMacros.Count == 0)
			{
				owner.sendPacket(new SendMacroListPacket(0, null, MacroUpdateType.LIST));
			}
			else
			{
				foreach (Macro m in allMacros)
				{
					owner.sendPacket(new SendMacroListPacket(count, m, MacroUpdateType.LIST));
				}
			}
		}
	}

	private void registerMacroInDb(Macro macro)
	{
		try
		{
			StringBuilder sb = new StringBuilder(1255);
			foreach (MacroCmd cmd in macro.getCommands())
			{
				sb.Append((int)cmd.getType() + "," + cmd.getD1() + "," + cmd.getD2());
				if (cmd.getCmd() != null && cmd.getCmd().Length > 0)
				{
					sb.Append("," + cmd.getCmd());
				}
				sb.Append(';');
			}

			string text = sb.Length > 1000 ? sb.ToString(0, 1000) : sb.ToString();

			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CharacterMacros.Add(new DbCharacterMacros()
			{
				CharacterId = owner.ObjectId,
				Id = macro.getId(),
				Icon = macro.getIcon(),
				Name = macro.getName(),
				Description = macro.getDescr(),
				Acronym = macro.getAcronym(), Commands = text,
			});

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("could not store macro:" + e);
		}
	}

	private void deleteMacroFromDb(Macro macro)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = owner.ObjectId;
			int macroId = macro.getId();
			ctx.CharacterMacros.Where(r => r.CharacterId == characterId && r.Id == macroId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("could not delete macro:" + e);
		}
	}

	public bool restoreMe()
	{
		_macroses.Clear();
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = owner.ObjectId;
			var query = ctx.CharacterMacros.Where(r => r.CharacterId == characterId);
			foreach (var record in query)
			{
				int id = record.Id;
				int? icon = record.Icon;
				string name = record.Name;
				string descr = record.Description;
				string acronym = record.Acronym;
				List<MacroCmd> commands = new();
				StringTokenizer st1 = new StringTokenizer(record.Commands, ";");
				while (st1.hasMoreTokens())
				{
					StringTokenizer st = new StringTokenizer(st1.nextToken(), ",");
					if (st.countTokens() < 3)
					{
						continue;
					}

					MacroType type = (MacroType)int.Parse(st.nextToken());
					int d1 = int.Parse(st.nextToken());
					int d2 = int.Parse(st.nextToken());
					string cmd = "";
					if (st.hasMoreTokens())
					{
						cmd = st.nextToken();
					}

					commands.Add(new MacroCmd(commands.Count, type, d1, d2, cmd));
				}

				_macroses.put(id, new Macro(id, icon, name, descr, acronym, commands));
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("could not read macros:" + e);
			return false;
		}

		return true;
	}
}