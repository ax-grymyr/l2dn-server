using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.CommunityBbs.Managers;

public abstract class BaseBBSManager
{
	public abstract void parsecmd(String command, Player player);
	
	public abstract void parsewrite(String ar1, String ar2, String ar3, String ar4, String ar5, Player player);
	
	/**
	 * @param html
	 * @param acha
	 */
	protected void send1001(String html, Player acha)
	{
		if (html.Length < 8192)
		{
			acha.sendPacket(new ShowBoardPacket(html, "1001"));
		}
	}
	
	/**
	 * @param acha
	 */
	protected void send1002(Player acha)
	{
		send1002(acha, " ", " ", "0");
	}

	/**
	 * @param player
	 * @param string
	 * @param string2
	 * @param string3
	 */
	protected void send1002(Player player, String str, String string2, String string3)
	{
		string[] args =
		[
			"0", "0", "0", "0", "0", "0", 
			player.getName(),
			player.getObjectId().ToString(),
			player.getAccountName(),
			"9",
			string2, // subject?
			string2, // subject?
			str, // text
			string3, // date?
			string3, // date?
			"0",
			"0"
		];
		
		player.sendPacket(new ShowBoardPacket(args));
	}
}
