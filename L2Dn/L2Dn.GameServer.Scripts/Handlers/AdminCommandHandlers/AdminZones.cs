using System.Text;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Annotations;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Model.Zones.Forms;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author UnAfraid
 */
public class AdminZones: AbstractScript, IAdminCommandHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(AdminZones));
	private static readonly string[] COMMANDS = ["admin_zones"];

	private readonly Map<int, ZoneNodeHolder> _zones = new();
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command);
		string cmd = st.nextToken();
		switch (cmd)
		{
			case "admin_zones":
			{
				if (!st.hasMoreTokens())
				{
					buildZonesEditorWindow(activeChar);
					return false;
				}
				string subCmd = st.nextToken();
				switch (subCmd)
				{
					case "load":
					{
						if (st.hasMoreTokens())
						{
							string name = "";
							while (st.hasMoreTokens())
							{
								name += st.nextToken() + " ";
							}
							loadZone(activeChar, name.Trim());
						}
						break;
					}
					case "create":
					{
						buildHtmlWindow(activeChar, 0);
						break;
					}
					case "setname":
					{
						string name = "";
						while (st.hasMoreTokens())
						{
							name += st.nextToken() + " ";
						}
						if (!string.IsNullOrEmpty(name))
						{
							name = name.Substring(0, name.Length - 1);
						}
						setName(activeChar, name);
						break;
					}
					case "start":
					{
						enablePicking(activeChar);
						break;
					}
					case "finish":
					{
						disablePicking(activeChar);
						break;
					}
					case "setMinZ":
					{
						if (st.hasMoreTokens())
						{
							int minZ = int.Parse(st.nextToken());
							setMinZ(activeChar, minZ);
						}
						break;
					}
					case "setMaxZ":
					{
						if (st.hasMoreTokens())
						{
							int maxZ = int.Parse(st.nextToken());
							setMaxZ(activeChar, maxZ);
						}
						break;
					}
					case "show":
					{
						showPoints(activeChar);
						ConfirmDialogPacket dlg = new ConfirmDialogPacket(
							"When enable show territory you must restart client to remove it, are you sure about that?",
							15 * 1000);
						activeChar.sendPacket(dlg);
						activeChar.addAction(PlayerAction.ADMIN_SHOW_TERRITORY);
						break;
					}
					case "hide":
					{
						ZoneNodeHolder holder = _zones.get(activeChar.getObjectId());
						if (holder != null)
						{
							ExServerPrimitivePacket exsp = new ExServerPrimitivePacket(
								"DebugPoint_" + activeChar.getObjectId(), activeChar.getX(), activeChar.getY(),
								activeChar.getZ());
							
							exsp.addPoint(Colors.Black, 0, 0, 0);
							activeChar.sendPacket(exsp);
						}
						break;
					}
					case "change":
					{
						if (!st.hasMoreTokens())
						{
							BuilderUtil.sendSysMessage(activeChar, "Missing node index!");
							break;
						}
						string indexToken = st.nextToken();
						if (!Util.isDigit(indexToken))
						{
							BuilderUtil.sendSysMessage(activeChar, "Node index should be int!");
							break;
						}
						int index = int.Parse(indexToken);
						changePoint(activeChar, index);
						break;
					}
					case "delete":
					{
						if (!st.hasMoreTokens())
						{
							BuilderUtil.sendSysMessage(activeChar, "Missing node index!");
							break;
						}
						string indexToken = st.nextToken();
						if (!Util.isDigit(indexToken))
						{
							BuilderUtil.sendSysMessage(activeChar, "Node index should be int!");
							break;
						}
						int index = int.Parse(indexToken);
						deletePoint(activeChar, index);
						showPoints(activeChar);
						break;
					}
					case "clear":
					{
						_zones.remove(activeChar.getObjectId());
						break;
					}
					case "dump":
					{
						dumpPoints(activeChar);
						break;
					}
					case "list":
					{
						int page = CommonUtil.parseNextInt(st, 0);
						buildHtmlWindow(activeChar, page);
						return false;
					}
				}
				break;
			}
		}
		
		buildHtmlWindow(activeChar, 0);
		return false;
 	}
	
	/**
	 * @param activeChar
	 * @param minZ
	 */
	private void setMinZ(Player activeChar, int minZ)
	{
		_zones.computeIfAbsent(activeChar.getObjectId(), key => new ZoneNodeHolder(activeChar)).setMinZ(minZ);
	}
	
	/**
	 * @param activeChar
	 * @param maxZ
	 */
	private void setMaxZ(Player activeChar, int maxZ)
	{
		_zones.computeIfAbsent(activeChar.getObjectId(), key => new ZoneNodeHolder(activeChar)).setMaxZ(maxZ);
	}
	
	private void buildZonesEditorWindow(Player activeChar)
	{
		StringBuilder sb = new StringBuilder();
		List<ZoneType> zones = ZoneManager.getInstance().getZones(activeChar.Location.Location3D);
		foreach (ZoneType zone in zones)
		{
			if (zone.getZone() is ZoneNPoly)
			{
				sb.Append("<tr>");
				sb.Append("<td fixwidth=200><a action=\"bypass -h admin_zones load " + zone.getName() + "\">" + zone.getName() + "</a></td>");
				sb.Append("</tr>");
			}
		}

		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/zone_editor.htm", activeChar);
		NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(null, 1, htmlContent);
		htmlContent.Replace("%zones%", sb.ToString());
		activeChar.sendPacket(msg);
	}
	
	/**
	 * @param activeChar
	 * @param zoneName
	 */
	private void loadZone(Player activeChar, string zoneName)
	{
		BuilderUtil.sendSysMessage(activeChar, "Searching for zone: " + zoneName);
		List<ZoneType> zones = ZoneManager.getInstance().getZones(activeChar.Location.Location3D);
		ZoneType zoneType = null;
		foreach (ZoneType zone in zones)
		{
			if (zone.getName().equalsIgnoreCase(zoneName))
			{
				zoneType = zone;
				BuilderUtil.sendSysMessage(activeChar, "Zone found: " + zone.getId());
				break;
			}
		}
		
		if (zoneType != null && zoneType.getZone() is ZoneNPoly)
		{
			ZoneNPoly zone = (ZoneNPoly) zoneType.getZone();
			ZoneNodeHolder holder = _zones.computeIfAbsent(activeChar.getObjectId(), val => new ZoneNodeHolder(activeChar));
			holder.getNodes().Clear();
			holder.setName(zoneType.getName());
			holder.setMinZ(zone.getLowZ());
			holder.setMaxZ(zone.getHighZ());
			for (int i = 0; i < zone.getX().Length; i++)
			{
				int x = zone.getX()[i];
				int y = zone.getY()[i];
				holder.addNode(new Location3D(x, y,
					GeoEngine.getInstance().getHeight(new Location3D(x, y, Rnd.get(zone.getLowZ(), zone.getHighZ())))));
			}
			showPoints(activeChar);
		}
	}
	
	/**
	 * @param activeChar
	 * @param name
	 */
	private void setName(Player activeChar, string name)
	{
		if (name.contains("<") || name.contains(">") || name.contains("&") || name.contains("\\") || name.contains("\"") || name.contains("$"))
		{
			BuilderUtil.sendSysMessage(activeChar, "You cannot use symbols like: < > & \" $ \\");
			return;
		}
		_zones.computeIfAbsent(activeChar.getObjectId(), key => new ZoneNodeHolder(activeChar)).setName(name);
	}
	
	/**
	 * @param activeChar
	 */
	private void enablePicking(Player activeChar)
	{
		if (!activeChar.hasAction(PlayerAction.ADMIN_POINT_PICKING))
		{
			activeChar.addAction(PlayerAction.ADMIN_POINT_PICKING);
			BuilderUtil.sendSysMessage(activeChar, "Point picking mode activated!");
		}
		else
		{
			BuilderUtil.sendSysMessage(activeChar, "Point picking mode is already activated!");
		}
	}
	
	/**
	 * @param activeChar
	 */
	private void disablePicking(Player activeChar)
	{
		if (activeChar.removeAction(PlayerAction.ADMIN_POINT_PICKING))
		{
			BuilderUtil.sendSysMessage(activeChar, "Point picking mode deactivated!");
		}
		else
		{
			BuilderUtil.sendSysMessage(activeChar, "Point picking mode was not activated!");
		}
	}
	
	/**
	 * @param activeChar
	 */
	private void showPoints(Player activeChar)
	{
		ZoneNodeHolder holder = _zones.get(activeChar.getObjectId());
		if (holder != null)
		{
			if (holder.getNodes().Count < 3)
			{
				BuilderUtil.sendSysMessage(activeChar, "In order to visualize this zone you must have at least 3 points.");
				return;
			}

			ExServerPrimitivePacket exsp = new ExServerPrimitivePacket("DebugPoint_" + activeChar.getObjectId(),
				activeChar.getX(), activeChar.getY(), activeChar.getZ());

			Location3D prevLoc;
			Location3D nextLoc;
			
			List<Location3D> list = holder.getNodes();
			for (int i = 1; i < list.Count; i++)
			{
				prevLoc = list[i - 1];
				nextLoc = list[i];
				if (holder.getMinZ() != 0)
				{
					exsp.addLine("Min Point " + i + " > " + (i + 1), Colors.CYAN, true, prevLoc.X, prevLoc.Y, holder.getMinZ(), nextLoc.X, nextLoc.Y, holder.getMinZ());
				}
				exsp.addLine("Point " + i + " > " + (i + 1), Colors.White, true, prevLoc.X, prevLoc.Y, prevLoc.Z, nextLoc.X, nextLoc.Y, nextLoc.Z);
				if (holder.getMaxZ() != 0)
				{
					exsp.addLine("Max Point " + i + " > " + (i + 1), Colors.RED, true, prevLoc.X, prevLoc.Y, holder.getMaxZ(), nextLoc.X, nextLoc.Y, holder.getMaxZ());
				}
			}
			
			prevLoc = list[^1];
			nextLoc = list[0];
			if (holder.getMinZ() != 0)
			{
				exsp.addLine("Min Point " + list.Count + " > 1", Colors.CYAN, true, prevLoc.X, prevLoc.Y, holder.getMinZ(), nextLoc.X, nextLoc.Y, holder.getMinZ());
			}
			
			exsp.addLine("Point " + list.Count + " > 1", Colors.White, true, prevLoc.X, prevLoc.Y, prevLoc.Z, nextLoc.X, nextLoc.Y, nextLoc.Z);
			if (holder.getMaxZ() != 0)
			{
				exsp.addLine("Max Point " + list.Count + " > 1", Colors.RED, true, prevLoc.X, prevLoc.Y, holder.getMaxZ(), nextLoc.X, nextLoc.Y, holder.getMaxZ());
			}
			
			activeChar.sendPacket(exsp);
		}
	}
	
	/**
	 * @param activeChar
	 * @param index
	 */
	private void changePoint(Player activeChar, int index)
	{
		ZoneNodeHolder holder = _zones.get(activeChar.getObjectId());
		if (holder != null)
		{
			Location3D loc = holder.getNodes()[index];
			if (loc != null)
			{
				enablePicking(activeChar);
				holder.setChangingLoc(loc);
			}
		}
	}
	
	/**
	 * @param activeChar
	 * @param index
	 */
	private void deletePoint(Player activeChar, int index)
	{
		ZoneNodeHolder holder = _zones.get(activeChar.getObjectId());
		if (holder != null)
		{
			Location3D loc = holder.getNodes()[index];
			if (loc != null)
			{
				holder.getNodes().Remove(loc);
				BuilderUtil.sendSysMessage(activeChar, "Node " + index + " has been removed!");
				if (holder.getNodes().Count == 0)
				{
					BuilderUtil.sendSysMessage(activeChar, "Since node list is empty destroying session!");
					_zones.remove(activeChar.getObjectId());
				}
			}
		}
	}
	
	/**
	 * @param activeChar
	 */
	private void dumpPoints(Player activeChar)
	{
		ZoneNodeHolder holder = _zones.get(activeChar.getObjectId());
		if (holder != null && holder.getNodes().Count != 0)
		{
			if (string.IsNullOrEmpty(holder.getName()))
			{
				BuilderUtil.sendSysMessage(activeChar, "Set name first!");
				return;
			}
			
			Location3D firstNode = holder.getNodes()[0];
			StringBuilder sj = new StringBuilder();
			sj.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			sj.AppendLine("<list enabled=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"../../data/xsd/zones.xsd\">");
			sj.AppendLine("\t<zone name=\"" + holder.getName() + "\" type=\"ScriptZone\" shape=\"NPoly\" minZ=\"" + (holder.getMinZ() != 0 ? holder.getMinZ() : firstNode.Z - 100) + "\" maxZ=\"" + (holder.getMaxZ() != 0 ? holder.getMaxZ() : firstNode.Z + 100) + "\">");
			foreach (Location3D loc in holder.getNodes())
			{
				sj.AppendLine("\t\t<node X=\"" + loc.X + "\" Y=\"" + loc.Y + "\" />");
			}
			
			sj.AppendLine("\t</zone>");
			sj.AppendLine("</list>");
			sj.AppendLine(); // new line at end of file
			
			try
			{
				Directory.CreateDirectory("log/points/" + activeChar.getAccountName());
				string filePath = "log/points/" + activeChar.getAccountName() + "/" + holder.getName() + ".xml";
				int i = 0;
				while (File.Exists(filePath))
				{
					filePath = "log/points/" + activeChar.getAccountName() + "/" + holder.getName() + i + ".xml";
					i++;
				}

				File.WriteAllText(filePath, sj.ToString());
				BuilderUtil.sendSysMessage(activeChar, "Successfully written on: " + filePath);
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Failed writing the dump: " + e);
				_logger.Warn("Failed writing point picking dump for " + activeChar.getName() + ": " + e);
			}
		}
	}
	
	[SubscribeEvent(SubscriptionType.GlobalPlayers)]
	public void onPlayerPointPicking(OnPlayerMoveRequest ev)
	{
		Player player = ev.getPlayer();
		if (player.hasAction(PlayerAction.ADMIN_POINT_PICKING))
		{
			Location3D newLocation = ev.getLocation();
			ZoneNodeHolder holder = _zones.computeIfAbsent(player.getObjectId(), key => new ZoneNodeHolder(player));
			Location3D? changeLog = holder.getChangingLoc();
			if (changeLog != null)
			{
				changeLog = newLocation;
				holder.setChangingLoc(null);
				BuilderUtil.sendSysMessage(player, "Location " + (holder.indexOf(changeLog.Value) + 1) + " has been updated!");
				disablePicking(player);
			}
			else
			{
				holder.addNode(newLocation);
				BuilderUtil.sendSysMessage(player, "Location " + (holder.indexOf(newLocation) + 1) + " has been added!");
			}
			
			// Auto visualization when nodes >= 3
			if (holder.getNodes().Count >= 3)
			{
				showPoints(player);
			}

			buildHtmlWindow(player, 0);

			ev.Terminate = true;
		}
	}
	
	[SubscribeEvent(SubscriptionType.GlobalPlayers)]
	public void onPlayerDlgAnswer(OnPlayerDlgAnswer ev)
	{
		Player player = ev.getPlayer();
		if (player.removeAction(PlayerAction.ADMIN_SHOW_TERRITORY) && ev.getAnswer() == 1)
		{
			ZoneNodeHolder holder = _zones.get(player.getObjectId());
			if (holder != null)
			{
				List<Location3D> list = holder.getNodes();
				if (list.Count < 3)
				{
					BuilderUtil.sendSysMessage(player, "You must have at least 3 nodes to use this option!");
					return;
				}
				
				Location3D firstLoc = list[0];
				int minZ = holder.getMinZ() != 0 ? holder.getMinZ() : firstLoc.Z - 100;
				int maxZ = holder.getMaxZ() != 0 ? holder.getMaxZ() : firstLoc.Z + 100;

				List<Location2D> vertices = list.Select(x => x.Location2D).ToList();
				ExShowTerritoryPacket exst = new(minZ, maxZ, vertices);
				player.sendPacket(exst);
				BuilderUtil.sendSysMessage(player, "In order to remove the debug you must restart your game client!");
			}
		}
	}
	
	public string[] getAdminCommandList()
	{
		return COMMANDS;
	}
	
	private void buildHtmlWindow(Player activeChar, int page)
	{
		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/zone_editor_create.htm", activeChar);
		ZoneNodeHolder holder = _zones.computeIfAbsent(activeChar.getObjectId(), key => new ZoneNodeHolder(activeChar));
		AtomicInteger position = new AtomicInteger(page * 20);

		PageResult result = PageBuilder.newBuilder(holder.getNodes(), 20, "bypass -h admin_zones list")
			.currentPage(page).bodyHandler((pages, loc, sb) =>
			{
				sb.Append("<tr>");
				sb.Append("<td fixwidth=5></td>");
				sb.Append("<td fixwidth=20>" + position.getAndIncrement() + "</td>");
				sb.Append("<td fixwidth=60>" + loc.X + "</td>");
				sb.Append("<td fixwidth=60>" + loc.Y + "</td>");
				sb.Append("<td fixwidth=60>" + loc.Z + "</td>");
				sb.Append("<td fixwidth=30><a action=\"bypass -h admin_zones change " + holder.indexOf(loc) +
				          "\">[E]</a></td>");
				sb.Append("<td fixwidth=30><a action=\"bypass -h admin_move_to " + loc.X + " " + loc.Y + " " +
				          loc.Z + "\">[T]</a></td>");
				sb.Append("<td fixwidth=30><a action=\"bypass -h admin_zones delete " + holder.indexOf(loc) +
				          "\">[D]</a></td>");
				sb.Append("<td fixwidth=5></td>");
				sb.Append("</tr>");
			}).build();
		
		htmlContent.Replace("%name%", holder.getName());
		htmlContent.Replace("%minZ%", holder.getMinZ());
		htmlContent.Replace("%maxZ%", holder.getMaxZ());
		htmlContent.Replace("%pages%", result.getPagerTemplate());
		htmlContent.Replace("%nodes%", result.getBodyTemplate());

		NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(null, 1, htmlContent);
		activeChar.sendPacket(msg);
	}

	private class ZoneNodeHolder
	{
		private readonly List<Location3D> _nodes = [];
		private string _name = string.Empty;
		private Location3D? _changingLoc;
		private int _minZ;
		private int _maxZ;
		
		public ZoneNodeHolder(Player player)
		{
			_minZ = player.getZ() - 200;
			_maxZ = player.getZ() + 200;
		}
		
		public void setName(string name)
		{
			_name = name;
		}
		
		public string getName()
		{
			return _name;
		}
		
		public void setChangingLoc(Location3D? loc)
		{
			_changingLoc = loc;
		}
		
		public Location3D? getChangingLoc()
		{
			return _changingLoc;
		}
		
		public void addNode(Location3D loc)
		{
			_nodes.Add(loc);
		}
		
		public List<Location3D> getNodes()
		{
			return _nodes;
		}
		
		public int indexOf(Location3D loc)
		{
			return _nodes.IndexOf(loc);
		}
		
		public int getMinZ()
		{
			return _minZ;
		}
		
		public int getMaxZ()
		{
			return _maxZ;
		}
		
		public void setMinZ(int minZ)
		{
			_minZ = minZ;
		}
		
		public void setMaxZ(int maxZ)
		{
			_maxZ = maxZ;
		}
	}

	public override string Name => nameof(AdminZones);
}