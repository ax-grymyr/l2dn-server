﻿using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model.Actor.Appearance;

public class PlayerAppearance
{
	public const int DEFAULT_TITLE_COLOR = 0xECF9A2;
	
	private Player _owner;
	
	private byte _face;
	
	private byte _hairColor;
	
	private byte _hairStyle;
	
	private Sex _sex;
	
	/** The current visible name of this player, not necessarily the real one */
	private string _visibleName;
	
	/** The current visible title of this player, not necessarily the real one */
	private string _visibleTitle;
	
	/** The default name color is 0xFFFFFF. */
	private Color _nameColor = Colors.White;
	
	/** The default title color is 0xECF9A2. */
	private Color _titleColor = new(DEFAULT_TITLE_COLOR);
	
	private int? _visibleClanId;
	private int? _visibleClanCrestId;
	private int? _visibleClanLargeCrestId;
	private int? _visibleAllyId;
	private int? _visibleAllyCrestId;
	
	public PlayerAppearance(byte face, byte hColor, byte hStyle, Sex sex)
	{
		_face = face;
		_hairColor = hColor;
		_hairStyle = hStyle;
		_sex = sex;
	}
	
	/**
	 * @param visibleName The visibleName to set.
	 */
	public void setVisibleName(string visibleName)
	{
		_visibleName = visibleName;
	}
	
	/**
	 * @return Returns the visibleName.
	 */
	public string getVisibleName()
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
	public void setVisibleTitle(string visibleTitle)
	{
		_visibleTitle = visibleTitle;
	}
	
	/**
	 * @return Returns the visibleTitle.
	 */
	public string getVisibleTitle()
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
	
	public void setSex(Sex sex)
	{
		_sex = sex;
	}
	
	/**
	 * @return Sex of the char
	 */
	public Sex getSex()
	{
		return _sex;
	}
	
	public Color getNameColor()
	{
		return _nameColor;
	}
	
	public void setNameColor(Color nameColor)
	{
		_nameColor = nameColor;
	}
	
	public Color getTitleColor()
	{
		return _titleColor;
	}
	
	public void setTitleColor(Color titleColor)
	{
		_titleColor = titleColor;
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
	
	public int? getVisibleClanId()
	{
		return _visibleClanId ?? (_owner.isCursedWeaponEquipped() ? null : _owner.getClanId());
	}
	
	public int? getVisibleClanCrestId()
	{
		return _visibleClanCrestId ?? (_owner.isCursedWeaponEquipped() ? null : _owner.getClanCrestId());
	}
	
	public int? getVisibleClanLargeCrestId()
	{
		return _visibleClanLargeCrestId ?? (_owner.isCursedWeaponEquipped() ? null : _owner.getClanCrestLargeId());
	}
	
	public int? getVisibleAllyId()
	{
		return _visibleAllyId ?? (_owner.isCursedWeaponEquipped() ? null : _owner.getAllyId());
	}
	
	public int? getVisibleAllyCrestId()
	{
		return _visibleAllyCrestId ?? ((_owner == null) || _owner.isCursedWeaponEquipped() ? null : _owner.getAllyCrestId());
	}
	
	public void setVisibleClanData(int? clanId, int? clanCrestId, int? clanLargeCrestId, int? allyId, int? allyCrestId)
	{
		_visibleClanId = clanId;
		_visibleClanCrestId = clanCrestId;
		_visibleClanLargeCrestId = clanLargeCrestId;
		_visibleAllyId = allyId;
		_visibleAllyCrestId = allyCrestId;
	}
}