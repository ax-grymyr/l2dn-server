using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public struct RelationChangedPacket: IOutgoingPacket
{
	public const int RELATION_PARTY1 = 1; // party member
	public const int RELATION_PARTY2 = 2; // party member
	public const int RELATION_PARTY3 = 4; // party member
	public const int RELATION_PARTY4 = 8; // party member (for information, see Player.getRelation())
	public const int RELATION_PARTYLEADER = 16; // true if is party leader
	public const int RELATION_HAS_PARTY = 32; // true if is in party
	public const int RELATION_CLAN_MEMBER = 64; // true if is in clan
	public const int RELATION_LEADER = 128; // true if is clan leader
	public const int RELATION_CLAN_MATE = 256; // true if is in same clan
	public const int RELATION_INSIEGE = 512; // true if in siege
	public const int RELATION_ATTACKER = 1024; // true when attacker
	public const int RELATION_ALLY = 2048; // blue siege icon, cannot have if red
	public const int RELATION_ENEMY = 4096; // true when red icon, doesn't matter with blue
	public const int RELATION_DECLARED_WAR = 8192; // single sword
	public const int RELATION_MUTUAL_WAR = 24576; // double swords
	public const int RELATION_ALLY_MEMBER = 65536; // clan is in alliance
	public const int RELATION_TERRITORY_WAR = 524288; // show Territory War icon
	public const int RELATION_DEATH_KNIGHT_PK = 536870912;
	public const long RELATION_SURVEILLANCE = 2147483648L;
	// Masks
	public const byte SEND_DEFAULT = 1;
	public const byte SEND_ONE = 2;
	public const byte SEND_MULTI = 4;

	private struct Relation
	{
		public int ObjId;
		public long RelationCode;
		public bool AutoAttackable;
		public int Reputation;
		public PvpFlagStatus PvpFlag;
	}

	private Relation? _relation;
	private List<Relation>? _relations;

	public RelationChangedPacket(Playable activeChar, long relation, bool autoAttackable)
    {
        _relation = new Relation
        {
            ObjId = activeChar.ObjectId,
            RelationCode = relation,
            AutoAttackable = autoAttackable,
            Reputation = activeChar.getReputation(),
            PvpFlag = activeChar.getPvpFlag(),
        };
    }

	public RelationChangedPacket()
	{
	}

	public void addRelation(Playable activeChar, long relation, bool autoAttackable)
	{
		if (activeChar.isInvisible())
			return;

        Relation r = new Relation
        {
            ObjId = activeChar.ObjectId,
            RelationCode = relation,
            AutoAttackable = autoAttackable,
            Reputation = activeChar.getReputation(),
            PvpFlag = activeChar.getPvpFlag(),
        };

        if (_relations != null)
        {
            _relations.Add(r);
            return;

        }

        if (_relation != null)
        {
            _relations = [_relation.Value, r];
            _relation = null;
            return;
        }

        _relation = r;
    }

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.RELATION_CHANGED);
		if (_relation != null)
		{
			writer.WriteByte(SEND_ONE);
			WriteRelation(writer, _relation.Value);
		}
		else
		{
			writer.WriteByte(SEND_MULTI);
            if (_relations is null)
            {
                writer.WriteInt16(0);
            }
            else
            {
                writer.WriteInt16((short)_relations.Count);
                foreach (Relation r in _relations)
                    WriteRelation(writer, r);
            }
        }
	}

	private static void WriteRelation(PacketBitWriter writer, in Relation relation)
	{
		writer.WriteInt32(relation.ObjId);
		//if ((_mask & SEND_DEFAULT) != SEND_DEFAULT)
		{
			writer.WriteInt64(relation.RelationCode);
			writer.WriteByte(relation.AutoAttackable);
			writer.WriteInt32(relation.Reputation);
			writer.WriteByte((byte)relation.PvpFlag);
		}
	}
}