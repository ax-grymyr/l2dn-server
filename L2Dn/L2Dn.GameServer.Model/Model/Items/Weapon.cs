using System.Globalization;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Npcs;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Items;

/**
 * This class is dedicated to the management of weapons.
 */
public class Weapon: ItemTemplate
{
	private WeaponType _type;
	private bool _isMagicWeapon;
	private int _soulShotCount;
	private int _spiritShotCount;
	private int _mpConsume;
	private int _baseAttackRange;
	private int _baseAttackRadius;
	private int _baseAttackAngle;
	private int _changeWeaponId;

	private int _reducedSoulshot;
	private int _reducedSoulshotChance;

	private int _reducedMpConsume;
	private int _reducedMpConsumeChance;

	private bool _isForceEquip;
	private bool _isAttackWeapon;
	private bool _useWeaponSkillsOnly;

	/**
	 * Constructor for Weapon.
	 * @param set the StatSet designating the set of couples (key,value) characterizing the weapon.
	 */
	public Weapon(StatSet set): base(set)
	{
        _type = Enum.Parse<WeaponType>(set.getString("weapon_type", "none").toUpperCase());
        _type1 = TYPE1_WEAPON_RING_EARRING_NECKLACE;
        _type2 = TYPE2_WEAPON;
        _isMagicWeapon = set.getBoolean("is_magic_weapon", false);
        _soulShotCount = set.getInt("soulshots", 0);
        _spiritShotCount = set.getInt("spiritshots", 0);
        _mpConsume = set.getInt("mp_consume", 0);
        _baseAttackRange = set.getInt("attack_range", 40);
        string[] damageRange = set.getString("damage_range", "").Split(";"); // 0?;0?;fan sector;base attack angle
        if (damageRange.Length >= 4 &&
            int.TryParse(damageRange[2], CultureInfo.InvariantCulture, out int baseAttackRadius) &&
            int.TryParse(damageRange[3], CultureInfo.InvariantCulture, out int baseAttackAngle))
        {
            _baseAttackRadius = baseAttackRadius;
            _baseAttackAngle = baseAttackAngle;
        }
        else
        {
            _baseAttackRadius = 40;
            _baseAttackAngle = 0;
        }

        string[] reducedSoulshots = set.getString("reduced_soulshot", "").Split(",");
        _reducedSoulshotChance = reducedSoulshots.Length == 2 ? int.Parse(reducedSoulshots[0]) : 0;
        _reducedSoulshot = reducedSoulshots.Length == 2 ? int.Parse(reducedSoulshots[1]) : 0;

        string[] reducedMpConsume = set.getString("reduced_mp_consume", "").Split(",");
        _reducedMpConsumeChance = reducedMpConsume.Length == 2 ? int.Parse(reducedMpConsume[0]) : 0;
        _reducedMpConsume = reducedMpConsume.Length == 2 ? int.Parse(reducedMpConsume[1]) : 0;
        _changeWeaponId = set.getInt("change_weaponId", 0);
        _isForceEquip = set.getBoolean("isForceEquip", false);
        _isAttackWeapon = set.getBoolean("isAttackWeapon", true);
        _useWeaponSkillsOnly = set.getBoolean("useWeaponSkillsOnly", false);

        // Check if ranged weapon reuse delay is missing.
        if (_reuseDelay == TimeSpan.Zero && _type.isRanged())
        {
            _reuseDelay = TimeSpan.FromMilliseconds(1500);
        }
	}

	/**
	 * @return the type of Weapon
	 */
	public override ItemType getItemType()
	{
		return _type;
	}

	/**
	 * @return the type of Weapon
	 */
	public WeaponType getWeaponType()
	{
		return _type;
	}

	/**
	 * @return {@code true} if the weapon is magic, {@code false} otherwise.
	 */
	public override bool isMagicWeapon()
	{
		return _isMagicWeapon;
	}

	/**
	 * @return the quantity of SoulShot used.
	 */
	public int getSoulShotCount()
	{
		return _soulShotCount;
	}

