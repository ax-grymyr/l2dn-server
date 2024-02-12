using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model.Actor.Appearance;

public class PlayerAppearance
{
	public const int DEFAULT_TITLE_COLOR = 0xECF9A2;
	
	private Player _owner;
	
	private byte _face;
	
	private byte _hairColor;
	
	private byte _hairStyle;
	
	private bool _isFemale;
	
	/** The current visible name of this player, not necessarily the real one */
	private String _visibleName;
	
	/** The current visible title of this player, not necessarily the real one */
	private String _visibleTitle;
	
	/** The default name color is 0xFFFFFF. */
	private Color _nameColor = Colors.White;
	
	/** The default title color is 0xECF9A2. */
	private Color _titleColor = new(DEFAULT_TITLE_COLOR);
	
	private int _visibleClanId = -1;
	private int _visibleClanCrestId = -1;
	private int _visibleClanLargeCrestId = -1;
	private int _visibleAllyId = -1;
	private int _visibleAllyCrestId = -1;
	
	public PlayerAppearance(byte face, byte hColor, byte hStyle, bool isFemale)
	{
		_face = face;
		_hairColor = hColor;
		_hairStyle = hStyle;
		_isFemale = isFemale;
	}
	
	/**
	 * @param visibleName The visibleName to set.
	 */
	public void setVisibleName(String visibleName)
	{
		_visibleName = visibleName;
	}
	
	/**
	 * @return Returns the visibleName.
	 */
	public String getVisibleName()
	{
		if (_visibleName == null)
		{
			return _owner.getName();
		}
		return _visibleName;
	}
	
	/**
	 * @param visibleTitle The visibleTitle to set.
	 */
	public void setVisibleTitle(String visibleTitle)
	{
		_visibleTitle = visibleTitle;
	}
	
	/**
	 * @return Returns the visibleTitle.
	 */
	public String getVisibleTitle()
	{
		if (_visibleTitle == null)
		{
			return _owner.getTitle();
		}
		return _visibleTitle;
	}
	
	public byte getFace()
	{
		return _face;
	}
	
	/**
	 * @param value
	 */
	public void setFace(int value)
	{
		_face = (byte) value;
	}
	
	public byte getHairColor()
	{
		return _hairColor;
	}
	
	/**
	 * @param value
	 */
	public void setHairColor(int value)
	{
		_hairColor = (byte) value;
	}
	
	public byte getHairStyle()
	{
		return _hairStyle;
	}
	
	/**
	 * @param value
	 */
	public void setHairStyle(int value)
	{
		_hairStyle = (byte) value;
	}
	
	/**
	 * @return true if char is female
	 */
	public bool isFemale()
	{
		return _isFemale;
	}
	
	public void setFemale()
	{
		_isFemale = true;
	}
	
	public void setMale()
	{
		_isFemale = false;
	}
	
	/**
	 * @return Sex of the char
	 */
	public Sex getSexType()
	{
		return _isFemale ? Sex.FEMALE : Sex.MALE;
	}
	
	public int getNameColor()
	{
		return _nameColor;
	}
	
	public void setNameColor(Color nameColor)
	{
		_nameColor = nameColor;
	}
	
	public void setNameColor(int red, int green, int blue)
	{
		_nameColor = (red & 0xFF) + ((green & 0xFF) << 8) + ((blue & 0xFF) << 16);
	}
	
	public int getTitleColor()
	{
		return _titleColor;
	}
	
	public void setTitleColor(Color titleColor)
	{
		if (titleColor < 0)
		{
			return;
		}
		
		_titleColor = titleColor;
	}
	
	public void setTitleColor(int red, int green, int blue)
	{
		_titleColor = (red & 0xFF) + ((green & 0xFF) << 8) + ((blue & 0xFF) << 16);
	}
	
	/**
	 * @param owner The owner to set.
	 */
	public void setOwner(Player owner)
	{
		_owner = owner;
	}
	
	/**
	 * @return Returns the owner.
	 */
	public Player getOwner()
	{
		return _owner;
	}
	
	public int getVisibleClanId()
	{
		return _visibleClanId != -1 ? _visibleClanId : _owner.isCursedWeaponEquipped() ? 0 : _owner.getClanId();
	}
	
	public int getVisibleClanCrestId()
	{
		return _visibleClanCrestId != -1 ? _visibleClanCrestId : _owner.isCursedWeaponEquipped() ? 0 : _owner.getClanCrestId();
	}
	
	public int getVisibleClanLargeCrestId()
	{
		return _visibleClanLargeCrestId != -1 ? _visibleClanLargeCrestId : _owner.isCursedWeaponEquipped() ? 0 : _owner.getClanCrestLargeId();
	}
	
	public int getVisibleAllyId()
	{
		return _visibleAllyId != -1 ? _visibleAllyId : _owner.isCursedWeaponEquipped() ? 0 : _owner.getAllyId();
	}
	
	public int getVisibleAllyCrestId()
	{
		return _visibleAllyCrestId != -1 ? _visibleAllyCrestId : (_owner == null) || _owner.isCursedWeaponEquipped() ? 0 : _owner.getAllyCrestId();
	}
	
	public void setVisibleClanData(int clanId, int clanCrestId, int clanLargeCrestId, int allyId, int allyCrestId)
	{
		_visibleClanId = clanId;
		_visibleClanCrestId = clanCrestId;
		_visibleClanLargeCrestId = clanLargeCrestId;
		_visibleAllyId = allyId;
		_visibleAllyCrestId = allyCrestId;
	}
}
