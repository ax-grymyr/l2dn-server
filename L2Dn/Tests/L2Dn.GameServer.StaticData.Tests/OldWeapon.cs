using System.Globalization;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Items.Types;

namespace L2Dn.GameServer.StaticData.Tests;

/**
 * This class is dedicated to the management of weapons.
 */
public sealed class OldWeapon: OldItemTemplate
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
    public OldWeapon(StatSet set): base(set)
    {
        _type = Enum.Parse<WeaponType>(set.getString("weapon_type", "none"), true);
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
}