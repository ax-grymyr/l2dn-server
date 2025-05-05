using System.Collections.Immutable;
using L2Dn.GameServer.Constants;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.StaticData.Xml.Items;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Items;

/**
* This class is dedicated to the management of EtcItem.
*/
public sealed class EtcItem: ItemTemplate
{
    private readonly EtcItemType _type;
    private readonly string _handler;
    private readonly int _extractableCountMin;
    private readonly int _extractableCountMax;
    private readonly bool _isInfinite;
    private bool _isMineral;
    private bool _isEnsoulStone;

    /**
 * Constructor for EtcItem.
 * @param set StatSet designating the set of couples (key,value) for description of the Etc
 */
    internal EtcItem(XmlItem xmlItem, ItemParameterSet parameters): base(xmlItem, parameters)
    {
        _type = parameters.GetEnum(XmlItemParameterType.etcitem_type, EtcItemType.NONE);
        _type1 = TYPE1_ITEM_QUESTITEM_ADENA;
        _type2 = TYPE2_OTHER; // default is other

        if (isQuestItem())
            _type2 = TYPE2_QUEST;
        else if (Id is KnownItemId.Adena or KnownItemId.AncientAdena)
            _type2 = TYPE2_MONEY;

        _handler = parameters.GetString(XmlItemParameterType.handler, string.Empty);

        _extractableCountMin = parameters.GetInt32(XmlItemParameterType.extractableCountMin, 0);
        _extractableCountMax = parameters.GetInt32(XmlItemParameterType.extractableCountMax, 0);
        if (_extractableCountMin > _extractableCountMax)
        {
            Logger.Warn("Item " + this + " extractableCountMin is bigger than extractableCountMax!");
        }

        _isInfinite = parameters.GetBoolean(XmlItemParameterType.is_infinite, false);
    }

    public override ItemType getItemType()
    {
        return _type;
    }

    /**
 * @return the handler name, null if no handler for item.
 */
    public string getHandlerName()
    {
        return _handler;
    }

    /**
 * @return the minimum count of extractable items
 */
    public int getExtractableCountMin()
    {
        return _extractableCountMin;
    }

    /**
 * @return the maximum count of extractable items
 */
    public int getExtractableCountMax()
    {
        return _extractableCountMax;
    }

    /**
 * @return true if item is infinite
 */
    public bool isInfinite()
    {
        return _isInfinite;
    }

    public bool isMineral()
    {
        return _isMineral;
    }

    public void setMineral()
    {
        _isMineral = true;
    }

    public bool isEnsoulStone()
    {
        return _isEnsoulStone;
    }

    public void setEnsoulStone()
    {
        _isEnsoulStone = true;
    }
}