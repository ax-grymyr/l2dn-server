using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.StaticData.Xml.Items;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Items;

/**
 * This class is dedicated to the management of armors.
 */
public sealed class Armor: ItemTemplate
{
    private readonly ArmorType _type;

    /**
     * Constructor for Armor.
     * @param set the StatSet designating the set of couples (key,value) characterizing the armor.
     */
    internal Armor(XmlItem xmlItem, ItemParameterSet parameters): base(xmlItem, parameters)
    {
        _type = parameters.GetEnum(XmlItemParameterType.armor_type, ArmorType.NONE);

        long bodyPart = getBodyPart();
        if (bodyPart == SLOT_ARTIFACT || bodyPart == SLOT_AGATHION)
        {
            _type1 = TYPE1_SHIELD_ARMOR;
            _type2 = TYPE2_ACCESSORY;
        }
        else if (bodyPart == SLOT_NECK || (bodyPart & SLOT_L_EAR) != 0 || (bodyPart & SLOT_L_FINGER) != 0 ||
                 (bodyPart & SLOT_R_BRACELET) != 0 || (bodyPart & SLOT_L_BRACELET) != 0 ||
                 (bodyPart & SLOT_ARTIFACT_BOOK) != 0)
        {
            _type1 = TYPE1_WEAPON_RING_EARRING_NECKLACE;
            _type2 = TYPE2_ACCESSORY;
        }
        else
        {
            if (_type == ArmorType.NONE && getBodyPart() == SLOT_L_HAND)
            {
                _type = ArmorType.SHIELD;
            }

            _type1 = TYPE1_SHIELD_ARMOR;
            _type2 = TYPE2_SHIELD_ARMOR;
        }
    }

    public ArmorType getArmorType() => _type;

    /**
     * @return the type of the armor.
     */
    public override ItemType getItemType() => _type;
}