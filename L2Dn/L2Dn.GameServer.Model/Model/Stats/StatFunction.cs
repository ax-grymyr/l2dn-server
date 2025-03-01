using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Transforms;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Stats;

public abstract class StatFunction: IStatFunction
{
    public abstract double calc(Creature creature, double? @base, Stat stat);

    protected void throwIfPresent(double? @base)
    {
        if (@base == 0.0)
            return; // TODO hack for now

        if (@base is not null)
        {
            throw new InvalidOperationException("base should not be set for " + GetType().Name);
        }
    }

    protected double validateValue(Creature creature, double value, double minValue, double maxValue)
    {
        if (value > maxValue && !creature.canOverrideCond(PlayerCondOverride.MAX_STATS_VALUE))
        {
            return maxValue;
        }

        return Math.Max(minValue, value);
    }

    protected double calcEnchantBodyPart(Creature creature, params int[] slots)
    {
        double value = 0;
        foreach (int slot in slots)
        {
            Item? item = creature.getInventory()?.getPaperdollItemByItemId(slot);
            if (item != null && item.getEnchantLevel() >= 4 && item.getTemplate().getCrystalTypePlus() == CrystalType.R)
            {
                value += calcEnchantBodyPartBonus(item.getEnchantLevel(), item.getTemplate().isBlessed());
            }
        }
        return value;
    }

    protected virtual double calcEnchantBodyPartBonus(int enchantLevel, bool isBlessed)
    {
        return 0;
    }

    protected double calcWeaponBaseValue(Creature creature, Stat stat)
    {
        double baseTemplateValue = creature.getTemplate().getBaseValue(stat, 0);
        Transform? transformation = creature.getTransformation();
        double baseValue = transformation?.getStats(creature, stat, baseTemplateValue) ?? baseTemplateValue;

        if (creature.isPet())
        {
            Pet pet = (Pet)creature;
            Item? weapon = pet.getActiveWeaponInstance();
            double baseVal = stat == Stat.PHYSICAL_ATTACK ? pet.getPetLevelData().getPetPAtk() :
                stat == Stat.MAGIC_ATTACK ? pet.getPetLevelData().getPetMAtk() : baseTemplateValue;
            baseValue = baseVal + (weapon != null ? weapon.getTemplate().getStats(stat, baseVal) : 0);
        }
        else if (creature.isPlayer() && (!creature.isTransformed() || transformation == null ||
                     transformation.getType() == TransformType.COMBAT ||
                     transformation.getType() == TransformType.MODE_CHANGE))
        {
            Item? weapon = creature.getActiveWeaponInstance();
            baseValue = weapon != null ? weapon.getTemplate().getStats(stat, baseTemplateValue) : baseTemplateValue;
        }

        return baseValue;
    }

    protected double calcWeaponPlusBaseValue(Creature creature, Stat stat)
    {
        double baseTemplateValue = creature.getTemplate().getBaseValue(stat, 0);

        Transform? transform = creature.getTransformation();
        double baseValue = transform != null && !transform.isStance()
            ? transform.getStats(creature, stat, baseTemplateValue)
            : baseTemplateValue;

        if (creature.isPlayable())
        {
            Inventory? inv = creature.getInventory();
            if (inv != null)
            {
                baseValue += inv.getPaperdollCache().getStats(stat);
            }
        }

        return baseValue;
    }

