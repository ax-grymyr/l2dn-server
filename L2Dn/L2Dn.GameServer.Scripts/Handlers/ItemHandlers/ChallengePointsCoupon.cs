using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Enchanting;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

/**
 * @author Serenitty
 */
public class ChallengePointsCoupon: IItemHandler
{
	public bool useItem(Playable playable, Item item, bool forceUse)
	{
        Player? player = playable.getActingPlayer();
		if (!playable.isPlayer() || player == null)
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}

		int pointsToGive;
		int categoryId;
		switch (item.getId())
		{
			case 97125: // Rare Accessory Challenge Points +50
			{
				pointsToGive = 50;
				categoryId = 1;
				break;
			}
			case 97126: // Talisman Challenge Points +50
			{
				pointsToGive = 50;
				categoryId = 2;
				break;
			}
			case 97127: // Special Equipment Challenge Points +50
			{
				pointsToGive = 50;
				categoryId = 3;
				break;
			}
			case 97276: // Rare Accessory Challenge Points +20
			{
				pointsToGive = 20;
				categoryId = 1;
				break;
			}
			case 97277: // Talisman Challenge Points +20
			{
				pointsToGive = 20;
				categoryId = 2;
				break;
			}
			case 97278: // Special Equipment Challenge Points +20
			{
				pointsToGive = 20;
				categoryId = 3;
				break;
			}
			default:
			{
				return false;
			}
		}

		if (player.getChallengeInfo().canAddPoints(categoryId, pointsToGive))
		{
			player.destroyItem("Challenge Coupon", item.ObjectId, 1, null, false);
			player.getChallengeInfo().getChallengePoints().compute(categoryId,
				(_, v) => Math.Min(EnchantChallengePointData.getInstance().getMaxPoints(), v + pointsToGive));

			player.sendPacket(new ExEnchantChallengePointInfoPacket(player));
		}
		else
		{
			player.sendMessage("The points of this coupon exceed the limit.");
		}

		return true;
	}
}