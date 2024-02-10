using System.Globalization;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.BuyList;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class Merchant: Folk
{
    public Merchant(NpcTemplate template): base(template)
    {
        setInstanceType(InstanceType.Merchant);
    }

    public override bool isAutoAttackable(Creature attacker)
    {
        if (attacker.isMonster())
        {
            return true;
        }

        return base.isAutoAttackable(attacker);
    }

    public override String getHtmlPath(int npcId, int value, Player player)
    {
        String pom;
        if (value == 0)
        {
            pom = npcId.ToString(CultureInfo.InvariantCulture);
        }
        else
        {
            pom = npcId + "-" + value;
        }

        return "data/html/merchant/" + pom + ".htm";
    }

    public void showBuyWindow(Player player, int value)
    {
        showBuyWindow(player, value, true);
    }

    public void showBuyWindow(Player player, int value, bool applyCastleTax)
    {
        ProductList buyList = BuyListData.getInstance().getBuyList(value);
        if (buyList == null)
        {
            LOGGER.Warn("BuyList not found! BuyListId:" + value);
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return;
        }

        if (!buyList.isNpcAllowed(getId()))
        {
            LOGGER.Warn("Npc not allowed in BuyList! BuyListId:" + value + " NpcId:" + getId());
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return;
        }

        player.setInventoryBlockingStatus(true);

        player.sendPacket(new ExBuySellList(buyList, player, (applyCastleTax) ? getCastleTaxRate(TaxType.BUY) : 0));
        player.sendPacket(new ExBuySellList(player, false));
        player.sendPacket(new ExBuySellList());
    }
}