    protected double calcEnchantedItemBonus(Creature creature, Stat stat)
    {
        Player? player = creature.getActingPlayer();
        if (!creature.isPlayer() || player == null)
        {
            return 0;
        }

        double value = 0;
        foreach (Item equippedItem in player.getInventory().getPaperdollItems(x => x.isEnchanted()))
        {
            ItemTemplate item = equippedItem.getTemplate();
            long bodypart = item.getBodyPart();
            if (bodypart == ItemTemplate.SLOT_HAIR || //
                bodypart == ItemTemplate.SLOT_HAIR2 || //
                bodypart == ItemTemplate.SLOT_HAIRALL)
            {
                // TODO: Item after enchant shows pDef, but scroll says mDef increase.
                if (stat != Stat.PHYSICAL_DEFENCE && stat != Stat.MAGICAL_DEFENCE)
                {
                    continue;
                }
            }
            else if (item.getStats(stat, 0) <= 0)
            {
                continue;
            }

            double blessedBonus = item.isBlessed() ? 1.5 : 1;
            int enchant = equippedItem.getEnchantLevel();

            if (player.isInOlympiadMode())
            {
                if (item.isWeapon())
                {
                    if (Config.ALT_OLY_WEAPON_ENCHANT_LIMIT >= 0 && enchant > Config.ALT_OLY_WEAPON_ENCHANT_LIMIT)
                    {
                        enchant = Config.ALT_OLY_WEAPON_ENCHANT_LIMIT;
                    }
                }
                else
                {
                    if (Config.ALT_OLY_ARMOR_ENCHANT_LIMIT >= 0 && enchant > Config.ALT_OLY_ARMOR_ENCHANT_LIMIT)
                    {
                        enchant = Config.ALT_OLY_ARMOR_ENCHANT_LIMIT;
                    }
                }
            }

            if (stat == Stat.MAGICAL_DEFENCE)
            {
                value += calcEnchantmDefBonus(equippedItem, blessedBonus, enchant);
            }
            else if (stat == Stat.PHYSICAL_DEFENCE)
            {
                value += calcEnchantDefBonus(equippedItem, blessedBonus, enchant);
            }
            else if (stat == Stat.MAGIC_ATTACK)
            {
                value += calcEnchantMatkBonus(equippedItem, blessedBonus, enchant);
            }
            else if (stat == Stat.PHYSICAL_ATTACK && equippedItem.isWeapon())
            {
                value += calcEnchantedPAtkBonus(equippedItem, blessedBonus, enchant);
            }
        }
        return value;
    }

    /**
	 * @param item
	 * @param blessedBonus
	 * @param enchant
	 * @return
	 */
    protected static double calcEnchantmDefBonus(Item item, double blessedBonus, int enchant)
    {
        switch (item.getTemplate().getCrystalTypePlus())
        {
            case CrystalType.S:
            {
                return 5 * enchant + 10 * Math.Max(0, enchant - 3);
            }
            case CrystalType.A:
            {
                return 3 * enchant + 4 * Math.Max(0, enchant - 3);
            }
            default:
            {
                return enchant + 3 * Math.Max(0, enchant - 3);
            }
        }
    }

    protected static double calcEnchantDefBonus(Item item, double blessedBonus, int enchant)
    {
        switch (item.getTemplate().getCrystalTypePlus())
        {
            case CrystalType.S:
            {
                return 7 * enchant + 14 * Math.Max(0, enchant - 3);
            }
            case CrystalType.A:
            {
                return 4 * enchant + 5 * Math.Max(0, enchant - 3);
            }
            default:
            {
                return enchant + 3 * Math.Max(0, enchant - 3);
            }
        }
    }

    /**
	 * @param item
	 * @param blessedBonus
	 * @param enchant
	 * @return
	 */
    protected static double calcEnchantMatkBonus(Item item, double blessedBonus, int enchant)
    {
        switch (item.getTemplate().getCrystalTypePlus())
        {
            case CrystalType.S:
            {
                // M. Atk. increases by 10 for all S weapons.
                // Starting at +4, M. Atk. bonus triple.
                return 10 * enchant + 20 * Math.Max(0, enchant - 3);
            }
            case CrystalType.A:
            {
                // M. Atk. increases by 6 for all A weapons.
                // Starting at +4, M. Atk. bonus triple.
                return 6 * enchant + 12 * Math.Max(0, enchant - 3) + getFrostLordWeaponBonus(item, enchant) * 18 * Math.Max(0, enchant - 7);
            }
            case CrystalType.B:
            case CrystalType.C:
            case CrystalType.D:
            {
                // M. Atk. increases by 3 for all B,C,D weapons.
                // Starting at +4, M. Atk. bonus double.
                return 3 * enchant + 3 * Math.Max(0, enchant - 3);
            }
            default:
            {
                // M. Atk. increases by 2 for all weapons. Starting at +4, M. Atk. bonus double.
                // Starting at +4, M. Atk. bonus double.
                return 2 * enchant + 2 * Math.Max(0, enchant - 3);
            }
        }
    }

