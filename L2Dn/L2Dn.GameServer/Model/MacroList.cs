using System.Text;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using NLog;
using StringTokenizer = Microsoft.Extensions.Primitives.StringTokenizer;

namespace L2Dn.GameServer.Model;

public class MacroList : IRestorable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(MacroList));
	
	private readonly Player _owner;
	private int _macroId;
	private readonly Map<int, Macro> _macroses = new();
	
	public MacroList(Player owner)
	{
		_owner = owner;
		_macroId = 1000;
	}
	
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
			while (_macroses.containsKey(macro.getId()))
			{
				macro.setId(_macroId++);
			}
			_macroses.put(macro.getId(), macro);
			registerMacroInDb(macro);
		}
		else
		{
			updateType = MacroUpdateType.MODIFY;
			Macro old = _macroses.put(macro.getId(), macro);
			if (old != null)
			{
				deleteMacroFromDb(old);
			}
			registerMacroInDb(macro);
		}
		_owner.sendPacket(new SendMacroList(1, macro, updateType));
	}
	
	public void deleteMacro(int id)
	{
		Macro removed = _macroses.remove(id);
		if (removed != null)
		{
			deleteMacroFromDb(removed);
		}
		
		foreach (Shortcut sc in _owner.getAllShortCuts())
		{
			if ((sc.getId() == id) && (sc.getType() == ShortcutType.MACRO))
			{
				_owner.deleteShortCut(sc.getSlot(), sc.getPage());
			}
		}
		_owner.sendPacket(new SendMacroList(0, removed, MacroUpdateType.DELETE));
	}
	
	public void sendAllMacros()
	{
		ICollection<Macro> allMacros = _macroses.values();
		int count = allMacros.size();
		
		lock (_macroses)
		{
			if (allMacros.isEmpty())
			{
				_owner.sendPacket(new SendMacroList(0, null, MacroUpdateType.LIST));
			}
			else
			{
				foreach (Macro m in allMacros)
				{
					_owner.sendPacket(new SendMacroList(count, m, MacroUpdateType.LIST));
				}
			}
		}
	}
	
	private void registerMacroInDb(Macro macro)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			using PreparedStatement ps = con.prepareStatement(
				"INSERT INTO character_macroses (charId,id,icon,name,descr,acronym,commands) values(?,?,?,?,?,?,?)");
			ps.setInt(1, _owner.getObjectId());
			ps.setInt(2, macro.getId());
			ps.setInt(3, macro.getIcon());
			ps.setString(4, macro.getName());
			ps.setString(5, macro.getDescr());
			ps.setString(6, macro.getAcronym());
			StringBuilder sb = new StringBuilder(1255);
			foreach (MacroCmd cmd in macro.getCommands())
			{
				sb.append(cmd.getType().ordinal() + "," + cmd.getD1() + "," + cmd.getD2());
				if ((cmd.getCmd() != null) && (cmd.getCmd().length() > 0))
				{
					sb.append("," + cmd.getCmd());
				}
				sb.append(';');
			}
			
			if (sb.length() > 1000)
			{
				sb.setLength(1000);
			}
			
			ps.setString(7, sb.toString());
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("could not store macro:" + e);
		}
	}
	
	private void deleteMacroFromDb(Macro macro)
	{
		try 
		{using GameServerDbContext ctx = new();
			using PreparedStatement ps = con.prepareStatement("DELETE FROM character_macroses WHERE charId=? AND id=?");
			ps.setInt(1, _owner.getObjectId());
			ps.setInt(2, macro.getId());
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("could not delete macro:" + e);
		}
	}
	
	public bool restoreMe()
	{
		_macroses.clear();
		try
		{
			using GameServerDbContext ctx = new();
			using PreparedStatement ps = con.prepareStatement(
				"SELECT charId, id, icon, name, descr, acronym, commands FROM character_macroses WHERE charId=?");
			ps.setInt(1, _owner.getObjectId());
				using ResultSet rset = ps.executeQuery();
				while (rset.next())
				{
					int id = rset.getInt("id");
					int icon = rset.getInt("icon");
					String name = rset.getString("name");
					String descr = rset.getString("descr");
					String acronym = rset.getString("acronym");
					List<MacroCmd> commands = new ArrayList<>();
					StringTokenizer st1 = new StringTokenizer(rset.getString("commands"), ";");
					while (st1.hasMoreTokens())
					{
						StringTokenizer st = new StringTokenizer(st1.nextToken(), ",");
						if (st.countTokens() < 3)
						{
							continue;
						}
						MacroType type = MacroType.values()[Integer.parseInt(st.nextToken())];
						int d1 = Integer.parseInt(st.nextToken());
						int d2 = Integer.parseInt(st.nextToken());
						String cmd = "";
						if (st.hasMoreTokens())
						{
							cmd = st.nextToken();
						}
						commands.add(new MacroCmd(commands.size(), type, d1, d2, cmd));
					}
					_macroses.put(id, new Macro(id, icon, name, descr, acronym, commands));
				}
		}
		catch (Exception e)
		{
			LOGGER.Warn("could not store shortcuts:" + e);
			return false;
		}
		return true;
	}
}
