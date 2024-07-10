using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.OutgoingPackets.LimitShop;
using L2Dn.GameServer.Network.OutgoingPackets.PledgeDonation;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Network.IncomingPackets.Pledges;

public struct RequestExPledgeDonationRequestPacket: IIncomingPacket<GameSession>
{
    private int _type;

    public void ReadContent(PacketBitReader reader)
    {
        _type = reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		Clan clan = player.getClan();
		if (clan == null)
			return ValueTask.CompletedTask;
		
		switch (_type)
		{
			case 0:
			{
				if (player.reduceAdena("Pledge donation", 100000, null, true))
				{
					clan.addExp(player.getObjectId(), 50);
				}
				else
				{
					player.sendPacket(new ExPledgeDonationRequestPacket(false, _type, 2));
				}
				break;
			}
			case 1:
			{
				if (player.getInventory().getInventoryItemCount(Inventory.LCOIN_ID, -1) >= 100)
				{
					player.destroyItemByItemId("Pledge donation", Inventory.LCOIN_ID, 100, player, true);
					clan.addExp(player.getObjectId(), 100);
					player.setHonorCoins(player.getHonorCoins() + 100);
				}
				else
				{
					player.sendPacket(new ExPledgeDonationRequestPacket(false, _type, 2));
				}
				break;
			}
			case 2:
			{
				if (player.getInventory().getInventoryItemCount(Inventory.LCOIN_ID, -1) >= 500)
				{
					player.destroyItemByItemId("Pledge donation", Inventory.LCOIN_ID, 500, player, true);
					clan.addExp(player.getObjectId(), 500);
					player.setHonorCoins(player.getHonorCoins() + 500);
				}
				else
				{
					player.sendPacket(new ExPledgeDonationRequestPacket(false, _type, 2));
				}
				break;
			}
		}
		player.getVariables().set(PlayerVariables.CLAN_DONATION_POINTS, Math.Max(player.getClanDonationPoints() - 1, 0));
		criticalSuccess(player, clan, _type);
		player.sendPacket(new ExBloodyCoinCountPacket(player));
		player.sendItemList();
		player.sendPacket(new ExPledgeDonationRequestPacket(true, _type, player.getClanDonationPoints()));
		player.sendPacket(new ExPledgeDonationInfoPacket(player.getClanDonationPoints(), true));

		return ValueTask.CompletedTask;
	}
	
	private void criticalSuccess(Player player, Clan clan, int type)
	{
		if (type == 1)
		{
			if (Rnd.get(100) < 5)
			{
				player.setHonorCoins(player.getHonorCoins() + 200);
				clan.getMembers().ForEach(clanMember => sendMail(clanMember.getObjectId(), 1, player.getName()));
			}
		}
		else if (type == 2)
		{
			if (Rnd.get(100) < 5)
			{
				player.setHonorCoins(player.getHonorCoins() + 1000);
				clan.getMembers().ForEach(clanMember => sendMail(clanMember.getObjectId(), 5, player.getName()));
			}
		}
	}
	
	private static void sendMail(int charId, int amount, String donator)
	{
		Message msg = new Message(charId, "Clan Rewards for " + donator + " Donation",
			"The entire clan receives rewards for " + donator + " donation.",
			MailType.PLEDGE_DONATION_CRITICAL_SUCCESS);
		
		Mail attachment = msg.createAttachments();
		attachment.addItem("Pledge reward", 95672, amount, null, donator); // Honor Coin Pouch
		
		MailManager.getInstance().sendMessage(msg);
	}
}