using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct AttackPacket: IOutgoingPacket
{
	private readonly int _attackerObjId;
	private readonly Location3D _attackerLoc;
	private readonly Location3D _targetLoc;
	private readonly List<Hit> _hits;
	private readonly int _soulshotVisualSubstitute;
	
	/**
	 * @param attacker
	 * @param target
	 */
	public AttackPacket(Creature attacker, Creature target)
	{
		_hits = new List<Hit>();
		_attackerObjId = attacker.getObjectId();
		_attackerLoc = attacker.getLocation().ToLocation3D();
		_targetLoc = target.getLocation().ToLocation3D();
		Player player = attacker.getActingPlayer();
		if (player == null)
		{
			_soulshotVisualSubstitute = 0;
		}
		else
		{
			BroochJewel? activeRuby = player.getActiveRubyJewel();
			BroochJewel? activeShappire = player.getActiveShappireJewel();
			if (activeRuby != null)
			{
				_soulshotVisualSubstitute = activeRuby.Value.GetItemId();
			}
			else if (activeShappire != null)
			{
				_soulshotVisualSubstitute = activeShappire.Value.GetItemId();
			}
			else
			{
				_soulshotVisualSubstitute = 0;
			}
		}
	}
	
	/**
	 * Adds hit to the attack (Attacks such as dual dagger/sword/fist has two hits)
	 * @param hit
	 */
	public void addHit(Hit hit)
	{
		_hits.Add(hit);
	}
	
	public List<Hit> getHits()
	{
		return _hits;
	}
	
	/**
	 * @return {@code true} if current attack contains at least 1 hit.
	 */
	public bool hasHits()
	{
		return !_hits.isEmpty();
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		Hit firstHit = _hits[0];

		writer.WritePacketCode(OutgoingPacketCodes.ATTACK);
		writer.WriteInt32(_attackerObjId);
		writer.WriteInt32(firstHit.getTargetId());
		writer.WriteInt32(_soulshotVisualSubstitute); // Ertheia
		writer.WriteInt32(firstHit.getDamage());
		writer.WriteInt32((int)firstHit.getFlags());
		writer.WriteInt32((int)firstHit.getGrade()); // GOD
		writer.WriteLocation3D(_attackerLoc);
		writer.WriteInt16((short)(_hits.size() - 1));

		for (int index = 1; index < _hits.Count; index++)
		{
			Hit hit = _hits[index];
			writer.WriteInt32(hit.getTargetId());
			writer.WriteInt32(hit.getDamage());
			writer.WriteInt32((int)hit.getFlags());
			writer.WriteInt32((int)hit.getGrade()); // GOD
		}

		writer.WriteLocation3D(_targetLoc);
	}
}