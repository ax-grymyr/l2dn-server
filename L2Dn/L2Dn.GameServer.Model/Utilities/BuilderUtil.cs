using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Utilities;

/**
 * @author lord_rex
 */
public static class BuilderUtil
{
	/**
	 * Sends builder system message to the player.
	 * @param player
	 * @param message
	 */
	public static void sendSysMessage(Player player, string message)
	{
		if (Config.GM_STARTUP_BUILDER_HIDE)
		{
			player.sendPacket(new CreatureSayPacket(null, ChatType.GENERAL, "SYS",
				SendMessageLocalisationData.getLocalisation(player, message)));
		}
		else
		{
			player.sendMessage(message);
		}
	}

	/**
	 * Sends builder html message to the player.
	 * @param player
	 * @param message
	 */
	public static void sendHtmlMessage(Player player, string message)
	{
		player.sendPacket(new CreatureSayPacket(null, ChatType.GENERAL, "HTML", message));
	}

	/**
	 * Changes player's hiding state.
	 * @param player
	 * @param hide
	 * @return {@code true} if hide state was changed, otherwise {@code false}
	 */
	public static bool setHiding(Player player, bool hide)
	{
		if (player.hasEnteredWorld())
		{
			if (player.isInvisible() && hide)
			{
				// already hiding
				return false;
			}

			if (!player.isInvisible() && !hide)
			{
				// already visible
				return false;
			}
		}

		player.setSilenceMode(hide);
		player.setInvul(hide);
		player.setInvisible(hide);

		player.broadcastUserInfo();
		player.sendPacket(new ExUserInfoAbnormalVisualEffectPacket(player));
		return true;
	}
}