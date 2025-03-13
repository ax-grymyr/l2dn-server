using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class AttributeSystem
    {
        public static int S_WEAPON_STONE;
        public static int S80_WEAPON_STONE;
        public static int S84_WEAPON_STONE;
        public static int R_WEAPON_STONE;
        public static int R95_WEAPON_STONE;
        public static int R99_WEAPON_STONE;

        public static int S_ARMOR_STONE;
        public static int S80_ARMOR_STONE;
        public static int S84_ARMOR_STONE;
        public static int R_ARMOR_STONE;
        public static int R95_ARMOR_STONE;
        public static int R99_ARMOR_STONE;

        public static int S_WEAPON_CRYSTAL;
        public static int S80_WEAPON_CRYSTAL;
        public static int S84_WEAPON_CRYSTAL;
        public static int R_WEAPON_CRYSTAL;
        public static int R95_WEAPON_CRYSTAL;
        public static int R99_WEAPON_CRYSTAL;

        public static int S_ARMOR_CRYSTAL;
        public static int S80_ARMOR_CRYSTAL;
        public static int S84_ARMOR_CRYSTAL;
        public static int R_ARMOR_CRYSTAL;
        public static int R95_ARMOR_CRYSTAL;
        public static int R99_ARMOR_CRYSTAL;

        public static int S_WEAPON_STONE_SUPER;
        public static int S80_WEAPON_STONE_SUPER;
        public static int S84_WEAPON_STONE_SUPER;
        public static int R_WEAPON_STONE_SUPER;
        public static int R95_WEAPON_STONE_SUPER;
        public static int R99_WEAPON_STONE_SUPER;

        public static int S_ARMOR_STONE_SUPER;
        public static int S80_ARMOR_STONE_SUPER;
        public static int S84_ARMOR_STONE_SUPER;
        public static int R_ARMOR_STONE_SUPER;
        public static int R95_ARMOR_STONE_SUPER;
        public static int R99_ARMOR_STONE_SUPER;

        public static int S_WEAPON_CRYSTAL_SUPER;
        public static int S80_WEAPON_CRYSTAL_SUPER;
        public static int S84_WEAPON_CRYSTAL_SUPER;
        public static int R_WEAPON_CRYSTAL_SUPER;
        public static int R95_WEAPON_CRYSTAL_SUPER;
        public static int R99_WEAPON_CRYSTAL_SUPER;

        public static int S_ARMOR_CRYSTAL_SUPER;
        public static int S80_ARMOR_CRYSTAL_SUPER;
        public static int S84_ARMOR_CRYSTAL_SUPER;
        public static int R_ARMOR_CRYSTAL_SUPER;
        public static int R95_ARMOR_CRYSTAL_SUPER;
        public static int R99_ARMOR_CRYSTAL_SUPER;

        public static int S_WEAPON_JEWEL;
        public static int S80_WEAPON_JEWEL;
        public static int S84_WEAPON_JEWEL;
        public static int R_WEAPON_JEWEL;
        public static int R95_WEAPON_JEWEL;
        public static int R99_WEAPON_JEWEL;

        public static int S_ARMOR_JEWEL;
        public static int S80_ARMOR_JEWEL;
        public static int S84_ARMOR_JEWEL;
        public static int R_ARMOR_JEWEL;
        public static int R95_ARMOR_JEWEL;
        public static int R99_ARMOR_JEWEL;

        public static void Load(string configPath)
        {
            ConfigurationParser parser = new(configPath);
            parser.LoadConfig(FileNames.Configs.AttributeSystemFile);

            S_WEAPON_STONE = parser.getInt("SWeaponStone", 50);
            S80_WEAPON_STONE = parser.getInt("S80WeaponStone", 50);
            S84_WEAPON_STONE = parser.getInt("S84WeaponStone", 50);
            R_WEAPON_STONE = parser.getInt("RWeaponStone", 50);
            R95_WEAPON_STONE = parser.getInt("R95WeaponStone", 50);
            R99_WEAPON_STONE = parser.getInt("R99WeaponStone", 50);
            S_ARMOR_STONE = parser.getInt("SArmorStone", 60);
            S80_ARMOR_STONE = parser.getInt("S80ArmorStone", 80);
            S84_ARMOR_STONE = parser.getInt("S84ArmorStone", 80);
            R_ARMOR_STONE = parser.getInt("RArmorStone", 100);
            R95_ARMOR_STONE = parser.getInt("R95ArmorStone", 100);
            R99_ARMOR_STONE = parser.getInt("R99ArmorStone", 100);
            S_WEAPON_CRYSTAL = parser.getInt("SWeaponCrystal", 30);
            S80_WEAPON_CRYSTAL = parser.getInt("S80WeaponCrystal", 40);
            S84_WEAPON_CRYSTAL = parser.getInt("S84WeaponCrystal", 50);
            R_WEAPON_CRYSTAL = parser.getInt("RWeaponCrystal", 60);
            R95_WEAPON_CRYSTAL = parser.getInt("R95WeaponCrystal", 60);
            R99_WEAPON_CRYSTAL = parser.getInt("R99WeaponCrystal", 60);
            S_ARMOR_CRYSTAL = parser.getInt("SArmorCrystal", 50);
            S80_ARMOR_CRYSTAL = parser.getInt("S80ArmorCrystal", 70);
            S84_ARMOR_CRYSTAL = parser.getInt("S84ArmorCrystal", 80);
            R_ARMOR_CRYSTAL = parser.getInt("RArmorCrystal", 80);
            R95_ARMOR_CRYSTAL = parser.getInt("R95ArmorCrystal", 100);
            R99_ARMOR_CRYSTAL = parser.getInt("R99ArmorCrystal", 100);
            S_WEAPON_STONE_SUPER = parser.getInt("SWeaponStoneSuper", 100);
            S80_WEAPON_STONE_SUPER = parser.getInt("S80WeaponStoneSuper", 100);
            S84_WEAPON_STONE_SUPER = parser.getInt("S84WeaponStoneSuper", 100);
            R_WEAPON_STONE_SUPER = parser.getInt("RWeaponStoneSuper", 100);
            R95_WEAPON_STONE_SUPER = parser.getInt("R95WeaponStoneSuper", 100);
            R99_WEAPON_STONE_SUPER = parser.getInt("R99WeaponStoneSuper", 100);
            S_ARMOR_STONE_SUPER = parser.getInt("SArmorStoneSuper", 100);
            S80_ARMOR_STONE_SUPER = parser.getInt("S80ArmorStoneSuper", 100);
            S84_ARMOR_STONE_SUPER = parser.getInt("S84ArmorStoneSuper", 100);
            R_ARMOR_STONE_SUPER = parser.getInt("RArmorStoneSuper", 100);
            R95_ARMOR_STONE_SUPER = parser.getInt("R95ArmorStoneSuper", 100);
            R99_ARMOR_STONE_SUPER = parser.getInt("R99ArmorStoneSuper", 100);
            S_WEAPON_CRYSTAL_SUPER = parser.getInt("SWeaponCrystalSuper", 80);
            S80_WEAPON_CRYSTAL_SUPER = parser.getInt("S80WeaponCrystalSuper", 90);
            S84_WEAPON_CRYSTAL_SUPER = parser.getInt("S84WeaponCrystalSuper", 100);
            R_WEAPON_CRYSTAL_SUPER = parser.getInt("RWeaponCrystalSuper", 100);
            R95_WEAPON_CRYSTAL_SUPER = parser.getInt("R95WeaponCrystalSuper", 100);
            R99_WEAPON_CRYSTAL_SUPER = parser.getInt("R99WeaponCrystalSuper", 100);
            S_ARMOR_CRYSTAL_SUPER = parser.getInt("SArmorCrystalSuper", 100);
            S80_ARMOR_CRYSTAL_SUPER = parser.getInt("S80ArmorCrystalSuper", 100);
            S84_ARMOR_CRYSTAL_SUPER = parser.getInt("S84ArmorCrystalSuper", 100);
            R_ARMOR_CRYSTAL_SUPER = parser.getInt("RArmorCrystalSuper", 100);
            R95_ARMOR_CRYSTAL_SUPER = parser.getInt("R95ArmorCrystalSuper", 100);
            R99_ARMOR_CRYSTAL_SUPER = parser.getInt("R99ArmorCrystalSuper", 100);
            S_WEAPON_JEWEL = parser.getInt("SWeaponJewel", 100);
            S80_WEAPON_JEWEL = parser.getInt("S80WeaponJewel", 100);
            S84_WEAPON_JEWEL = parser.getInt("S84WeaponJewel", 100);
            R_WEAPON_JEWEL = parser.getInt("RWeaponJewel", 100);
            R95_WEAPON_JEWEL = parser.getInt("R95WeaponJewel", 100);
            R99_WEAPON_JEWEL = parser.getInt("R99WeaponJewel", 100);
            S_ARMOR_JEWEL = parser.getInt("SArmorJewel", 100);
            S80_ARMOR_JEWEL = parser.getInt("S80ArmorJewel", 100);
            S84_ARMOR_JEWEL = parser.getInt("S84ArmorJewel", 100);
            R_ARMOR_JEWEL = parser.getInt("RArmorJewel", 100);
            R95_ARMOR_JEWEL = parser.getInt("R95ArmorJewel", 100);
            R99_ARMOR_JEWEL = parser.getInt("R99ArmorJewel", 100);
        }
    }
}