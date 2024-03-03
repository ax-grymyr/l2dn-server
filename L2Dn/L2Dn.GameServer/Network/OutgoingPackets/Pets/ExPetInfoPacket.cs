using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Pets;

/**
 * @author Sdw
 */
public readonly struct ExPetInfoPacket: IOutgoingPacket
{
	private readonly MaskablePacketHelper<NpcInfoType> _helper;
	private readonly Summon _summon;
	private readonly Player _attacker;
	private readonly long _relation;
	private readonly int _value;
	private readonly int _initSize = 0;
	private readonly int _blockSize = 0;
	private readonly int _clanCrest = 0;
	private readonly int _clanLargeCrest = 0;
	private readonly int _allyCrest = 0;
	private readonly int _allyId = 0;
	private readonly int _clanId = 0;
	private readonly int _statusMask = 0;
	private readonly string _title;
	private readonly Set<AbnormalVisualEffect> _abnormalVisualEffects;
	
	public ExPetInfoPacket(Summon summon, Player attacker, int value, NpcInfoType addComponent = NpcInfoType.ID)
	{
		_helper = new MaskablePacketHelper<NpcInfoType>(5);
		_helper.AddComponent(addComponent);
		
		_summon = summon;
		_attacker = attacker;
		_relation = (attacker != null) && (summon.getOwner() != null) ? summon.getOwner().getRelation(attacker) : 0;
		_title = (summon.getOwner() != null) && summon.getOwner().isOnline() ? summon.getOwner().getName() : "";
		_value = value;
		_abnormalVisualEffects = summon.getEffectList().getCurrentAbnormalVisualEffects();
		
		// These 4 bits are set for some reason. Find out why.
		_helper.AddComponent((NpcInfoType)0x0C);
		_helper.AddComponent((NpcInfoType)0x0D);
		_helper.AddComponent((NpcInfoType)0x14);
		_helper.AddComponent((NpcInfoType)0x15);

		if (summon.getTemplate().getDisplayId() != summon.getTemplate().getId())
		{
			_helper.AddComponent(NpcInfoType.PET_EVOLUTION_ID);
			_helper.AddComponent(NpcInfoType.NAME);
		}

		_helper.AddComponent(NpcInfoType.ATTACKABLE);
		_helper.AddComponent(NpcInfoType.RELATIONS);
		_helper.AddComponent(NpcInfoType.TITLE);
		_helper.AddComponent(NpcInfoType.ID);
		_helper.AddComponent(NpcInfoType.POSITION);
		_helper.AddComponent(NpcInfoType.STOP_MODE);
		_helper.AddComponent(NpcInfoType.MOVE_MODE);
		_helper.AddComponent(NpcInfoType.PVP_FLAG);
		
		if (summon.getHeading() > 0)
		{
			_helper.AddComponent(NpcInfoType.HEADING);
		}
		if ((summon.getStat().getPAtkSpd() > 0) || (summon.getStat().getMAtkSpd() > 0))
		{
			_helper.AddComponent(NpcInfoType.ATK_CAST_SPEED);
		}
		if (summon.getRunSpeed() > 0)
		{
			_helper.AddComponent(NpcInfoType.SPEED_MULTIPLIER);
		}
		if ((summon.getWeapon() > 0) || (summon.getArmor() > 0))
		{
			_helper.AddComponent(NpcInfoType.EQUIPPED);
		}
		if (summon.getTeam() != Team.NONE)
		{
			_helper.AddComponent(NpcInfoType.TEAM);
		}
		if (summon.isInsideZone(ZoneId.WATER) || summon.isFlying())
		{
			_helper.AddComponent(NpcInfoType.SWIM_OR_FLY);
		}
		if (summon.isFlying())
		{
			_helper.AddComponent(NpcInfoType.FLYING);
		}
		if (summon.getMaxHp() > 0)
		{
			_helper.AddComponent(NpcInfoType.MAX_HP);
		}
		if (summon.getMaxMp() > 0)
		{
			_helper.AddComponent(NpcInfoType.MAX_MP);
		}
		if (summon.getCurrentHp() <= summon.getMaxHp())
		{
			_helper.AddComponent(NpcInfoType.CURRENT_HP);
		}
		if (summon.getCurrentMp() <= summon.getMaxMp())
		{
			_helper.AddComponent(NpcInfoType.CURRENT_MP);
		}
		if (!_abnormalVisualEffects.isEmpty())
		{
			_helper.AddComponent(NpcInfoType.ABNORMALS);
		}
		if (summon.getTemplate().getWeaponEnchant() > 0)
		{
			_helper.AddComponent(NpcInfoType.ENCHANT);
		}
		if (summon.getTransformationDisplayId() > 0)
		{
			_helper.AddComponent(NpcInfoType.TRANSFORMATION);
		}
		if (summon.isShowSummonAnimation())
		{
			_helper.AddComponent(NpcInfoType.SUMMONED);
		}
		if (summon.getReputation() != 0)
		{
			_helper.AddComponent(NpcInfoType.REPUTATION);
		}
		if (summon.getOwner().getClan() != null)
		{
			_clanId = summon.getOwner().getAppearance().getVisibleClanId() ?? 0;
			_clanCrest = summon.getOwner().getAppearance().getVisibleClanCrestId() ?? 0;
			_clanLargeCrest = summon.getOwner().getAppearance().getVisibleClanLargeCrestId() ?? 0;
			_allyCrest = summon.getOwner().getAppearance().getVisibleAllyId() ?? 0;
			_allyId = summon.getOwner().getAppearance().getVisibleAllyCrestId() ?? 0;
			_helper.AddComponent(NpcInfoType.CLAN);
		}
		
		_helper.AddComponent(NpcInfoType.PET_EVOLUTION_ID);
		
		// TODO: Confirm me
		if (summon.isInCombat())
		{
			_statusMask |= 0x01;
		}
		if (summon.isDead())
		{
			_statusMask |= 0x02;
		}
		if (summon.isTargetable())
		{
			_statusMask |= 0x04;
		}
		
		_statusMask |= 0x08; // Show name (current on retail is empty).
		if (_statusMask != 0x00)
		{
			_helper.AddComponent(NpcInfoType.VISUAL_STATE);
		}
		
		// Calculate sizes
		foreach (NpcInfoType npcInfoType in Enum.GetValues<NpcInfoType>())
		{
			switch (npcInfoType)
			{
				case NpcInfoType.ATTACKABLE:
				case NpcInfoType.RELATIONS:
				{
					_initSize += npcInfoType.GetBlockLength();
					break;
				}
				case NpcInfoType.TITLE:
				{
					_initSize += npcInfoType.GetBlockLength() + (_title.Length * 2);
					break;
				}
				case NpcInfoType.NAME:
				{
					_blockSize += npcInfoType.GetBlockLength() + (summon.getName().Length * 2);
					break;
				}
				default:
				{
					_blockSize += npcInfoType.GetBlockLength();
					break;
				}
			}
		}
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_PET_INFO);
		
		writer.WriteInt32(_summon.getObjectId());
		writer.WriteByte((byte)_value); // 0=teleported 1=default 2=summoned
		writer.WriteInt16(38); // 338 - mask_bits_38
		_helper.WriteMask(writer);
		
		// Block 1
		writer.WriteByte((byte)_initSize);
		if (_helper.HasComponent(NpcInfoType.ATTACKABLE))
		{
			writer.WriteByte(_summon.isAutoAttackable(_attacker));
		}
		if (_helper.HasComponent(NpcInfoType.RELATIONS))
		{
			writer.WriteInt64(_relation);
		}
		if (_helper.HasComponent(NpcInfoType.TITLE))
		{
			writer.WriteString(_title);
		}
		// Block 2
		writer.WriteInt16((short)_blockSize);
		if (_helper.HasComponent(NpcInfoType.ID))
		{
			writer.WriteInt32(_summon.getTemplate().getDisplayId() + 1000000);
		}
		if (_helper.HasComponent(NpcInfoType.POSITION))
		{
			writer.WriteInt32(_summon.getX());
			writer.WriteInt32(_summon.getY());
			writer.WriteInt32(_summon.getZ());
		}
		if (_helper.HasComponent(NpcInfoType.HEADING))
		{
			writer.WriteInt32(_summon.getHeading());
		}
		if (_helper.HasComponent(NpcInfoType.VEHICLE_ID))
		{
			writer.WriteInt32(0); // Vehicle object id.
		}
		if (_helper.HasComponent(NpcInfoType.ATK_CAST_SPEED))
		{
			writer.WriteInt32(_summon.getPAtkSpd());
			writer.WriteInt32(_summon.getMAtkSpd());
		}
		if (_helper.HasComponent(NpcInfoType.SPEED_MULTIPLIER))
		{
			writer.WriteFloat((float) _summon.getStat().getMovementSpeedMultiplier());
			writer.WriteFloat((float) _summon.getStat().getAttackSpeedMultiplier());
		}
		if (_helper.HasComponent(NpcInfoType.EQUIPPED))
		{
			writer.WriteInt32(_summon.getWeapon());
			writer.WriteInt32(_summon.getArmor()); // Armor id?
			writer.WriteInt32(0);
		}
		if (_helper.HasComponent(NpcInfoType.STOP_MODE))
		{
			writer.WriteByte(!_summon.isDead());
		}
		if (_helper.HasComponent(NpcInfoType.MOVE_MODE))
		{
			writer.WriteByte(_summon.isRunning());
		}
		if (_helper.HasComponent(NpcInfoType.SWIM_OR_FLY))
		{
			writer.WriteByte((byte)(_summon.isInsideZone(ZoneId.WATER) ? 1 : _summon.isFlying() ? 2 : 0));
		}
		if (_helper.HasComponent(NpcInfoType.TEAM))
		{
			writer.WriteByte((byte)_summon.getTeam());
		}
		if (_helper.HasComponent(NpcInfoType.ENCHANT))
		{
			writer.WriteInt32(_summon.getTemplate().getWeaponEnchant());
		}
		if (_helper.HasComponent(NpcInfoType.FLYING))
		{
			writer.WriteInt32(_summon.isFlying());
		}
		if (_helper.HasComponent(NpcInfoType.CLONE))
		{
			writer.WriteInt32(0); // Player ObjectId with Decoy
		}
		if (_helper.HasComponent(NpcInfoType.PET_EVOLUTION_ID))
		{
			writer.WriteInt32(0); // Unknown
		}
		if (_helper.HasComponent(NpcInfoType.DISPLAY_EFFECT))
		{
			writer.WriteInt32(0);
		}
		if (_helper.HasComponent(NpcInfoType.TRANSFORMATION))
		{
			writer.WriteInt32(_summon.getTransformationDisplayId()); // Transformation ID
		}
		if (_helper.HasComponent(NpcInfoType.CURRENT_HP))
		{
			writer.WriteInt32((int) _summon.getCurrentHp());
		}
		if (_helper.HasComponent(NpcInfoType.CURRENT_MP))
		{
			writer.WriteInt32((int) _summon.getCurrentMp());
		}
		if (_helper.HasComponent(NpcInfoType.MAX_HP))
		{
			writer.WriteInt32(_summon.getMaxHp());
		}
		if (_helper.HasComponent(NpcInfoType.MAX_MP))
		{
			writer.WriteInt32(_summon.getMaxMp());
		}
		if (_helper.HasComponent(NpcInfoType.SUMMONED))
		{
			writer.WriteByte((byte)(_summon.isShowSummonAnimation() ? 2 : 0)); // 2 - do some animation on spawn
		}
		if (_helper.HasComponent(NpcInfoType.FOLLOW_INFO))
		{
			writer.WriteInt32(0);
			writer.WriteInt32(0);
		}
		if (_helper.HasComponent(NpcInfoType.NAME))
		{
			writer.WriteString(_summon.getName());
		}
		if (_helper.HasComponent(NpcInfoType.NAME_NPCSTRINGID))
		{
			writer.WriteInt32(-1); // NPCStringId for name
		}
		if (_helper.HasComponent(NpcInfoType.TITLE_NPCSTRINGID))
		{
			writer.WriteInt32(-1); // NPCStringId for title
		}
		if (_helper.HasComponent(NpcInfoType.PVP_FLAG))
		{
			writer.WriteByte(_summon.getPvpFlag()); // PVP flag
		}
		if (_helper.HasComponent(NpcInfoType.REPUTATION))
		{
			writer.WriteInt32(_summon.getReputation()); // Name color
		}
		if (_helper.HasComponent(NpcInfoType.CLAN))
		{
			writer.WriteInt32(_clanId);
			writer.WriteInt32(_clanCrest);
			writer.WriteInt32(_clanLargeCrest);
			writer.WriteInt32(_allyId);
			writer.WriteInt32(_allyCrest);
		}
		if (_helper.HasComponent(NpcInfoType.VISUAL_STATE))
		{
			writer.WriteInt32(_statusMask); // Main writeC, Essence writeD.
		}
		if (_helper.HasComponent(NpcInfoType.ABNORMALS))
		{
			writer.WriteInt16((short)_abnormalVisualEffects.size());
			foreach (AbnormalVisualEffect abnormalVisualEffect in _abnormalVisualEffects)
			{
				writer.WriteInt16((short)abnormalVisualEffect);
			}
		}
	}
}