using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Pets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Restoration effect implementation.
/// </summary>
[HandlerName("Restoration")]
public sealed class Restoration: AbstractEffect
{
    private readonly int _itemId;
    private readonly int _itemCount;
    private readonly int _itemEnchantmentLevel;

    public Restoration(EffectParameterSet parameters)
    {
        _itemId = parameters.GetInt32(XmlSkillEffectParameterType.ItemId, 0);
        _itemCount = parameters.GetInt32(XmlSkillEffectParameterType.ItemCount, 0);
        _itemEnchantmentLevel = parameters.GetInt32(XmlSkillEffectParameterType.ItemEnchantmentLevel, 0);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effected.isPlayable())
            return;

        if (_itemId <= 0 || _itemCount <= 0)
        {
            effected.sendPacket(SystemMessageId.FAILED_TO_CHANGE_THE_ITEM);
            Logger.Warn(GetType().Name + " effect with wrong item Id/count: " + _itemId + "/" + _itemCount + "!");
            return;
        }

        Player? effectedPlayer = effected.getActingPlayer();
        if (effected.isPlayer() && effectedPlayer != null)
        {
            Item? newItem = effectedPlayer.addItem("Skill", _itemId, _itemCount, effector, true);
            if (newItem == null)
            {
                effected.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL); // TODO: proper message
                return;
            }

            if (_itemEnchantmentLevel > 0)
                newItem.setEnchantLevel(_itemEnchantmentLevel);
        }
        else if (effected.isPet() && effectedPlayer != null)
        {
            Item? newItem = effectedPlayer.getInventory().
                addItem("Skill", _itemId, _itemCount, effectedPlayer, effector);

            if (newItem != null && _itemEnchantmentLevel > 0)
                newItem.setEnchantLevel(_itemEnchantmentLevel);

            effectedPlayer.sendPacket(new PetItemListPacket(effectedPlayer.getInventory().getItems()));
        }
    }

    public override EffectTypes EffectTypes => EffectTypes.EXTRACT_ITEM;

    public override int GetHashCode() => HashCode.Combine(_itemId, _itemCount, _itemEnchantmentLevel);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._itemId, x._itemCount, x._itemEnchantmentLevel));
}