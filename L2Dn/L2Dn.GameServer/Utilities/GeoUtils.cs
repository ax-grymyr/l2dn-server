using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Geo.GeoDataImpl;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Utilities;

/**
 * @author HorridoJoho
 */
public class GeoUtils
{
	public static void debug2DLine(Player player, int x, int y, int tx, int ty, int z)
	{
		int gx = GeoEngine.getInstance().getGeoX(x);
		int gy = GeoEngine.getInstance().getGeoY(y);

		int tgx = GeoEngine.getInstance().getGeoX(tx);
		int tgy = GeoEngine.getInstance().getGeoY(ty);

		ExServerPrimitivePacket prim = new ExServerPrimitivePacket("Debug2DLine", x, y, z);
		prim.addLine(Color.BLUE, GeoEngine.getInstance().getWorldX(gx), GeoEngine.getInstance().getWorldY(gy), z,
			GeoEngine.getInstance().getWorldX(tgx), GeoEngine.getInstance().getWorldY(tgy), z);

		LinePointIterator iter = new LinePointIterator(gx, gy, tgx, tgy);

		while (iter.next())
		{
			int wx = GeoEngine.getInstance().getWorldX(iter.x());
			int wy = GeoEngine.getInstance().getWorldY(iter.y());

			prim.addPoint(Color.RED, wx, wy, z);
		}

		player.sendPacket(prim);
	}

	public static void debug3DLine(Player player, int x, int y, int z, int tx, int ty, int tz)
	{
		int gx = GeoEngine.getInstance().getGeoX(x);
		int gy = GeoEngine.getInstance().getGeoY(y);

		int tgx = GeoEngine.getInstance().getGeoX(tx);
		int tgy = GeoEngine.getInstance().getGeoY(ty);

		ExServerPrimitivePacket prim = new ExServerPrimitivePacket("Debug3DLine", x, y, z);
		prim.addLine(Color.BLUE, GeoEngine.getInstance().getWorldX(gx), GeoEngine.getInstance().getWorldY(gy), z,
			GeoEngine.getInstance().getWorldX(tgx), GeoEngine.getInstance().getWorldY(tgy), tz);

		LinePointIterator3D iter = new LinePointIterator3D(gx, gy, z, tgx, tgy, tz);
		iter.next();
		int prevX = iter.x();
		int prevY = iter.y();
		int wx = GeoEngine.getInstance().getWorldX(prevX);
		int wy = GeoEngine.getInstance().getWorldY(prevY);
		int wz = iter.z();
		prim.addPoint(Color.RED, wx, wy, wz);

		while (iter.next())
		{
			int curX = iter.x();
			int curY = iter.y();

			if ((curX != prevX) || (curY != prevY))
			{
				wx = GeoEngine.getInstance().getWorldX(curX);
				wy = GeoEngine.getInstance().getWorldY(curY);
				wz = iter.z();

				prim.addPoint(Color.RED, wx, wy, wz);

				prevX = curX;
				prevY = curY;
			}
		}

		player.sendPacket(prim);
	}

	private static Color getDirectionColor(int x, int y, int z, int nswe)
	{
		if (GeoEngine.getInstance().checkNearestNswe(x, y, z, nswe))
		{
			return Color.GREEN;
		}

		return Color.RED;
	}

	public static void debugGrid(Player player)
	{
		int geoRadius = 20;
		int blocksPerPacket = 40;

		int iBlock = blocksPerPacket;
		int iPacket = 0;

		ExServerPrimitivePacket exsp = default;
		GeoEngine ge = GeoEngine.getInstance();
		int playerGx = ge.getGeoX(player.getX());
		int playerGy = ge.getGeoY(player.getY());
		for (int dx = -geoRadius; dx <= geoRadius; ++dx)
		{
			for (int dy = -geoRadius; dy <= geoRadius; ++dy)
			{
				if (iBlock >= blocksPerPacket)
				{
					iBlock = 0;
					if (exsp.Name != null)
					{
						++iPacket;
						player.sendPacket(exsp);
					}

					exsp = new ExServerPrimitivePacket("DebugGrid_" + iPacket, player.getX(), player.getY(), -16000);
				}

				if (exsp.Name == null)
				{
					throw new InvalidOperationException();
				}

				int gx = playerGx + dx;
				int gy = playerGy + dy;

				int x = ge.getWorldX(gx);
				int y = ge.getWorldY(gy);
				int z = ge.getNearestZ(gx, gy, player.getZ());

				// north arrow
				Color col = getDirectionColor(gx, gy, z, Cell.NSWE_NORTH);
				exsp.addLine(col, x - 1, y - 7, z, x + 1, y - 7, z);
				exsp.addLine(col, x - 2, y - 6, z, x + 2, y - 6, z);
				exsp.addLine(col, x - 3, y - 5, z, x + 3, y - 5, z);
				exsp.addLine(col, x - 4, y - 4, z, x + 4, y - 4, z);

				// east arrow
				col = getDirectionColor(gx, gy, z, Cell.NSWE_EAST);
				exsp.addLine(col, x + 7, y - 1, z, x + 7, y + 1, z);
				exsp.addLine(col, x + 6, y - 2, z, x + 6, y + 2, z);
				exsp.addLine(col, x + 5, y - 3, z, x + 5, y + 3, z);
				exsp.addLine(col, x + 4, y - 4, z, x + 4, y + 4, z);

				// south arrow
				col = getDirectionColor(gx, gy, z, Cell.NSWE_SOUTH);
				exsp.addLine(col, x - 1, y + 7, z, x + 1, y + 7, z);
				exsp.addLine(col, x - 2, y + 6, z, x + 2, y + 6, z);
				exsp.addLine(col, x - 3, y + 5, z, x + 3, y + 5, z);
				exsp.addLine(col, x - 4, y + 4, z, x + 4, y + 4, z);

				col = getDirectionColor(gx, gy, z, Cell.NSWE_WEST);
				exsp.addLine(col, x - 7, y - 1, z, x - 7, y + 1, z);
				exsp.addLine(col, x - 6, y - 2, z, x - 6, y + 2, z);
				exsp.addLine(col, x - 5, y - 3, z, x - 5, y + 3, z);
				exsp.addLine(col, x - 4, y - 4, z, x - 4, y + 4, z);

				++iBlock;
			}
		}

		player.sendPacket(exsp);
	}

	/**
	 * difference between x values: never above 1<br>
	 * difference between y values: never above 1
	 * @param lastX
	 * @param lastY
	 * @param x
	 * @param y
	 * @return
	 */
	public static int computeNswe(int lastX, int lastY, int x, int y)
	{
		if (x > lastX) // east
		{
			if (y > lastY)
			{
				return Cell.NSWE_SOUTH_EAST;
			}
			else if (y < lastY)
			{
				return Cell.NSWE_NORTH_EAST;
			}
			else
			{
				return Cell.NSWE_EAST;
			}
		}
		else if (x < lastX) // west
		{
			if (y > lastY)
			{
				return Cell.NSWE_SOUTH_WEST;
			}
			else if (y < lastY)
			{
				return Cell.NSWE_NORTH_WEST;
			}
			else
			{
				return Cell.NSWE_WEST;
			}
		}
		else // unchanged x
		{
			if (y > lastY)
			{
				return Cell.NSWE_SOUTH;
			}
			else if (y < lastY)
			{
				return Cell.NSWE_NORTH;
			}
			else
			{
				throw new InvalidOperationException();
			}
		}
	}
}