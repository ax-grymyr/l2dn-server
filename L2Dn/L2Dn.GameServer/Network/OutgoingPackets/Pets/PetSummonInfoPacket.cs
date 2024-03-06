using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Pets;

/**
 * 12 - wolf, 13 - buffalo, 14 - tiger, 15-kukkabara, 17 - hawk, 16 - dragon
 */
public readonly struct PetSummonInfoPacket: IOutgoingPacket
{
	private readonly Summon _summon;
	private readonly int _value;
	private readonly int _runSpd;
	private readonly int _walkSpd;
	private readonly int _swimRunSpd;
	private readonly int _swimWalkSpd;
	private readonly int _flRunSpd = 0;
	private readonly int _flWalkSpd = 0;
	private readonly int _flyRunSpd;
	private readonly int _flyWalkSpd;
	private readonly double _moveMultiplier;
	private readonly int _maxFed;
	private readonly int _curFed;
	private readonly int _statusMask = 0;
	
	public PetSummonInfoPacket(Summon summon, int value)
	{
		_summon = summon;
		_moveMultiplier = summon.getMovementSpeedMultiplier();
		_runSpd = (int) Math.Round(summon.getRunSpeed() / _moveMultiplier);
		_walkSpd = (int) Math.Round(summon.getWalkSpeed() / _moveMultiplier);
		_swimRunSpd = (int) Math.Round(summon.getSwimRunSpeed() / _moveMultiplier);
		_swimWalkSpd = (int) Math.Round(summon.getSwimWalkSpeed() / _moveMultiplier);
		_flyRunSpd = summon.isFlying() ? _runSpd : 0;
		_flyWalkSpd = summon.isFlying() ? _walkSpd : 0;
		_value = value;
		if (summon.isPet())
		{
			Pet pet = (Pet) _summon;
			_curFed = pet.getCurrentFed(); // how fed it is
			_maxFed = pet.getMaxFed(); // max fed it can be
		}
		else if (summon.isServitor())
		{
			Servitor sum = (Servitor) _summon;
			_curFed = (int)(sum.getLifeTimeRemaining() ?? TimeSpan.Zero).TotalSeconds;
			_maxFed = (int)(sum.getLifeTime() ?? TimeSpan.Zero).TotalSeconds;
		}
		if (summon.isBetrayed())
		{
			_statusMask |= 0x01; // Auto attackable status
		}
		_statusMask |= 0x02; // can be chatted with
		if (summon.isRunning())
		{
			_statusMask |= 0x04;
		}
		if (AttackStanceTaskManager.getInstance().hasAttackStanceTask(summon))
		{
			_statusMask |= 0x08;
		}
		if (summon.isDead())
		{
			_statusMask |= 0x10;
		}
		if (summon.isMountable())
		{
			_statusMask |= 0x20;
		}
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.PET_INFO);
		writer.WriteByte((byte)_summon.getSummonType());
		writer.WriteInt32(_summon.getObjectId());
		writer.WriteInt32(_summon.getTemplate().getDisplayId() + 1000000);
		writer.WriteInt32(_summon.getX());
		writer.WriteInt32(_summon.getY());
		writer.WriteInt32(_summon.getZ());
		writer.WriteInt32(_summon.getHeading());
		writer.WriteInt32(_summon.getStat().getMAtkSpd());
		writer.WriteInt32(_summon.getStat().getPAtkSpd());
		writer.WriteInt16((short)_runSpd);
		writer.WriteInt16((short)_walkSpd);
		writer.WriteInt16((short)_swimRunSpd);
		writer.WriteInt16((short)_swimWalkSpd);
		writer.WriteInt16((short)_flRunSpd);
		writer.WriteInt16((short)_flWalkSpd);
		writer.WriteInt16((short)_flyRunSpd);
		writer.WriteInt16((short)_flyWalkSpd);
		writer.WriteDouble(_moveMultiplier);
		writer.WriteDouble(_summon.getAttackSpeedMultiplier()); // attack speed multiplier
		writer.WriteDouble(_summon.getTemplate().getFCollisionRadius());
		writer.WriteDouble(_summon.getTemplate().getFCollisionHeight());
		writer.WriteInt32(_summon.getWeapon()); // right hand weapon
		writer.WriteInt32(_summon.getArmor()); // body armor
		writer.WriteInt32(0); // left hand weapon
		writer.WriteByte((byte)(_summon.isDead() ? 0 : _summon.isShowSummonAnimation() ? 2 : _value));
		writer.WriteInt32(-1); // High Five NPCString ID
		if (_summon.isPet())
		{
			writer.WriteString(_summon.getName()); // Pet name.
		}
		else
		{
			writer.WriteString(_summon.getTemplate().isUsingServerSideName() ? _summon.getName() : ""); // Summon name.
		}
		writer.WriteInt32(-1); // High Five NPCString ID
		writer.WriteString(_summon.getTitle()); // owner name
		writer.WriteByte(_summon.getPvpFlag()); // confirmed
		writer.WriteInt32(_summon.getReputation()); // confirmed
		writer.WriteInt32(_curFed); // how fed it is
		writer.WriteInt32(_maxFed); // max fed it can be
		writer.WriteInt32((int) _summon.getCurrentHp()); // current hp
		writer.WriteInt32(_summon.getMaxHp()); // max hp
		writer.WriteInt32((int) _summon.getCurrentMp()); // current mp
		writer.WriteInt32(_summon.getMaxMp()); // max mp
		writer.WriteInt64(_summon.getStat().getSp()); // sp
		writer.WriteInt16((short)_summon.getLevel()); // level
		writer.WriteInt64(_summon.getStat().getExp()); // 0% absolute value
		writer.WriteInt64(Math.Min(_summon.getExpForThisLevel(), _summon.getStat().getExp())); // 0% absolute value
		writer.WriteInt64(_summon.getExpForNextLevel()); // 100% absolute value
		writer.WriteInt32(_summon.isPet() ? _summon.getInventory().getTotalWeight() : 0); // weight
		writer.WriteInt32(_summon.getMaxLoad()); // max weight it can carry
		writer.WriteInt32(_summon.getPAtk()); // patk
		writer.WriteInt32(_summon.getPDef()); // pdef
		writer.WriteInt32(_summon.getAccuracy()); // accuracy
		writer.WriteInt32(_summon.getEvasionRate()); // evasion
		writer.WriteInt32(_summon.getCriticalHit()); // critical
		writer.WriteInt32(_summon.getMAtk()); // matk
		writer.WriteInt32(_summon.getMDef()); // mdef
		writer.WriteInt32(_summon.getMagicAccuracy()); // magic accuracy
		writer.WriteInt32(_summon.getMagicEvasionRate()); // magic evasion
		writer.WriteInt32(_summon.getMCriticalHit()); // mcritical
		writer.WriteInt32((int) _summon.getStat().getMoveSpeed()); // speed
		writer.WriteInt32(_summon.getPAtkSpd()); // atkspeed
		writer.WriteInt32(_summon.getMAtkSpd()); // casting speed
		writer.WriteByte(0); // TODO: Check me, might be ride status
		writer.WriteByte((byte)_summon.getTeam()); // Confirmed
		writer.WriteByte((byte)_summon.getSoulShotsPerHit()); // How many soulshots this servitor uses per hit - Confirmed
		writer.WriteByte((byte)_summon.getSpiritShotsPerHit()); // How many spiritshots this servitor uses per hit - - Confirmed
		writer.WriteInt32(-1);
		writer.WriteInt32(0); // "Transformation ID - Confirmed" - Used to bug Fenrir after 64 level.
		writer.WriteByte(0); // Used Summon Points
		writer.WriteByte(0); // Maximum Summon Points

		Set<AbnormalVisualEffect> aves = _summon.getEffectList().getCurrentAbnormalVisualEffects();
		Team team = (Config.BLUE_TEAM_ABNORMAL_EFFECT != null) && (Config.RED_TEAM_ABNORMAL_EFFECT != null) ? _summon.getTeam() : Team.NONE;
		writer.WriteInt16((short)(aves.size() + (_summon.isInvisible() ? 1 : 0) + (team != Team.NONE ? 1 : 0))); // Confirmed
		foreach (AbnormalVisualEffect ave in aves)
		{
			writer.WriteInt16((short)ave); // Confirmed
		}
		
		if (_summon.isInvisible())
		{
			writer.WriteInt16((short)AbnormalVisualEffect.STEALTH);
		}
		
		if (team == Team.BLUE)
		{
			if (Config.BLUE_TEAM_ABNORMAL_EFFECT != null)
			{
				writer.WriteInt16((short)Config.BLUE_TEAM_ABNORMAL_EFFECT);
			}
		}
		else if ((team == Team.RED) && (Config.RED_TEAM_ABNORMAL_EFFECT != null))
		{
			writer.WriteInt16((short)Config.RED_TEAM_ABNORMAL_EFFECT);
		}
		
		writer.WriteByte((byte)_statusMask);
		if (_summon.isPet())
		{
			Pet pet = (Pet) _summon;
			writer.WriteInt32(pet.getPetData().getType());
			writer.WriteInt32((int)pet.getEvolveLevel());
			writer.WriteInt32(pet.getEvolveLevel() == 0 ? -1 : pet.getId());
		}
		else
		{
			writer.WriteInt32(0);
			writer.WriteInt32((int)EvolveLevel.None);
			writer.WriteInt32(0);
		}
	}
}