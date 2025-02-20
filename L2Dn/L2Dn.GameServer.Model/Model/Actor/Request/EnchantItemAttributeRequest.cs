using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Actor.Request;

public class EnchantItemAttributeRequest: AbstractRequest
{
    private volatile int _enchantingItemObjectId;
    private volatile int _enchantingStoneObjectId;

    public EnchantItemAttributeRequest(Player player, int enchantingStoneObjectId): base(player)
    {
        _enchantingStoneObjectId = enchantingStoneObjectId;
    }

    public Item getEnchantingItem()
    {
        return getActiveChar().getInventory().getItemByObjectId(_enchantingItemObjectId);
    }

    public void setEnchantingItem(int objectId)
    {
        _enchantingItemObjectId = objectId;
    }

    public Item getEnchantingStone()
    {
        return getActiveChar().getInventory().getItemByObjectId(_enchantingStoneObjectId);
    }

    public void setEnchantingStone(int objectId)
    {
        _enchantingStoneObjectId = objectId;
    }

    public override bool isItemRequest()
    {
        return true;
    }

    public override bool canWorkWith(AbstractRequest request)
    {
        return !request.isItemRequest();
    }

    public override bool isUsing(int objectId)
    {
        return objectId > 0 && (objectId == _enchantingItemObjectId || objectId == _enchantingStoneObjectId);
    }
}