    /**
	 * @param item
	 * @param blessedBonus
	 * @param enchant
	 * @return
	 */
    protected static double calcEnchantedPAtkBonus(Item item, double blessedBonus, int enchant)
    {
        Weapon? weapon = item.getWeaponItem();
        if (weapon == null)
            return 0;

        switch (item.getTemplate().getCrystalTypePlus())
        {
            case CrystalType.S:
            {
                if (weapon.getBodyPart() == ItemTemplate.SLOT_LR_HAND && weapon.getItemType() != WeaponType.POLE)
                {
                    if (weapon.getItemType().isRanged())
                    {
                        // P. Atk. increases by 31 for S bows.
                        // Starting at +4, P. Atk. bonus double.
                        return 31 * enchant + 62 * Math.Max(0, enchant - 3);
                    }
                    // P. Atk. increases by 19 for two-handed swords, two-handed blunts, dualswords, and two-handed combat weapons.
                    // Starting at +4, P. Atk. bonus double.
                    return 19 * enchant + 38 * Math.Max(0, enchant - 3);
                }
                // P. Atk. increases by 15 for one-handed swords, one-handed blunts, daggers, spears, and other weapons.
                // Starting at +4, P. Atk. bonus double.
                return 15 * enchant + 30 * Math.Max(0, enchant - 3);
            }
            case CrystalType.A:
            {
                if (weapon.getBodyPart() == ItemTemplate.SLOT_LR_HAND && weapon.getItemType() != WeaponType.POLE)
                {
                    if (weapon.getItemType().isRanged())
                    {
                        // P. Atk. increases by 16 for A bows.
                        // Starting at +4, P. Atk. bonus triple.
                        return 16 * enchant + 32 * Math.Max(0, enchant - 3) + getFrostLordWeaponBonus(item, enchant) * 48 * Math.Max(0, enchant - 7);
                    }
                    // P. Atk. increases by 12 for two-handed swords, two-handed blunts, dualswords, and two-handed combat A weapons.
                    // Starting at +4, P. Atk. bonus triple.
                    return 12 * enchant + 24 * Math.Max(0, enchant - 3) + getFrostLordWeaponBonus(item, enchant) * 36 * Math.Max(0, enchant - 7);
                }
                // P. Atk. increases by 10 for one-handed swords, one-handed blunts, daggers, spears, and other A weapons.
                // Starting at +4, P. Atk. bonus triple.
                return 10 * enchant + 20 * Math.Max(0, enchant - 3) + getFrostLordWeaponBonus(item, enchant) * 30 * Math.Max(0, enchant - 7);
            }
            case CrystalType.B:
            case CrystalType.C:
            case CrystalType.D:
            {
                if (weapon.getBodyPart() == ItemTemplate.SLOT_LR_HAND && weapon.getItemType() != WeaponType.POLE)
                {
                    if (weapon.getItemType().isRanged())
                    {
                        // P. Atk. increases by 8 for B,C,D bows.
                        // Starting at +4, P. Atk. bonus double.
                        return 8 * enchant + 8 * Math.Max(0, enchant - 3);
                    }
                    // P. Atk. increases by 5 for two-handed swords, two-handed blunts, dualswords, and two-handed combat B,C,D weapons.
                    // Starting at +4, P. Atk. bonus double.
                    return 5 * enchant + 5 * Math.Max(0, enchant - 3);
                }
                // P. Atk. increases by 4 for one-handed swords, one-handed blunts, daggers, spears, and other B,C,D weapons.
                // Starting at +4, P. Atk. bonus double.
                return 4 * enchant + 4 * Math.Max(0, enchant - 3);
            }
            default:
            {
                if (weapon.getItemType().isRanged())
                {
                    // Bows increase by 4.
                    // Starting at +4, P. Atk. bonus double.
                    return 4 * enchant + 4 * Math.Max(0, enchant - 3);
                }
                // P. Atk. increases by 2 for all weapons with the exception of bows.
                // Starting at +4, P. Atk. bonus double.
                return 2 * enchant + 2 * Math.Max(0, enchant - 3);
            }
        }
    }

    protected static int getFrostLordWeaponBonus(Item item, int enchant)
    {
        return
            (enchant >= 8 && ((95725 <= item.getId() && item.getId() <= 95737) ||
                                (96751 <= item.getId() && item.getId() <= 96763))).CompareTo(false);
    }
}