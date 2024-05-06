using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Holders;

public sealed class ElementalItemHolder(int itemId, AttributeType element, ElementalItemType type, int power)
{
	public int getItemId() => itemId;
	public AttributeType getElement() => element;
	public ElementalItemType getType() => type;
	public int getPower() => power;
}