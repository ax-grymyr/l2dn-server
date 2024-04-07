using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Geo.GeoDataImpl;
using L2Dn.GameServer.Geo.GeoDataImpl.Regions;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author -Nemesiss-, HorridoJoho
 */
public class AdminGeodata: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_geo_pos",
		"admin_geo_spawn_pos",
		"admin_geo_can_move",
		"admin_geo_can_see",
		"admin_geogrid",
		"admin_geomap",
		"admin_geocell",
		"admin_geosave",
		"admin_geosaveall",
		"admin_geoenablenorth",
		"admin_en",
		"admin_geodisablenorth",
		"admin_dn",
		"admin_geoenablesouth",
		"admin_es",
		"admin_geodisablesouth",
		"admin_ds",
		"admin_geoenableeast",
		"admin_ee",
		"admin_geodisableeast",
		"admin_de",
		"admin_geoenablewest",
		"admin_ew",
		"admin_geodisablewest",
		"admin_dw",
		"admin_geoedit",
		"admin_ge"
	};
	
	public bool useAdminCommand(String command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command, " ");
		String actualCommand = st.nextToken();
		switch (actualCommand.toLowerCase())
		{
			case "admin_geo_pos":
			{
				int worldX = activeChar.getX();
				int worldY = activeChar.getY();
				int worldZ = activeChar.getZ();
				int geoX = GeoEngine.getGeoX(worldX);
				int geoY = GeoEngine.getGeoY(worldY);
				
				if (GeoEngine.getInstance().hasGeoPos(geoX, geoY))
				{
					BuilderUtil.sendSysMessage(activeChar, "WorldX: " + worldX + ", WorldY: " + worldY + ", WorldZ: " + worldZ + ", GeoX: " + geoX + ", GeoY: " + geoY + ", GeoZ: " + GeoEngine.getInstance().getHeight(worldX, worldY, worldZ));
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "There is no geodata at this position.");
				}
				break;
			}
			case "admin_geo_spawn_pos":
			{
				int worldX = activeChar.getX();
				int worldY = activeChar.getY();
				int worldZ = activeChar.getZ();
				int geoX = GeoEngine.getGeoX(worldX);
				int geoY = GeoEngine.getGeoY(worldY);
				
				if (GeoEngine.getInstance().hasGeoPos(geoX, geoY))
				{
					BuilderUtil.sendSysMessage(activeChar, "WorldX: " + worldX + ", WorldY: " + worldY + ", WorldZ: " + worldZ + ", GeoX: " + geoX + ", GeoY: " + geoY + ", GeoZ: " + GeoEngine.getInstance().getHeight(worldX, worldY, worldZ));
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "There is no geodata at this position.");
				}
				break;
			}
			case "admin_geo_can_move":
			{
				WorldObject target = activeChar.getTarget();
				if (target != null)
				{
					if (GeoEngine.getInstance().canSeeTarget(activeChar, target))
					{
						BuilderUtil.sendSysMessage(activeChar, "Can move beeline.");
					}
					else
					{
						BuilderUtil.sendSysMessage(activeChar, "Can not move beeline!");
					}
				}
				else
				{
					activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				}
				break;
			}
			case "admin_geo_can_see":
			{
				WorldObject target = activeChar.getTarget();
				if (target != null)
				{
					if (GeoEngine.getInstance().canSeeTarget(activeChar, target))
					{
						BuilderUtil.sendSysMessage(activeChar, "Can see target.");
					}
					else
					{
						activeChar.sendPacket(SystemMessageId.CANNOT_SEE_TARGET);
					}
				}
				else
				{
					activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				}
				break;
			}
			case "admin_geogrid":
			{
				if (!st.hasMoreTokens() || !st.nextToken().equalsIgnoreCase("off"))
				{
					GeoUtils.debugGrid(activeChar);
				}
				else
				{
					GeoUtils.hideDebugGrid(activeChar);
				}
				break;
			}
			case "admin_geomap":
			{
				int x = ((activeChar.getX() - World.WORLD_X_MIN) >> 15) + World.TILE_X_MIN;
				int y = ((activeChar.getY() - World.WORLD_Y_MIN) >> 15) + World.TILE_Y_MIN;
				BuilderUtil.sendSysMessage(activeChar, "GeoMap: " + x + "_" + y + " (" + ((x - World.TILE_ZERO_COORD_X) * World.TILE_SIZE) + "," + ((y - World.TILE_ZERO_COORD_Y) * World.TILE_SIZE) + " to " + ((((x - World.TILE_ZERO_COORD_X) * World.TILE_SIZE) + World.TILE_SIZE) - 1) + "," + ((((y - World.TILE_ZERO_COORD_Y) * World.TILE_SIZE) + World.TILE_SIZE) - 1) + ")");
				break;
			}
			case "admin_geocell":
			{
				int geoX = GeoEngine.getGeoX(activeChar.getX());
				int geoY = GeoEngine.getGeoY(activeChar.getY());
				int geoZ = GeoEngine.getInstance().getNearestZ(geoX, geoY, activeChar.getZ());
				int worldX = GeoEngine.getWorldX(geoX);
				int worldY = GeoEngine.getWorldY(geoY);
				BuilderUtil.sendSysMessage(activeChar, "GeoCell: " + geoX + ", " + geoY + ". XYZ (" + worldX + ", " + worldY + ", " + geoZ + ")");
				break;
			}
			case "admin_geosave":
			{
				// Create the saves directory if it does not exist.
				string savesDir = Config.GEOEDIT_PATH;
				try
				{
					Directory.CreateDirectory(savesDir);
				}
				catch (IOException e)
				{
					BuilderUtil.sendSysMessage(activeChar, "Could not create output directory.");
					return false;
				}
				
				int x = ((activeChar.getX() - World.WORLD_X_MIN) >> 15) + World.TILE_X_MIN;
				int y = ((activeChar.getY() - World.WORLD_Y_MIN) >> 15) + World.TILE_Y_MIN;
				String fileName = string.Format(GeoEngine.FILE_NAME_FORMAT, x, y);
				IRegion region = GeoEngine.getInstance().getRegion(GeoEngine.getGeoX(activeChar.getX()), GeoEngine.getGeoY(activeChar.getY()));
				if (region is NullRegion)
				{
					BuilderUtil.sendSysMessage(activeChar, "Could not find region: " + x + "_" + y);
				}
				else if (region.saveToFile(fileName))
				{
					BuilderUtil.sendSysMessage(activeChar, "Saved region " + x + "_" + y + " at " + fileName);
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "Could not save region " + x + "_" + y);
				}
				break;
			}
			case "admin_geosaveall":
			{
				// Create the saves directory if it does not exist.
				string savesDir = Config.GEOEDIT_PATH;
				try
				{
					Directory.CreateDirectory(savesDir);
				}
				catch (IOException e)
				{
					BuilderUtil.sendSysMessage(activeChar, "Could not create output directory.");
					return false;
				}
				
				int count = 0;
				int worldX = -327680; // Top left Gracia X coord.
				int worldY = -262144; // Top left Gracia Y coord.
				for (int regionX = World.TILE_X_MIN - 1; regionX <= World.TILE_X_MAX; regionX++)
				{
					for (int regionY = World.TILE_Y_MIN - 1; regionY <= World.TILE_Y_MAX; regionY++)
					{
						int geoX = GeoEngine.getGeoX(worldX);
						int geoY = GeoEngine.getGeoY(worldY);
						IRegion region = GeoEngine.getInstance().getRegion(geoX, geoY);
						if (!(region is NullRegion))
						{
							int x = ((worldX - World.WORLD_X_MIN) >> 15) + World.TILE_X_MIN;
							int y = ((worldY - World.WORLD_Y_MIN) >> 15) + World.TILE_Y_MIN;
							String fileName = string.Format(GeoEngine.FILE_NAME_FORMAT, x, y);
							if (region.saveToFile(fileName))
							{
								BuilderUtil.sendSysMessage(activeChar, "Saved region " + x + "_" + y + " at " + fileName);
								count++;
							}
							else
							{
								BuilderUtil.sendSysMessage(activeChar, "Could not save region " + x + "_" + y);
							}
						}
						worldY += World.TILE_SIZE;
					}
					worldX += World.TILE_SIZE;
					worldY = -262144;
				}
				BuilderUtil.sendSysMessage(activeChar, "Saved " + count + " regions.");
				break;
			}
			case "admin_geoenablenorth":
			case "admin_en":
			{
				int geoX = st.hasMoreTokens() ? int.Parse(st.nextToken()) : GeoEngine.getGeoX(activeChar.getX());
				int geoY = st.hasMoreTokens() ? int.Parse(st.nextToken()) : GeoEngine.getGeoY(activeChar.getY());
				if (GeoEngine.getInstance().hasGeoPos(geoX, geoY))
				{
					GeoEngine.getInstance().setNearestNswe(geoX, geoY, activeChar.getZ(), Cell.NSWE_NORTH);
					if (!actualCommand.contains("geo"))
					{
						AdminCommandHandler.getInstance().useAdminCommand(activeChar, "admin_ge " + geoX + " " + geoY, false);
					}
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "There is no geodata at this position.");
				}
				break;
			}
			case "admin_geodisablenorth":
			case "admin_dn":
			{
				int geoX = st.hasMoreTokens() ? int.Parse(st.nextToken()) : GeoEngine.getGeoY(activeChar.getX());
				int geoY = st.hasMoreTokens() ? int.Parse(st.nextToken()) : GeoEngine.getGeoY(activeChar.getY());
				if (GeoEngine.getInstance().hasGeoPos(geoX, geoY))
				{
					GeoEngine.getInstance().unsetNearestNswe(geoX, geoY, activeChar.getZ(), Cell.NSWE_NORTH);
					if (!actualCommand.contains("geo"))
					{
						AdminCommandHandler.getInstance().useAdminCommand(activeChar, "admin_ge " + geoX + " " + geoY, false);
					}
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "There is no geodata at this position.");
				}
				break;
			}
			case "admin_geoenablesouth":
			case "admin_es":
			{
				int geoX = st.hasMoreTokens() ? int.Parse(st.nextToken()) : GeoEngine.getGeoX(activeChar.getX());
				int geoY = st.hasMoreTokens() ? int.Parse(st.nextToken()) : GeoEngine.getGeoY(activeChar.getY());
				if (GeoEngine.getInstance().hasGeoPos(geoX, geoY))
				{
					GeoEngine.getInstance().setNearestNswe(geoX, geoY, activeChar.getZ(), Cell.NSWE_SOUTH);
					if (!actualCommand.contains("geo"))
					{
						AdminCommandHandler.getInstance().useAdminCommand(activeChar, "admin_ge " + geoX + " " + geoY, false);
					}
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "There is no geodata at this position.");
				}
				break;
			}
			case "admin_geodisablesouth":
			case "admin_ds":
			{
				int geoX = st.hasMoreTokens() ? int.Parse(st.nextToken()) : GeoEngine.getGeoX(activeChar.getX());
				int geoY = st.hasMoreTokens() ? int.Parse(st.nextToken()) : GeoEngine.getGeoY(activeChar.getY());
				if (GeoEngine.getInstance().hasGeoPos(geoX, geoY))
				{
					GeoEngine.getInstance().unsetNearestNswe(geoX, geoY, activeChar.getZ(), Cell.NSWE_SOUTH);
					if (!actualCommand.contains("geo"))
					{
						AdminCommandHandler.getInstance().useAdminCommand(activeChar, "admin_ge " + geoX + " " + geoY, false);
					}
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "There is no geodata at this position.");
				}
				break;
			}
			case "admin_geoenableeast":
			case "admin_ee":
			{
				int geoX = st.hasMoreTokens() ? int.Parse(st.nextToken()) : GeoEngine.getGeoX(activeChar.getX());
				int geoY = st.hasMoreTokens() ? int.Parse(st.nextToken()) : GeoEngine.getGeoY(activeChar.getY());
				if (GeoEngine.getInstance().hasGeoPos(geoX, geoY))
				{
					GeoEngine.getInstance().setNearestNswe(geoX, geoY, activeChar.getZ(), Cell.NSWE_EAST);
					if (!actualCommand.contains("geo"))
					{
						AdminCommandHandler.getInstance().useAdminCommand(activeChar, "admin_ge " + geoX + " " + geoY, false);
					}
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "There is no geodata at this position.");
				}
				break;
			}
			case "admin_geodisableeast":
			case "admin_de":
			{
				int geoX = st.hasMoreTokens() ? int.Parse(st.nextToken()) : GeoEngine.getGeoX(activeChar.getX());
				int geoY = st.hasMoreTokens() ? int.Parse(st.nextToken()) : GeoEngine.getGeoY(activeChar.getY());
				if (GeoEngine.getInstance().hasGeoPos(geoX, geoY))
				{
					GeoEngine.getInstance().unsetNearestNswe(geoX, geoY, activeChar.getZ(), Cell.NSWE_EAST);
					if (!actualCommand.contains("geo"))
					{
						AdminCommandHandler.getInstance().useAdminCommand(activeChar, "admin_ge " + geoX + " " + geoY, false);
					}
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "There is no geodata at this position.");
				}
				break;
			}
			case "admin_geoenablewest":
			case "admin_ew":
			{
				int geoX = st.hasMoreTokens() ? int.Parse(st.nextToken()) : GeoEngine.getGeoX(activeChar.getX());
				int geoY = st.hasMoreTokens() ? int.Parse(st.nextToken()) : GeoEngine.getGeoY(activeChar.getY());
				if (GeoEngine.getInstance().hasGeoPos(geoX, geoY))
				{
					GeoEngine.getInstance().setNearestNswe(geoX, geoY, activeChar.getZ(), Cell.NSWE_WEST);
					if (!actualCommand.contains("geo"))
					{
						AdminCommandHandler.getInstance().useAdminCommand(activeChar, "admin_ge " + geoX + " " + geoY, false);
					}
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "There is no geodata at this position.");
				}
				break;
			}
			case "admin_geodisablewest":
			case "admin_dw":
			{
				int geoX = st.hasMoreTokens() ? int.Parse(st.nextToken()) : GeoEngine.getGeoX(activeChar.getX());
				int geoY = st.hasMoreTokens() ? int.Parse(st.nextToken()) : GeoEngine.getGeoY(activeChar.getY());
				if (GeoEngine.getInstance().hasGeoPos(geoX, geoY))
				{
					GeoEngine.getInstance().unsetNearestNswe(geoX, geoY, activeChar.getZ(), Cell.NSWE_WEST);
					if (!actualCommand.contains("geo"))
					{
						AdminCommandHandler.getInstance().useAdminCommand(activeChar, "admin_ge " + geoX + " " + geoY, false);
					}
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "There is no geodata at this position.");
				}
				break;
			}
			case "admin_geoedit":
			{
				String content = HtmCache.getInstance().getHtm("html/admin/geoedit.htm", activeChar.getLang());
				
				// Follow player heading.
				int direction = getPlayerDirection(activeChar);
				switch (direction)
				{
					case 0: // North.
					{
						content = content.Replace("%N%", "N");
						content = content.Replace("%E%", "E");
						content = content.Replace("%S%", "S");
						content = content.Replace("%W%", "W");
						break;
					}
					case 1: // East.
					{
						content = content.Replace("%N%", "E");
						content = content.Replace("%E%", "S");
						content = content.Replace("%S%", "W");
						content = content.Replace("%W%", "N");
						break;
					}
					case 2: // South.
					{
						content = content.Replace("%N%", "S");
						content = content.Replace("%E%", "W");
						content = content.Replace("%S%", "N");
						content = content.Replace("%W%", "E");
						break;
					}
					default: // West.
					{
						content = content.Replace("%N%", "W");
						content = content.Replace("%E%", "N");
						content = content.Replace("%S%", "E");
						content = content.Replace("%W%", "S");
						break;
					}
				}
				
				int geoRadius = 9;
				int geoX = GeoEngine.getGeoX(activeChar.getX());
				int geoY = GeoEngine.getGeoY(activeChar.getY());
				int playerZ = activeChar.getZ();
				for (int dx = -geoRadius; dx <= geoRadius; ++dx)
				{
					for (int dy = -geoRadius; dy <= geoRadius; ++dy)
					{
						int translatedDx;
						int translatedDy;
						switch (direction)
						{
							case 0: // North.
							{
								translatedDx = dx;
								translatedDy = dy;
								break;
							}
							case 1: // East.
							{
								translatedDx = dy;
								translatedDy = -dx;
								break;
							}
							case 2: // South.
							{
								translatedDx = -dx;
								translatedDy = -dy;
								break;
							}
							default: // West.
							{
								translatedDx = -dy;
								translatedDy = dx;
								break;
							}
						}
						
						int gx = geoX + dx;
						int gy = geoY + dy;
						content = content.Replace("xy_" + translatedDx + "_" + translatedDy, gx + " " + gy);
						
						int z = GeoEngine.getInstance().getNearestZ(gx, gy, playerZ);
						bool northEnabled = GeoEngine.getInstance().checkNearestNswe(gx, gy, z, Cell.NSWE_NORTH);
						bool eastEnabled = GeoEngine.getInstance().checkNearestNswe(gx, gy, z, Cell.NSWE_EAST);
						bool southEnabled = GeoEngine.getInstance().checkNearestNswe(gx, gy, z, Cell.NSWE_SOUTH);
						bool westEnabled = GeoEngine.getInstance().checkNearestNswe(gx, gy, z, Cell.NSWE_WEST);
						content = content.Replace("bg_" + translatedDx + "_" + translatedDy, northEnabled && eastEnabled && southEnabled && westEnabled ? "L2UI_CH3.minibar_food" : "L2UI_CH3.minibar_arrow");
					}
				}
				
				Util.sendCBHtml(activeChar, content);
				break;
			}
			case "admin_ge":
			{
				if (!st.hasMoreTokens())
				{
					AdminCommandHandler.getInstance().useAdminCommand(activeChar, "admin_geoedit", false);
					break;
				}
				
				String content = HtmCache.getInstance().getHtm("html/admin/geoedit_cell.htm", activeChar.getLang());
				
				// Follow player heading.
				int direction = getPlayerDirection(activeChar);
				switch (direction)
				{
					case 0: // North.
					{
						content = content.Replace("%", "");
						break;
					}
					case 1: // East.
					{
						content = content.Replace("%N%", "E");
						content = content.Replace("%E%", "S");
						content = content.Replace("%S%", "W");
						content = content.Replace("%W%", "N");
						content = content.Replace("%bg_n%", "bg_e");
						content = content.Replace("%bg_e%", "bg_s");
						content = content.Replace("%bg_s%", "bg_w");
						content = content.Replace("%bg_w%", "bg_n");
						content = content.Replace("%cmd_n%", "cmd_e");
						content = content.Replace("%cmd_e%", "cmd_s");
						content = content.Replace("%cmd_s%", "cmd_w");
						content = content.Replace("%cmd_w%", "cmd_n");
						break;
					}
					case 2: // South.
					{
						content = content.Replace("%N%", "S");
						content = content.Replace("%E%", "W");
						content = content.Replace("%S%", "N");
						content = content.Replace("%W%", "E");
						content = content.Replace("%bg_n%", "bg_s");
						content = content.Replace("%bg_e%", "bg_w");
						content = content.Replace("%bg_s%", "bg_n");
						content = content.Replace("%bg_w%", "bg_e");
						content = content.Replace("%cmd_n%", "cmd_s");
						content = content.Replace("%cmd_e%", "cmd_w");
						content = content.Replace("%cmd_s%", "cmd_n");
						content = content.Replace("%cmd_w%", "cmd_e");
						break;
					}
					default: // West.
					{
						content = content.Replace("%N%", "W");
						content = content.Replace("%E%", "N");
						content = content.Replace("%S%", "E");
						content = content.Replace("%W%", "S");
						content = content.Replace("%bg_n%", "bg_w");
						content = content.Replace("%bg_e%", "bg_n");
						content = content.Replace("%bg_s%", "bg_e");
						content = content.Replace("%bg_w%", "bg_s");
						content = content.Replace("%cmd_n%", "cmd_w");
						content = content.Replace("%cmd_e%", "cmd_n");
						content = content.Replace("%cmd_s%", "cmd_e");
						content = content.Replace("%cmd_w%", "cmd_s");
						break;
					}
				}
				
				int gx = int.Parse(st.nextToken());
				int gy = int.Parse(st.nextToken());
				int z = GeoEngine.getInstance().getNearestZ(gx, gy, activeChar.getZ());
				if (GeoEngine.getInstance().checkNearestNswe(gx, gy, z, Cell.NSWE_NORTH))
				{
					content = content.Replace("bg_n", "L2UI_CH3.minibar_food");
					content = content.Replace("cmd_n", "dn " + gx + " " + gy);
				}
				else
				{
					content = content.Replace("bg_n", "L2UI_CH3.minibar_arrow");
					content = content.Replace("cmd_n", "en " + gx + " " + gy);
				}
				if (GeoEngine.getInstance().checkNearestNswe(gx, gy, z, Cell.NSWE_EAST))
				{
					content = content.Replace("bg_e", "L2UI_CH3.minibar_food");
					content = content.Replace("cmd_e", "de " + gx + " " + gy);
				}
				else
				{
					content = content.Replace("bg_e", "L2UI_CH3.minibar_arrow");
					content = content.Replace("cmd_e", "ee " + gx + " " + gy);
				}
				if (GeoEngine.getInstance().checkNearestNswe(gx, gy, z, Cell.NSWE_SOUTH))
				{
					content = content.Replace("bg_s", "L2UI_CH3.minibar_food");
					content = content.Replace("cmd_s", "ds " + gx + " " + gy);
				}
				else
				{
					content = content.Replace("bg_s", "L2UI_CH3.minibar_arrow");
					content = content.Replace("cmd_s", "es " + gx + " " + gy);
				}
				if (GeoEngine.getInstance().checkNearestNswe(gx, gy, z, Cell.NSWE_WEST))
				{
					content = content.Replace("bg_w", "L2UI_CH3.minibar_food");
					content = content.Replace("cmd_w", "dw " + gx + " " + gy);
				}
				else
				{
					content = content.Replace("bg_w", "L2UI_CH3.minibar_arrow");
					content = content.Replace("cmd_w", "ew " + gx + " " + gy);
				}
				
				Util.sendCBHtml(activeChar, content);
				break;
			}
		}
		return true;
	}

	private static int getPlayerDirection(Player activeChar)
	{
		int heading = activeChar.getHeading();
		if ((heading < 8192) || (heading > 57344))
		{
			return 0; // North.
		}
		else if (heading < 24576)
		{
			return 1; // East.
		}
		else if (heading < 40960)
		{
			return 2; // South.
		}
		else
		{
			return 3; // West.
		}
	}
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
