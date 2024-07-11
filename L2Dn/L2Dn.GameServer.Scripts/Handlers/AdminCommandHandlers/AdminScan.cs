using L2Dn.GameServer.Data;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Html.Formatters;
using L2Dn.GameServer.Model.Html.PageHandlers;
using L2Dn.GameServer.Model.Html.Styles;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author NosBit
 */
public class AdminScan: IAdminCommandHandler
{
	private const string SPACE = " ";
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_scan",
		"admin_deleteNpcByObjectId"
	};
	
	private static int DEFAULT_RADIUS = 1000;
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command, " ");
		string actualCommand = st.nextToken();
		switch (actualCommand.toLowerCase())
		{
			case "admin_scan":
			{
				processBypass(activeChar, new BypassParser(command));
				break;
			}
			case "admin_deletenpcbyobjectid":
			{
				if (!st.hasMoreElements())
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: //deletenpcbyobjectid objectId=<object_id>");
					return false;
				}
				
				BypassParser parser = new BypassParser(command);
				try
				{
					int objectId = parser.getInt("objectId", 0);
					if (objectId == 0)
					{
						BuilderUtil.sendSysMessage(activeChar, "objectId is not set!");
					}
					
					WorldObject target = World.getInstance().findObject(objectId);
					Npc npc = target is Npc ? (Npc) target : null;
					if (npc == null)
					{
						BuilderUtil.sendSysMessage(activeChar, "NPC does not exist or object_id does not belong to an NPC");
						return false;
					}
					
					npc.deleteMe();
					
					Spawn spawn = npc.getSpawn();
					if (spawn != null)
					{
						spawn.stopRespawn();
						
						if (DbSpawnManager.getInstance().isDefined(spawn.getId()))
						{
							DbSpawnManager.getInstance().deleteSpawn(spawn, true);
						}
						else
						{
							SpawnTable.getInstance().deleteSpawn(spawn, true);
						}
					}
					
					activeChar.sendMessage(npc.getName() + " have been deleted.");
				}
				catch (FormatException e)
				{
					BuilderUtil.sendSysMessage(activeChar, "object_id must be a number.");
					return false;
				}
				
				processBypass(activeChar, parser);
				break;
			}
		}
		return true;
	}
	
	private void processBypass(Player activeChar, BypassParser parser)
	{
		int id = parser.getInt("id", 0);
		string name = parser.getString("name", null);
		int radius = parser.getInt("radius", parser.getInt("range", DEFAULT_RADIUS));
		int page = parser.getInt("page", 0);
		Predicate<Npc> condition;
		if (id > 0)
		{
			condition = npc => npc.getId() == id;
		}
		else if (name != null)
		{
			condition = npc => npc.getName().toLowerCase().startsWith(name.toLowerCase());
		}
		else
		{
			condition = npc => true;
		}
		
		sendNpcList(activeChar, radius, page, condition, parser);
	}
	
	private BypassBuilder createBypassBuilder(BypassParser parser, string bypass)
	{
		int id = parser.getInt("id", 0);
		string name = parser.getString("name", null);
		int radius = parser.getInt("radius", parser.getInt("range", DEFAULT_RADIUS));
		BypassBuilder builder = new BypassBuilder(bypass);
		if (id > 0)
		{
			builder.addParam("id", id);
		}
		else if (name != null)
		{
			builder.addParam("name", name);
		}
		
		if (radius > DEFAULT_RADIUS)
		{
			builder.addParam("radius", radius);
		}
		return builder;
	}
	
	private void sendNpcList(Player activeChar, int radius, int page, Predicate<Npc> condition, BypassParser parser)
	{
		BypassBuilder bypassParser = createBypassBuilder(parser, "bypass -h admin_scan");
		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/scan.htm", activeChar);
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
		
		//@formatter:off
		PageResult result = PageBuilder.newBuilder(World.getInstance().getVisibleObjectsInRange<Npc>(activeChar, radius, condition), 15, bypassParser.ToString())
			.currentPage(page)
			.pageHandler(NextPrevPageHandler.INSTANCE)
			.formatter(BypassParserFormatter.INSTANCE)
			.style(ButtonsStyle.INSTANCE)
			.bodyHandler((pages, character, sb) =>
		{
			BypassBuilder builder = createBypassBuilder(parser, "bypass -h admin_deleteNpcByObjectId");
			string npcName = character.getName();
			builder.addParam("page", page);
			builder.addParam("objectId", character.getObjectId());
			sb.Append("<tr>");
			sb.Append("<td width=\"45\">").Append(character.getId()).Append("</td>");
			sb.Append("<td><a action=\"bypass -h admin_move_to ").Append(character.getX()).Append(SPACE).Append(character.getY()).Append(SPACE).Append(character.getZ()).Append("\">").Append(string.IsNullOrEmpty(npcName) ? "No name NPC" : npcName).Append("</a></td>");
			sb.Append("<td width=\"60\">").Append(Util.formatAdena((long)Math.Round(activeChar.Distance2D(character)))).Append("</td>");
			sb.Append("<td width=\"54\"><a action=\"").Append(builder.toStringBuilder()).Append("\"><font color=\"LEVEL\">Delete</font></a></td>");
			sb.Append("</tr>");
		}).build();
		//@formatter:on
		
		if (result.getPages() > 0)
		{
			htmlContent.Replace("%pages%", "<center><table width=\"100%\" cellspacing=0><tr>" + result.getPagerTemplate() + "</tr></table></center>");
		}
		else
		{
			htmlContent.Replace("%pages%", "");
		}
		
		htmlContent.Replace("%data%", result.getBodyTemplate().ToString());
		activeChar.sendPacket(html);
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
