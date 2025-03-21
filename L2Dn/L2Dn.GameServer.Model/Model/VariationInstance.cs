using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Options;
using L2Dn.GameServer.StaticData;

namespace L2Dn.GameServer.Model;

/**
 * Used to store an augmentation and its bonuses.
 * @author durgus, UnAfraid, Pere
 */
public class VariationInstance
{
    private readonly int _mineralId;
    private readonly Option? _option1;
    private readonly Option? _option2;

    public VariationInstance(int mineralId, int option1Id, int option2Id)
    {
        _mineralId = mineralId;
        _option1 = OptionData.Instance.GetOptions(option1Id);
        _option2 = OptionData.Instance.GetOptions(option2Id);
    }

    public VariationInstance(int mineralId, Option? op1, Option? op2)
    {
        _mineralId = mineralId;
        _option1 = op1;
        _option2 = op2;
    }

    public int getMineralId()
    {
        return _mineralId;
    }

    public int getOption1Id()
    {
        return _option1 == null ? -1 : _option1.Id;
    }

    public int getOption2Id()
    {
        return _option2 == null ? -1 : _option2.Id;
    }

    public void applyBonus(Playable playable)
    {
        if (_option1 != null)
        {
            _option1.Apply(playable);
        }
        if (_option2 != null)
        {
            _option2.Apply(playable);
        }
    }

    public void removeBonus(Playable playable)
    {
        if (_option1 != null)
        {
            _option1.Remove(playable);
        }
        if (_option2 != null)
        {
            _option2.Remove(playable);
        }
    }
}