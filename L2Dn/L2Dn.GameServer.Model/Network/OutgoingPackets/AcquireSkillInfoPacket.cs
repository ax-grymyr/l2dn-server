using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct AcquireSkillInfoPacket: IOutgoingPacket
{
	/// <summary>
	///
	/// </summary>
	/// <param name="Type">TODO identify</param>
	/// <param name="ItemId">the item Id</param>
	/// <param name="Count">the item count</param>
	/// <param name="Unknown">TODO identify</param>
	private record struct Req(int Type, int ItemId, long Count, int Unknown);

	private readonly Player _player;
    private readonly AcquireSkillType _type;
    private readonly int _id;
    private readonly int _level;
    private readonly long _spCost;
    private readonly List<Req> _reqs;

    /**
	 * Constructor for the acquire skill info object.
	 * @param player
	 * @param skillType the skill learning type.
	 * @param skillLearn the skill learn.
	 */
    public AcquireSkillInfoPacket(Player player, AcquireSkillType skillType, SkillLearn skillLearn)
    {
        _player = player;
        _id = skillLearn.getSkillId();
        _level = skillLearn.getSkillLevel();
        _spCost = skillLearn.getLevelUpSp();
        _type = skillType;
        _reqs = new();

        if (skillType != AcquireSkillType.PLEDGE || Config.Character.LIFE_CRYSTAL_NEEDED)
        {
            foreach (List<ItemHolder> item in skillLearn.getRequiredItems())
            {
                if (!Config.Character.DIVINE_SP_BOOK_NEEDED && _id == (int)CommonSkill.DIVINE_INSPIRATION)
                {
                    continue;
                }

                _reqs.Add(new Req(99, item[0].getId(), item[0].getCount(), 50));
            }
        }
    }

    /**
	 * Special constructor for Alternate Skill Learning system.<br>
	 * Sets a custom amount of SP.
	 * @param player
	 * @param skillType the skill learning type.
	 * @param skillLearn the skill learn.
	 * @param sp the custom SP amount.
	 */
    public AcquireSkillInfoPacket(Player player, AcquireSkillType skillType, SkillLearn skillLearn, int sp)
    {
        _player = player;
        _id = skillLearn.getSkillId();
        _level = skillLearn.getSkillLevel();
        _spCost = sp;
        _type = skillType;
        _reqs = new();
        foreach (List<ItemHolder> item in skillLearn.getRequiredItems())
        {
            _reqs.Add(new Req(99, item[0].getId(), item[0].getCount(), 50));
        }
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.ACQUIRE_SKILL_INFO);

        writer.WriteInt32(_player.getReplacementSkill(_id));
        writer.WriteInt32(_level);
        writer.WriteInt64(_spCost);
        writer.WriteInt32((int)_type);
        writer.WriteInt32(_reqs.Count);
        foreach (Req temp in _reqs)
        {
            writer.WriteInt32(temp.Type);
            writer.WriteInt32(temp.ItemId);
            writer.WriteInt64(temp.Count);
            writer.WriteInt32(temp.Unknown);
        }
    }
}