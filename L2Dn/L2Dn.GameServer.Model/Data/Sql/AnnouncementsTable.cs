using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Announcements;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using Announcement = L2Dn.GameServer.Model.Announcements.Announcement;

namespace L2Dn.GameServer.Data.Sql;

/**
 * Loads announcements from database.
 * @author UnAfraid
 */
public class AnnouncementsTable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AnnouncementsTable));

	private readonly Map<int, IAnnouncement> _announcements = new();

	protected AnnouncementsTable()
	{
		load();
	}

	private void load()
	{
		_announcements.Clear();
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var announcements = ctx.Announcements.AsNoTracking();
			foreach (var announcement in announcements)
			{
				AnnouncementType type = (AnnouncementType)announcement.Type;
				Announcement announce;
				switch (type)
				{
					case AnnouncementType.NORMAL:
					case AnnouncementType.CRITICAL:
					{
						announce = new Announcement(announcement);
						break;
					}
					case AnnouncementType.AUTO_NORMAL:
					case AnnouncementType.AUTO_CRITICAL:
					{
						announce = new AutoAnnouncement(announcement);
						break;
					}
					default:
					{
						continue;
					}
				}

				_announcements.put(announce.getId(), announce);
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Failed loading announcements: " + e);
		}
	}

	/**
	 * Sending all announcements to the player
	 * @param player
	 */
	public void showAnnouncements(Player player)
	{
		sendAnnouncements(player, AnnouncementType.NORMAL);
		sendAnnouncements(player, AnnouncementType.CRITICAL);
		sendAnnouncements(player, AnnouncementType.EVENT);
	}

	/**
	 * Sends all announcements to the player by the specified type
	 * @param player
	 * @param type
	 */
	private void sendAnnouncements(Player player, AnnouncementType type)
	{
		foreach (IAnnouncement announce in _announcements.Values)
		{
			if (announce.isValid() && (announce.getType() == type))
			{
				player.sendPacket(new CreatureSayPacket(null,
					type == AnnouncementType.CRITICAL ? ChatType.CRITICAL_ANNOUNCE : ChatType.ANNOUNCEMENT,
					player.getName(), announce.getContent()));
			}
		}
	}

	/**
	 * Adds announcement
	 * @param announce
	 */
	public void addAnnouncement(IAnnouncement announce)
	{
		if (announce.storeMe())
		{
			_announcements.put(announce.getId(), announce);
		}
	}

	/**
	 * Removes announcement by id
	 * @param id
	 * @return {@code true} if announcement exists and was deleted successfully, {@code false} otherwise.
	 */
	public bool deleteAnnouncement(int id)
	{
		IAnnouncement? announce = _announcements.remove(id);
		return (announce != null) && announce.deleteMe();
	}

	/**
	 * @param id
	 * @return {@link IAnnouncement} by id
	 */
	public IAnnouncement? getAnnounce(int id)
	{
		return _announcements.GetValueOrDefault(id);
	}

	/**
	 * @return {@link Collection} containing all announcements
	 */
	public ICollection<IAnnouncement> getAllAnnouncements()
	{
		return _announcements.Values;
	}

	/**
	 * @return Single instance of {@link AnnouncementsTable}
	 */
	public static AnnouncementsTable getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly AnnouncementsTable INSTANCE = new();
	}
}