	/**
	 * @return the quantity of SpiritShot used.
	 */
	public int getSpiritShotCount()
	{
		return _spiritShotCount;
	}

	/**
	 * @return the reduced quantity of SoultShot used.
	 */
	public int getReducedSoulShot()
	{
		return _reducedSoulshot;
	}

	/**
	 * @return the chance to use Reduced SoultShot.
	 */
	public int getReducedSoulShotChance()
	{
		return _reducedSoulshotChance;
	}

	/**
	 * @return the MP consumption with the weapon.
	 */
	public int getMpConsume()
	{
		return _mpConsume;
	}

	public int getBaseAttackRange()
	{
		return _baseAttackRange;
	}

	public int getBaseAttackRadius()
	{
		return _baseAttackRadius;
	}

	public int getBaseAttackAngle()
	{
		return _baseAttackAngle;
	}

	/**
	 * @return the reduced MP consumption with the weapon.
	 */
	public int getReducedMpConsume()
	{
		return _reducedMpConsume;
	}

	/**
	 * @return the chance to use getReducedMpConsume()
	 */
	public int getReducedMpConsumeChance()
	{
		return _reducedMpConsumeChance;
	}

	/**
	 * @return the Id in which weapon this weapon can be changed.
	 */
	public int getChangeWeaponId()
	{
		return _changeWeaponId;
	}

	/**
	 * @return {@code true} if the weapon is force equip, {@code false} otherwise.
	 */
	public bool isForceEquip()
	{
		return _isForceEquip;
	}

	/**
	 * @return {@code true} if the weapon is attack weapon, {@code false} otherwise.
	 */
	public bool isAttackWeapon()
	{
		return _isAttackWeapon;
	}

	/**
	 * @return {@code true} if the weapon is skills only, {@code false} otherwise.
	 */
	public bool useWeaponSkillsOnly()
	{
		return _useWeaponSkillsOnly;
	}

	/**
	 * @param caster the Creature pointing out the caster
	 * @param target the Creature pointing out the target
	 * @param trigger
	 * @param type
	 */
	public void applyConditionalSkills(Creature caster, Creature target, Skill trigger, ItemSkillType type)
	{
		forEachSkill(type, holder =>
		{
			Skill skill = holder.getSkill();
			if (Rnd.get(100) >= holder.getChance())
			{
				return;
			}

			if (type == ItemSkillType.ON_MAGIC_SKILL)
			{
				// Trigger only if both are good or bad magic.
				if (trigger.IsBad != skill.IsBad)
				{
					return;
				}

				// No Trigger if not Magic Skill or is toggle
				if (trigger.IsMagic != skill.IsMagic)
				{
					return;
				}

				// No Trigger if skill is toggle
				if (trigger.IsToggle)
				{
					return;
				}

				if (skill.IsBad && Formulas.calcShldUse(caster, target) == Formulas.SHIELD_DEFENSE_PERFECT_BLOCK)
				{
					return;
				}
			}

			// Skill condition not met
			if (!skill.CheckCondition(caster, target, true))
			{
				return;
			}

			skill.ActivateSkill(caster, [target]);

			// TODO: Verify if this applies ONLY to ON_MAGIC_SKILL!
			if (type == ItemSkillType.ON_MAGIC_SKILL)
			{
				// notify quests of a skill use
                Player? casterPlayer = caster.getActingPlayer();
				if (caster.isPlayer() && casterPlayer != null)
				{
					World.getInstance().forEachVisibleObjectInRange<Npc>(caster, 1000, npc =>
					{
						if (npc.Events.HasSubscribers<OnNpcSkillSee>())
						{
							npc.Events.NotifyAsync(new OnNpcSkillSee(npc, casterPlayer, skill, false, target));
						}
					});
				}
				if (caster.isPlayer())
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_HAS_BEEN_ACTIVATED);
					sm.Params.addSkillName(skill);
					caster.sendPacket(sm);
				}
			}
		});
	}
}