using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Zones;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A mother-trees zone Basic type zone for Hp, MP regen
 * @author durgus
 */
public class MotherTreeZone(int id, ZoneForm form): Zone(id, form)
{
	private SystemMessageId _enterMsg;
	private SystemMessageId _leaveMsg;
	private int _mpRegen;
	private int _hpRegen;

    public override void setParameter(XmlZoneStatName name, string value)
	{
		if (name == XmlZoneStatName.enterMsgId)
		{
			_enterMsg = (SystemMessageId)int.Parse(value);
		}
		else if (name == XmlZoneStatName.leaveMsgId)
		{
			_leaveMsg = (SystemMessageId)int.Parse(value);
		}
		else if (name == XmlZoneStatName.MpRegenBonus)
		{
			_mpRegen = int.Parse(value);
		}
		else if (name == XmlZoneStatName.HpRegenBonus)
		{
			_hpRegen = int.Parse(value);
		}
		else
		{
			base.setParameter(name, value);
		}
	}

	protected override void onEnter(Creature creature)
	{
        Player? player = creature.getActingPlayer();
		if (creature.isPlayer() && player != null)
		{
			creature.setInsideZone(ZoneId.MOTHER_TREE, true);
			if (_enterMsg != 0)
			{
				player.sendPacket(new SystemMessagePacket(_enterMsg));
			}
		}
	}

	protected override void onExit(Creature creature)
	{
        Player? player = creature.getActingPlayer();
        if (creature.isPlayer() && player != null)
		{
			player.setInsideZone(ZoneId.MOTHER_TREE, false);
			if (_leaveMsg != 0)
			{
				player.sendPacket(new SystemMessagePacket(_leaveMsg));
			}
		}
	}

	/**
	 * @return the _mpRegen
	 */
	public int getMpRegenBonus()
	{
		return _mpRegen;
	}

	/**
	 * @return the _hpRegen
	 */
	public int getHpRegenBonus()
	{
		return _hpRegen;
	}
}