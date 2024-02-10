using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model;

/**
 * Used to store an augmentation and its bonuses.
 * @author durgus, UnAfraid, Pere
 */
public class VariationInstance
{
    private readonly int _mineralId;
    private readonly Options.Options _option1;
    private readonly Options.Options _option2;
	
    public VariationInstance(int mineralId, int option1Id, int option2Id)
    {
        _mineralId = mineralId;
        _option1 = OptionData.getInstance().getOptions(option1Id);
        _option2 = OptionData.getInstance().getOptions(option2Id);
    }
	
    public VariationInstance(int mineralId, Options.Options op1, Options.Options op2)
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
        return _option1 == null ? -1 : _option1.getId();
    }
	
    public int getOption2Id()
    {
        return _option2 == null ? -1 : _option2.getId();
    }
	
    public void applyBonus(Playable playable)
    {
        if (_option1 != null)
        {
            _option1.apply(playable);
        }
        if (_option2 != null)
        {
            _option2.apply(playable);
        }
    }
	
    public void removeBonus(Playable playable)
    {
        if (_option1 != null)
        {
            _option1.remove(playable);
        }
        if (_option2 != null)
        {
            _option2.remove(playable);
        }
    }
}
