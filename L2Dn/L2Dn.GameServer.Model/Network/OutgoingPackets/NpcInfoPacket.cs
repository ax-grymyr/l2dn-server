using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct NpcInfoPacket: IOutgoingPacket
{
	private readonly MaskablePacketHelper<NpcInfoType> _helper;
	private readonly Npc _npc;
	private readonly Set<AbnormalVisualEffect> _abnormalVisualEffects;

	private readonly int _clanCrest;
	private readonly int _clanLargeCrest;
	private readonly int _allyCrest;
	private readonly int _allyId;
	private readonly int _clanId;
	private readonly int _statusMask;
	
	public NpcInfoPacket(Npc npc)
	{
		_helper = new(5);
		
		_npc = npc;
		_abnormalVisualEffects = npc.getEffectList().getCurrentAbnormalVisualEffects();

		// These 4 bits are set for some reason. Find out why.
		_helper.AddComponent((NpcInfoType)0x0C);
		_helper.AddComponent((NpcInfoType)0x0D);
		_helper.AddComponent((NpcInfoType)0x14);
		_helper.AddComponent((NpcInfoType)0x15);
		
		_helper.AddComponent(NpcInfoType.ID);
		_helper.AddComponent(NpcInfoType.ATTACKABLE);
		_helper.AddComponent(NpcInfoType.RELATIONS);
		_helper.AddComponent(NpcInfoType.POSITION);
		_helper.AddComponent(NpcInfoType.STOP_MODE);
		_helper.AddComponent(NpcInfoType.MOVE_MODE);
		
		if (npc.getHeading() > 0)
			_helper.AddComponent(NpcInfoType.HEADING);
		
		if ((npc.getStat().getPAtkSpd() > 0) || (npc.getStat().getMAtkSpd() > 0))
			_helper.AddComponent(NpcInfoType.ATK_CAST_SPEED);

		if (npc.getRunSpeed() > 0)
			_helper.AddComponent(NpcInfoType.SPEED_MULTIPLIER);

		if ((npc.getLeftHandItem() > 0) || (npc.getRightHandItem() > 0))
			_helper.AddComponent(NpcInfoType.EQUIPPED);

		if (npc.getTeam() != Team.NONE)
		{
			if (Config.BLUE_TEAM_ABNORMAL_EFFECT != AbnormalVisualEffect.None &&
			    Config.RED_TEAM_ABNORMAL_EFFECT != AbnormalVisualEffect.None)
				_helper.AddComponent(NpcInfoType.ABNORMALS);
			else
				_helper.AddComponent(NpcInfoType.TEAM);
		}

		if (npc.getDisplayEffect() > 0)
			_helper.AddComponent(NpcInfoType.DISPLAY_EFFECT);

		if (npc.isInsideZone(ZoneId.WATER) || npc.isFlying())
			_helper.AddComponent(NpcInfoType.SWIM_OR_FLY);

		if (npc.isFlying())
			_helper.AddComponent(NpcInfoType.FLYING);

		if (npc.getCloneObjId() > 0)
			_helper.AddComponent(NpcInfoType.CLONE);

		if (npc.getMaxHp() > 0)
			_helper.AddComponent(NpcInfoType.MAX_HP);

		if (npc.getMaxMp() > 0)
			_helper.AddComponent(NpcInfoType.MAX_MP);

		if (npc.getCurrentHp() <= npc.getMaxHp())
			_helper.AddComponent(NpcInfoType.CURRENT_HP);

		if (npc.getCurrentMp() <= npc.getMaxMp())
			_helper.AddComponent(NpcInfoType.CURRENT_MP);

		if (npc.getTemplate().isUsingServerSideName())
			_helper.AddComponent(NpcInfoType.NAME);

		if (npc.getTemplate().isUsingServerSideTitle() || (npc.isMonster() && (Config.SHOW_NPC_LEVEL || Config.SHOW_NPC_AGGRESSION)) || npc.isChampion() || npc.isTrap())
			_helper.AddComponent(NpcInfoType.TITLE);

		if (npc.getNameString() != null)
			_helper.AddComponent(NpcInfoType.NAME_NPCSTRINGID);

		if (npc.getTitleString() != null)
			_helper.AddComponent(NpcInfoType.TITLE_NPCSTRINGID);

		if (_npc.getReputation() != 0)
			_helper.AddComponent(NpcInfoType.REPUTATION);

		if (!_abnormalVisualEffects.isEmpty() || npc.isInvisible())
			_helper.AddComponent(NpcInfoType.ABNORMALS);

		if (npc.getEnchantEffect() > 0)
			_helper.AddComponent(NpcInfoType.ENCHANT);

		if (npc.getTransformationDisplayId() > 0)
			_helper.AddComponent(NpcInfoType.TRANSFORMATION);

		if (npc.isShowSummonAnimation())
			_helper.AddComponent(NpcInfoType.SUMMONED);

		int? clanId = npc.getClanId(); 
		if (clanId > 0)
		{
			Clan clan = ClanTable.getInstance().getClan(clanId.Value);
			if ((clan != null) && ((npc.getTemplate().getId() == 34156 /* Clan Stronghold Device */) ||
			                       (!npc.isMonster() && npc.isInsideZone(ZoneId.PEACE))))
			{
				_clanId = clan.getId();
				_clanCrest = clan.getCrestId() ?? 0;
				_clanLargeCrest = clan.getCrestLargeId() ?? 0;
				_allyCrest = clan.getAllyCrestId() ?? 0;
				_allyId = clan.getAllyId() ?? 0;
				_helper.AddComponent(NpcInfoType.CLAN);
			}
		}
        
		_helper.AddComponent(NpcInfoType.PET_EVOLUTION_ID);
		if (npc.getPvpFlag() != PvpFlagStatus.None)
			_helper.AddComponent(NpcInfoType.PVP_FLAG);

		// TODO: Confirm me
		if (npc.isInCombat())
			_statusMask |= 0x01;

		if (npc.isDead())
			_statusMask |= 0x02;

		if (npc.isTargetable())
			_statusMask |= 0x04;

		if (npc.isShowName())
			_statusMask |= 0x08;
		
		if (_statusMask != 0x00)
			_helper.AddComponent(NpcInfoType.VISUAL_STATE);
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		// Localisation related.
		// String[] localisation = null;
		// if (Config.MULTILANG_ENABLE)
		// {
		// 	Player player = getPlayer();
		// 	if (player != null)
		// 	{
		// 		String lang = player.getLang();
		// 		if ((lang != null) && !lang.equals("en"))
		// 		{
		// 			localisation = NpcNameLocalisationData.getInstance().getLocalisation(lang, _npc.getId());
		// 			if (localisation != null)
		// 			{
		// 				if (!_helper.HasComponent(NpcInfoType.NAME))
		// 				{
		// 					addComponentType(NpcInfoType.NAME);
		// 				}
		// 				_blockSize -= _npc.getName().length() * 2;
		// 				_blockSize += localisation[0].length() * 2;
		// 				if (!localisation[1].equals(""))
		// 				{
		// 					if (!_helper.HasComponent(NpcInfoType.TITLE))
		// 					{
		// 						addComponentType(NpcInfoType.TITLE);
		// 					}
		// 					String title = _npc.getTitle();
		// 					_initSize -= title.length() * 2;
		// 					if (title.equals(""))
		// 					{
		// 						_initSize += localisation[1].length() * 2;
		// 					}
		// 					else
		// 					{
		// 						_initSize += title.replace(NpcData.getInstance().getTemplate(_npc.getId()).getTitle(), localisation[1]).length() * 2;
		// 					}
		// 				}
		// 			}
		// 		}
		// 	}
		// }

		// Calculate sizes
		int initSize = 0;
		int blockSize = 0;
		foreach (NpcInfoType npcInfoType in EnumUtil.GetValues<NpcInfoType>())
		{
			if (_helper.HasComponent(npcInfoType))
			{
				switch (npcInfoType)
				{
					case NpcInfoType.ATTACKABLE:
					case NpcInfoType.RELATIONS:
					{
						initSize += npcInfoType.GetBlockLength();
						break;
					}
					case NpcInfoType.TITLE:
					{
						initSize += npcInfoType.GetBlockLength() + _npc.getTitle().Length * 2;
						break;
					}
					case NpcInfoType.NAME:
					{
						blockSize += npcInfoType.GetBlockLength() + _npc.getName().Length * 2;
						break;
					}
					default:
					{
						blockSize += npcInfoType.GetBlockLength();
						break;
					}
				}
			}
		}
		
		writer.WritePacketCode(OutgoingPacketCodes.NPC_INFO);
		writer.WriteInt32(_npc.ObjectId);
		writer.WriteByte(_npc.isShowSummonAnimation() ? (byte)2 : (byte)0); // // 0=teleported 1=default 2=summoned
		writer.WriteInt16(38); // 338 - mask_bits_38
		_helper.WriteMask(writer);

		// Block 1
		writer.WriteByte((byte)initSize);
		if (_helper.HasComponent(NpcInfoType.ATTACKABLE))
			writer.WriteByte(_npc.isAttackable() && _npc is not Guard);

		if (_helper.HasComponent(NpcInfoType.RELATIONS))
			writer.WriteInt64(0);

		if (_helper.HasComponent(NpcInfoType.TITLE))
		{
			string title = _npc.getTitle();
			// Localisation related.
			// if ((localisation != null) && !localisation[1].equals(""))
			// {
			// 	if (title.equals(""))
			// 	{
			// 		title = localisation[1];
			// 	}
			// 	else
			// 	{
			// 		title = title.replace(NpcData.getInstance().getTemplate(_npc.getId()).getTitle(), localisation[1]);
			// 	}
			// }
			
			writer.WriteString(title);
		}
		
		// Block 2
		writer.WriteInt16((short)blockSize);
		if (_helper.HasComponent(NpcInfoType.ID))
			writer.WriteInt32(_npc.getTemplate().getDisplayId() + 1000000);

		if (_helper.HasComponent(NpcInfoType.POSITION))
		{
			writer.WriteInt32(_npc.getX());
			writer.WriteInt32(_npc.getY());
			writer.WriteInt32(_npc.getZ());
		}
		
		if (_helper.HasComponent(NpcInfoType.HEADING))
			writer.WriteInt32(_npc.getHeading());

		if (_helper.HasComponent(NpcInfoType.VEHICLE_ID))
			writer.WriteInt32(0); // Vehicle object id.

		if (_helper.HasComponent(NpcInfoType.ATK_CAST_SPEED))
		{
			writer.WriteInt32(_npc.getPAtkSpd());
			writer.WriteInt32(_npc.getMAtkSpd());
		}
		
		if (_helper.HasComponent(NpcInfoType.SPEED_MULTIPLIER))
		{
			writer.WriteFloat((float)_npc.getStat().getMovementSpeedMultiplier());
			writer.WriteFloat((float)_npc.getStat().getAttackSpeedMultiplier());
		}
		
		if (_helper.HasComponent(NpcInfoType.EQUIPPED))
		{
			writer.WriteInt32(_npc.getRightHandItem());
			writer.WriteInt32(0); // Armor id?
			writer.WriteInt32(_npc.getLeftHandItem());
		}
		
		if (_helper.HasComponent(NpcInfoType.STOP_MODE))
			writer.WriteByte(!_npc.isDead());

		if (_helper.HasComponent(NpcInfoType.MOVE_MODE))
			writer.WriteByte(_npc.isRunning());

		if (_helper.HasComponent(NpcInfoType.SWIM_OR_FLY))
			writer.WriteByte(_npc.isInsideZone(ZoneId.WATER) ? (byte)1 : _npc.isFlying() ? (byte)2 : (byte)0);

		if (_helper.HasComponent(NpcInfoType.TEAM))
			writer.WriteByte((byte)_npc.getTeam());

		if (_helper.HasComponent(NpcInfoType.ENCHANT))
			writer.WriteInt32(_npc.getEnchantEffect());

		if (_helper.HasComponent(NpcInfoType.FLYING))
			writer.WriteInt32(_npc.isFlying());

		if (_helper.HasComponent(NpcInfoType.CLONE))
			writer.WriteInt32(_npc.getCloneObjId()); // Player ObjectId with Decoy

		if (_helper.HasComponent(NpcInfoType.PET_EVOLUTION_ID))
			writer.WriteInt32(0); // Unknown

		if (_helper.HasComponent(NpcInfoType.DISPLAY_EFFECT))
			writer.WriteInt32(_npc.getDisplayEffect());

		if (_helper.HasComponent(NpcInfoType.TRANSFORMATION))
			writer.WriteInt32(_npc.getTransformationDisplayId()); // Transformation ID

		if (_helper.HasComponent(NpcInfoType.CURRENT_HP))
			writer.WriteInt32((int) _npc.getCurrentHp());

		if (_helper.HasComponent(NpcInfoType.CURRENT_MP))
			writer.WriteInt32((int) _npc.getCurrentMp());

		if (_helper.HasComponent(NpcInfoType.MAX_HP))
			writer.WriteInt32(_npc.getMaxHp());

		if (_helper.HasComponent(NpcInfoType.MAX_MP))
			writer.WriteInt32(_npc.getMaxMp());

		if (_helper.HasComponent(NpcInfoType.SUMMONED))
			writer.WriteByte(0); // 2 - do some animation on spawn

		if (_helper.HasComponent(NpcInfoType.FOLLOW_INFO))
		{
			writer.WriteInt32(0);
			writer.WriteInt32(0);
		}
		
		if (_helper.HasComponent(NpcInfoType.NAME))
		{
			//writer.WriteString(localisation != null ? localisation[0] : _npc.getName());
			writer.WriteString(_npc.getName());
		}
		
		if (_helper.HasComponent(NpcInfoType.NAME_NPCSTRINGID))
		{
			NpcStringId? nameString = _npc.getNameString();
			writer.WriteInt32(nameString != null ? (int)nameString : -1); // NPCStringId for name
		}
		
		if (_helper.HasComponent(NpcInfoType.TITLE_NPCSTRINGID))
		{
			NpcStringId? titleString = _npc.getTitleString();
			writer.WriteInt32(titleString != null ? (int)titleString : -1); // NPCStringId for title
		}
		
		if (_helper.HasComponent(NpcInfoType.PVP_FLAG))
			writer.WriteByte((byte)_npc.getPvpFlag()); // PVP flag

		if (_helper.HasComponent(NpcInfoType.REPUTATION))
			writer.WriteInt32(_npc.getReputation()); // Reputation

		if (_helper.HasComponent(NpcInfoType.CLAN))
		{
			writer.WriteInt32(_clanId);
			writer.WriteInt32(_clanCrest);
			writer.WriteInt32(_clanLargeCrest);
			writer.WriteInt32(_allyId);
			writer.WriteInt32(_allyCrest);
		}
		
		if (_helper.HasComponent(NpcInfoType.VISUAL_STATE))
			writer.WriteInt32(_statusMask); // Main writer.WriteByte, Essence writer.WriteInt32. // TODO: classic?

		if (_helper.HasComponent(NpcInfoType.ABNORMALS))
		{
			Team team = Config.BLUE_TEAM_ABNORMAL_EFFECT != AbnormalVisualEffect.None &&
			            Config.RED_TEAM_ABNORMAL_EFFECT != AbnormalVisualEffect.None
				? _npc.getTeam()
				: Team.NONE;
			
			int effectSize = _abnormalVisualEffects.size() + (_npc.isInvisible() ? 1 : 0) + (team != Team.NONE ? 1 : 0);
			writer.WriteInt16((short)effectSize);

			foreach (AbnormalVisualEffect abnormalVisualEffect in _abnormalVisualEffects)
				writer.WriteInt16((short)abnormalVisualEffect);
			
			if (_npc.isInvisible())
				writer.WriteInt16((short)AbnormalVisualEffect.STEALTH);
			
			if (team == Team.BLUE)
			{
				if (Config.BLUE_TEAM_ABNORMAL_EFFECT != AbnormalVisualEffect.None)
					writer.WriteInt16((short)Config.BLUE_TEAM_ABNORMAL_EFFECT);
			}
			else if (team == Team.RED && Config.RED_TEAM_ABNORMAL_EFFECT != AbnormalVisualEffect.None)
				writer.WriteInt16((short)Config.RED_TEAM_ABNORMAL_EFFECT);
		}
	}
}