using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;
using NLog;

namespace L2Dn.GameServer.Model;

public class ElementalSpirit
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(ElementalSpirit));
	
	private readonly Player _owner;
	private ElementalSpiritTemplateHolder _template;
	private readonly ElementalSpiritDataHolder _data;
	
	public ElementalSpirit(ElementalType type, Player owner)
	{
		_data = new ElementalSpiritDataHolder(type, owner.getObjectId());
		_template = ElementalSpiritData.getInstance().getSpirit(type, _data.getStage());
		_owner = owner;
	}
	
	public ElementalSpirit(ElementalSpiritDataHolder data, Player owner)
	{
		_owner = owner;
		_data = data;
		_template = ElementalSpiritData.getInstance().getSpirit(data.getType(), data.getStage());
	}
	
	public void addExperience(int experience)
	{
		if ((_data.getLevel() == _template.getMaxLevel()) && (_data.getExperience() >= _template.getMaxExperienceAtLevel(_template.getMaxLevel())))
		{
			return;
		}
		
		_data.addExperience(experience);
		_owner.sendPacket(new ExElementalSpiritGetExpPacket(_data.getType(), _data.getExperience()));

		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_ACQUIRED_S1_S2_ATTRIBUTE_XP);
		sm.Params.addInt(experience).addElementalSpirit(_data.getType());
		_owner.sendPacket(sm);
		
		if (_data.getExperience() > getExperienceToNextLevel())
		{
			levelUp();
			sm = new SystemMessagePacket(SystemMessageId.S1_ATTRIBUTE_SPIRIT_HAS_REACHED_LV_S2);
			sm.Params.addElementalSpirit(_data.getType()).addByte(_data.getLevel());
			_owner.sendPacket(sm);
			_owner.sendPacket(new ElementalSpiritInfoPacket(_owner, 0));
			_owner.sendPacket(new ExElementalSpiritAttackTypePacket(_owner));
			UserInfoPacket userInfo = new UserInfoPacket(_owner);
			userInfo.addComponentType(UserInfoType.ATT_SPIRITS);
			_owner.sendPacket(userInfo);
		}
	}
	
	private void levelUp()
	{
		do
		{
			if (_data.getLevel() < _template.getMaxLevel())
			{
				_data.increaseLevel();
			}
			else
			{
				_data.setExperience(getExperienceToNextLevel());
			}
		}
		while (_data.getExperience() > getExperienceToNextLevel());
	}
	
	public void reduceLevel()
	{
		_data.setLevel(Math.Max(1, _data.getLevel() - 1));
		_data.setExperience(ElementalSpiritData.getInstance().getSpirit(_data.getType(), _data.getStage()).getMaxExperienceAtLevel(_data.getLevel() - 1));
		resetCharacteristics();
	}
	
	public int getAvailableCharacteristicsPoints()
	{
		int stage = _data.getStage();
		int level = _data.getLevel();
		int points = (stage > 3 ? ((stage - 2) * 20) : (stage - 1) * 10) + (stage > 2 ? (level * 2) : level * 1);
		return Math.Max(points - _data.getAttackPoints() - _data.getDefensePoints() - _data.getCritDamagePoints() - _data.getCritRatePoints(), 0);
	}
	
	public ElementalSpiritAbsorbItemHolder getAbsorbItem(int itemId)
	{
		foreach (ElementalSpiritAbsorbItemHolder absorbItem in getAbsorbItems())
		{
			if (absorbItem.getId() == itemId)
			{
				return absorbItem;
			}
		}
		return null;
	}
	
	public int getExtractAmount()
	{
		float amount = _data.getExperience() / ElementalSpiritData.FRAGMENT_XP_CONSUME;
		if (getLevel() > 1)
		{
			amount += ElementalSpiritData.getInstance().getSpirit(_data.getType(), _data.getStage())
				.getMaxExperienceAtLevel(getLevel() - 1) / ElementalSpiritData.FRAGMENT_XP_CONSUME;
		}
		
		return (int)amount;
	}
	
	public void resetStage()
	{
		_data.setLevel(1);
		_data.setExperience(0);
		resetCharacteristics();
	}
	
	public bool canEvolve()
	{
		return (_data.getStage() < 5) && (_data.getLevel() == 10) && (_data.getExperience() == getExperienceToNextLevel());
	}
	
	public void upgrade()
	{
		_data.increaseStage();
		_data.setLevel(1);
		_data.setExperience(0);
		_template = ElementalSpiritData.getInstance().getSpirit(_data.getType(), _data.getStage());
		
		if (EventDispatcher.getInstance().hasListener(EventType.ON_ELEMENTAL_SPIRIT_UPGRADE, _owner))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnElementalSpiritUpgrade(_owner, this), _owner);
		}
	}
	
	public void resetCharacteristics()
	{
		_data.setAttackPoints((byte) 0);
		_data.setDefensePoints((byte) 0);
		_data.setCritRatePoints((byte) 0);
		_data.setCritDamagePoints((byte) 0);
	}
	
	public ElementalType getType()
	{
		return _template.getType();
	}
	
	public byte getStage()
	{
		return _template.getStage();
	}
	
	public int getNpcId()
	{
		return _template.getNpcId();
	}
	
	public long getExperience()
	{
		return _data.getExperience();
	}
	
	public long getExperienceToNextLevel()
	{
		return _template.getMaxExperienceAtLevel(_data.getLevel());
	}
	
	public int getLevel()
	{
		return _data.getLevel();
	}
	
	public int getMaxLevel()
	{
		return _template.getMaxLevel();
	}
	
	public int getAttack()
	{
		return _template.getAttackAtLevel(_data.getLevel()) + (_data.getAttackPoints() * 5);
	}
	
	public int getDefense()
	{
		return _template.getDefenseAtLevel(_data.getLevel()) + (_data.getDefensePoints() * 5);
	}
	
	public int getMaxCharacteristics()
	{
		return _template.getMaxCharacteristics();
	}
	
	public int getAttackPoints()
	{
		return _data.getAttackPoints();
	}
	
	public int getDefensePoints()
	{
		return _data.getDefensePoints();
	}
	
	public int getCriticalRatePoints()
	{
		return _data.getCritRatePoints();
	}
	
	public int getCriticalDamagePoints()
	{
		return _data.getCritDamagePoints();
	}
	
	public List<ItemHolder> getItemsToEvolve()
	{
		return _template.getItemsToEvolve();
	}
	
	public List<ElementalSpiritAbsorbItemHolder> getAbsorbItems()
	{
		return _template.getAbsorbItems();
	}
	
	public int getExtractItem()
	{
		return _template.getExtractItem();
	}
	
	public void save()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			int charId = _data.getCharId();
			byte type = (byte)_data.getType();
			CharacterSpirit? spirit = ctx.CharacterSpirits.SingleOrDefault(r => r.CharacterId == charId && r.Type == type);
			if (spirit is null)
			{
				spirit = new CharacterSpirit()
				{
					CharacterId = charId,
					Type = type
				};
				
				ctx.CharacterSpirits.Add(spirit);
			}

			spirit.Level = (byte)_data.getLevel();
			spirit.Stage = _data.getStage();
			spirit.Exp = _data.getExperience();
			spirit.AttackPoints = _data.getAttackPoints();
			spirit.DefensePoints = _data.getDefensePoints();
			spirit.CriticalRatePoints = _data.getCritRatePoints();
			spirit.CriticalDamagePoints = _data.getCritDamagePoints();
			spirit.IsInUse = _data.isInUse();

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			_logger.Error(e);
		}
	}
	
	public void addAttackPoints(byte attackPoints)
	{
		_data.addAttackPoints(attackPoints);
	}
	
	public void addDefensePoints(byte defensePoints)
	{
		_data.addDefensePoints(defensePoints);
	}
	
	public void addCritRatePoints(byte critRatePoints)
	{
		_data.addCritRatePoints(critRatePoints);
	}
	
	public void addCritDamage(byte critDamagePoints)
	{
		_data.addCritDamagePoints(critDamagePoints);
	}
	
	public int getCriticalRate()
	{
		return _template.getCriticalRateAtLevel(_data.getLevel()) + getCriticalRatePoints();
	}
	
	public int getCriticalDamage()
	{
		return _template.getCriticalDamageAtLevel(_data.getLevel()) + getCriticalDamagePoints();
	}
	
	public void setInUse(bool value)
	{
		_data.setInUse(value);
	}
	
	public bool isInUse()
	{
		return _data.isInUse();
	}
}