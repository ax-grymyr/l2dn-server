using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExUserViewInfoParameterPacket(Player player): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_USER_VIEW_INFO_PARAMETER);

        short index = 0;

        // Number of parameters.
        writer.WriteInt32(185);

        // ################################## ATTACK ##############################
        // P. Atk. (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PHYSICAL_ATTACK, 0));

        // P. Atk. (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getPAtk());

        // M. Atk. (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.MAGIC_ATTACK, 0));

        // M. Atk. (num)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getMAtk());

        // Soulshot Damage - Activation
        writer.WriteInt16(index++);
        writer.WriteInt32(player.isChargedShot(ShotType.BLESSED_SOULSHOTS) || player.isChargedShot(ShotType.SOULSHOTS)
            ? 10000 + (int)((player.getActiveRubyJewel()?.getBonus() ?? 0) * 1000)
            : 0);

        // Spiritshot Damage - Activation
        writer.WriteInt16(index++);
        writer.WriteInt32(player.isChargedShot(ShotType.BLESSED_SPIRITSHOTS) ||
            player.isChargedShot(ShotType.SPIRITSHOTS)
                ? 10000 + (int)((player.getActiveShappireJewel()?.getBonus() ?? 0) * 1000)
                : 0);

        // Soulshot Damage - Enchanted Weapons
        writer.WriteInt16(index++);
        Item? activeWeaponInstance = player.getActiveWeaponInstance();
        writer.WriteInt32(activeWeaponInstance != null && activeWeaponInstance.isEnchanted()
            ? (int)(activeWeaponInstance.getEnchantLevel() *
                (player.getActiveWeaponItem().getItemGrade() == ItemGrade.S ? 1.6 :
                    player.getActiveWeaponItem().getItemGrade() == ItemGrade.A ? 1.4 :
                    player.getActiveWeaponItem().getItemGrade() == ItemGrade.B ? 0.7 :
                    player.getActiveWeaponItem().getItemGrade() == ItemGrade.C ? 0.4 :
                    player.getActiveWeaponItem().getItemGrade() == ItemGrade.D ? 0.4 : 0) * 100)
            : 0);

        // Spiritshot Damage - Enchanted Weapons
        writer.WriteInt16(index++);
        writer.WriteInt32(activeWeaponInstance != null && activeWeaponInstance.isEnchanted()
            ? (int)(activeWeaponInstance.getEnchantLevel() *
                (player.getActiveWeaponItem().getItemGrade() == ItemGrade.S ? 1.6 :
                    player.getActiveWeaponItem().getItemGrade() == ItemGrade.A ? 1.4 :
                    player.getActiveWeaponItem().getItemGrade() == ItemGrade.B ? 0.7 :
                    player.getActiveWeaponItem().getItemGrade() == ItemGrade.C ? 0.4 :
                    player.getActiveWeaponItem().getItemGrade() == ItemGrade.D ? 0.4 : 0) * 100)
            : 0);

        // Soulshot Damage - Misc.
        writer.WriteInt16(index++);
        BroochJewel? activeRubyJewel = player.getActiveRubyJewel();
        writer.WriteInt32(activeRubyJewel != null ? (int)activeRubyJewel.Value.getBonus() * 1000 : 0);

        // Spiritshot Damage - Misc.
        writer.WriteInt16(index++);
        BroochJewel? activeShappireJewel = player.getActiveShappireJewel();
        writer.WriteInt32(activeShappireJewel != null ? (int)activeShappireJewel.Value.getBonus() * 1000 : 0);

        // Basic PvP Damage
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PVP_PHYSICAL_ATTACK_DAMAGE) * 100);

        // P. Skill Damage in PvP
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PVP_PHYSICAL_SKILL_DAMAGE) * 100);

        // M. Skill Damage in PvP
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PVP_MAGICAL_SKILL_DAMAGE) * 100);

        // Inflicted PvP Damage
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PVP_PHYSICAL_ATTACK_DAMAGE, 0));

        // PvP Damage Decrease Ignore
        writer.WriteInt16(index++);
        writer.WriteInt32(0);

        // Basic PvE Damage
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PVE_PHYSICAL_ATTACK_DAMAGE) * 100);

        // P. Skill Damage in PvE
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PVE_PHYSICAL_SKILL_DAMAGE) * 100);

        // M. Skill Damage in PvE
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PVE_MAGICAL_SKILL_DAMAGE) * 100);

        // PvE Damage
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PVE_DAMAGE_TAKEN) * 100);

        // PvE Damage Decrease Ignore
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PVE_PHYSICAL_SKILL_DAMAGE) * 100);

        // Basic Power
        writer.WriteInt16(index++);
        writer.WriteInt32(0);

        // P. Skill Power
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PHYSICAL_SKILL_POWER) * 100);

        // M. Skill Power
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.MAGICAL_SKILL_POWER) * 100);

        // AoE Skill Damage
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.AREA_OF_EFFECT_DAMAGE_MODIFY) * 100);

        // Damage Bonus - Sword
        writer.WriteInt16(index++);
        writer.WriteInt32(activeWeaponInstance != null && activeWeaponInstance.getItemType() == WeaponType.SWORD
            ? player.getStat().getWeaponBonusPAtk()
            : 0);

        // Damage Bonus - Sword Two hand
        writer.WriteInt16(index++);
        writer.WriteInt32(0);

        // Damage Bonus - Magic Sword
        writer.WriteInt16(index++);
        writer.WriteInt32(0);

        // Damage Bonus - Ancient Sword
        writer.WriteInt16(index++);
        writer.WriteInt32(activeWeaponInstance != null && activeWeaponInstance.getItemType() == WeaponType.ANCIENTSWORD
            ? player.getStat().getWeaponBonusPAtk()
            : 0);

        // Damage Bonus - Dagger
        writer.WriteInt16(index++);
        writer.WriteInt32(activeWeaponInstance != null && activeWeaponInstance.getItemType() == WeaponType.DAGGER
            ? player.getStat().getWeaponBonusPAtk()
            : 0);

        // Damage Bonus - Rapier
        writer.WriteInt16(index++);
        writer.WriteInt32(activeWeaponInstance != null && activeWeaponInstance.getItemType() == WeaponType.RAPIER
            ? player.getStat().getWeaponBonusPAtk()
            : 0);

        // Damage Bonus - Blunt Weapon (one hand)
        writer.WriteInt16(index++);
        writer.WriteInt32(activeWeaponInstance != null &&
            (activeWeaponInstance.getItemType() == WeaponType.ETC ||
                activeWeaponInstance.getItemType() == WeaponType.BLUNT ||
                activeWeaponInstance.getItemType() == WeaponType.DUALBLUNT)
                ? player.getStat().getWeaponBonusPAtk()
                : 0);

        // Damage Bonus - Blunt Weapon (two hand)
        writer.WriteInt16(index++);
        writer.WriteInt32(0);

        // Damage Bonus - Magic Blunt Weapon (one hand)
        writer.WriteInt16(index++);
        writer.WriteInt32(0);

        // Damage Bonus - Magic Blunt Weapon (two hand)
        writer.WriteInt16(index++);
        writer.WriteInt32(0);

        // Damage Bonus - Spear
        writer.WriteInt16(index++);
        writer.WriteInt32(activeWeaponInstance != null && activeWeaponInstance.getItemType() == WeaponType.POLE
            ? player.getStat().getWeaponBonusPAtk()
            : 0);

        // Damage Bonus - Fists
        writer.WriteInt16(index++);
        writer.WriteInt32(activeWeaponInstance != null && (activeWeaponInstance.getItemType() == WeaponType.FIST ||
            activeWeaponInstance.getItemType() == WeaponType.DUALFIST)
            ? player.getStat().getWeaponBonusPAtk()
            : 0);

        // Damage Bonus - Dual Swords
        writer.WriteInt16(index++);
        writer.WriteInt32(activeWeaponInstance != null && activeWeaponInstance.getItemType() == WeaponType.DUAL
            ? player.getStat().getWeaponBonusPAtk()
            : 0);

        // Damage Bonus - Bow
        writer.WriteInt16(index++);
        writer.WriteInt32(activeWeaponInstance != null && (activeWeaponInstance.getItemType() == WeaponType.BOW ||
            activeWeaponInstance.getItemType() == WeaponType.CROSSBOW ||
            activeWeaponInstance.getItemType() == WeaponType.TWOHANDCROSSBOW)
            ? player.getStat().getWeaponBonusPAtk()
            : 0);

        // Damage Bonus - Firearms
        writer.WriteInt16(index++);
        writer.WriteInt32(activeWeaponInstance != null && activeWeaponInstance.getItemType() == WeaponType.PISTOLS
            ? player.getStat().getWeaponBonusPAtk()
            : 0);

        // ################################## DEFENCE ##############################
        // P. Def. (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PHYSICAL_DEFENCE) * 100);

        // P. Def. (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getPDef());

        // M. Def. (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.MAGICAL_DEFENCE) * 100);

        // M. Def. (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getMDef());

        // Soulshot Damage Resistance
        writer.WriteInt16(index++);
        writer.WriteInt32((int)(100 - player.getStat().getValue(Stat.SOULSHOT_RESISTANCE, 1) * 100));

        // Spiritshot Damage Resistance
        writer.WriteInt16(index++);
        writer.WriteInt32((int)(100 - player.getStat().getValue(Stat.SPIRITSHOT_RESISTANCE, 1) * 100));

        // Received basic PvP Damage
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PVP_PHYSICAL_ATTACK_DEFENCE) * 100);

        // Received P. Skill Damage in PvP
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PVP_PHYSICAL_SKILL_DEFENCE) * 100);

        // Received M. Skill Damage in PvP
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PVP_MAGICAL_SKILL_DEFENCE) * 100);

        // Received PvP Damage
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PVP_DAMAGE_TAKEN));

        // PvP Damage Decrease
        writer.WriteInt16(index++);
        writer.WriteInt32(0);

        // Received basic PvE Damage
        writer.WriteInt16(index++);
        writer.WriteInt32(0);

        // Received P. Skill Damage in PvE
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PVE_PHYSICAL_SKILL_DAMAGE) * 100);

        // Received M. Skill Damage in PvE
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PVE_MAGICAL_SKILL_DAMAGE) * 100);

        // Received PvE Damage
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PVE_DAMAGE_TAKEN));

        // PvE Damage Decrease
        writer.WriteInt16(index++);
        writer.WriteInt32(0);

        // Received basic damage power
        writer.WriteInt16(index++);
        writer.WriteInt32(0);

        // P. Skill Power when hit
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PHYSICAL_SKILL_POWER) * 100);

        // M. Skill Power when hit
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.MAGICAL_SKILL_POWER) * 100);

        // Received AoE Skill Damage
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.AREA_OF_EFFECT_DAMAGE_DEFENCE) * 100);

        // Damage Resistance Bonus - One hand Sword
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.SWORD) * 100);

        // Damage Resistance Bonus - Two hand Sword
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.SWORD) * 100);

        // Damage Resistance Bonus - Magic Sword
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.SWORD) * 100);

        // Damage Resistance Bonus - Ancient Sword
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.ANCIENTSWORD) * 100);

        // Damage Resistance Bonus - Dagger
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.DAGGER) * 100);

        // Damage Resistance Bonus - Rapier
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.RAPIER) * 100);

        // Damage Resistance Bonus - Blunt Weapon one hand
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.BLUNT) * 100);

        // Damage Resistance Bonus - Blunt Weapon two hand
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.BLUNT) * 100);

        // Damage Resistance Bonus - Magic Blunt Weapon (one hand)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.BLUNT) * 100);

        // Damage Resistance Bonus - Magic Blunt Weapon (two hand)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.BLUNT) * 100);

        // Damage Resistance Bonus - Spear
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.POLE) * 100);

        // Damage Resistance Bonus - Fists
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.FIST) * 100);

        // Damage Resistance Bonus - Dual Swords
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.DUAL) * 100);

        // Damage Resistance Bonus - Bow
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.BOW) * 100);

        // Damage Resistance Bonus - Firearms
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.PISTOLS) * 100);

        // Shield Defense (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.SHIELD_DEFENCE) * 100);

        // Shield Defence (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getShldDef());

        // Shield Defence Rate
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getStat().getShldDef());

        // M. Damage Resistance (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.ABNORMAL_RESIST_MAGICAL) * 100);

        // M. Damage Resistance (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getMDef());

        // M. Damage Reflection (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.REFLECT_DAMAGE_PERCENT) * 100);

        // M. Damage Reflection Resistance
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.REFLECT_DAMAGE_PERCENT_DEFENSE) * 100);

        // Received Fixed Damage (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.REAL_DAMAGE_RESIST) * 100);

        // Casting Interruption Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.ATTACK_CANCEL) * 100);

        // Casting Interruption Rate (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(0);

        // ################################## ACCURACY ##############################
        // P. Accuracy (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.ACCURACY_COMBAT) * 100);

        // P. Accuracy (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getAccuracy());

        // M. Accuracy (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.ACCURACY_MAGIC) * 100);

        // M. Accuracy (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getMagicAccuracy());

        // Vital Point Attack Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.BLOW_RATE) * 100);

        // Vital Point Attack Rate (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(0);

        // ################################## EVASION ##############################
        // P. Evasion (%)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getEvasionRate() * 100 / Config.Character.MAX_EVASION);

        // P. Evasion (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getEvasionRate());

        // M. Evasion (%)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getMagicEvasionRate() * 100 / Config.Character.MAX_EVASION);

        // M. Evasion (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getMagicEvasionRate());

        // Received Vital Point Attack Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.BLOW_RATE_DEFENCE) * 100);

        // Received Vital Point Attack Rate (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(0);

        // P. Skill Evasion (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.EVASION_RATE) * 100);

        // M. Skill Evasion (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.MAGIC_EVASION_RATE) * 100);

        // ################################## SPEED ##############################
        // Atk. Spd. (%)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getPAtkSpd() * 100 / Config.Character.MAX_PATK_SPEED);

        // Atk. Spd. (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getStat().getPAtkSpd());

        // Casting Spd. (%)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getMAtkSpd() * 100 / Config.Character.MAX_MATK_SPEED);

        // Casting Spd. (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getStat().getMAtkSpd());

        // Speed (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)(player.getMoveSpeed() * 100 / Config.Character.MAX_RUN_SPEED));

        // Speed (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getMoveSpeed());

        // ################################## CRITICAL RATE ##############################
        // Basic Critical Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getStat().getCriticalHit());

        // Basic Critical Rate (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getStat().getCriticalHit());

        // P. Skill Critical Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.CRITICAL_RATE_SKILL) * 100);

        // P. Skill Critical Rate (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getCriticalHit());

        // M. Skill Critical Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.MAGIC_CRITICAL_RATE) * 100);

        // M. Skill Critical Rate (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getMCriticalHit());

        // Received basic Critical Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.CRITICAL_RATE) * 100);

        // Received basic Critical Rate (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(0);

        // Received P. Skill Critical Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.DEFENCE_CRITICAL_RATE) * 100);

        // Received P. Skill Critical Rate (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.DEFENCE_CRITICAL_RATE_ADD));

        // Received M. Skill Critical Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.DEFENCE_MAGIC_CRITICAL_RATE) * 100);

        // Received M. Skill Critical Rate (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.DEFENCE_MAGIC_CRITICAL_RATE_ADD));

        // ################################## CRITICAL DAMAGE ##############################
        // Basic Critical Damage (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.CRITICAL_DAMAGE) * 100);

        // Basic Critical Damage (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getCriticalDmg(1) * 100);

        // P. Skill Critical Damage (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PHYSICAL_SKILL_CRITICAL_DAMAGE) * 100);

        // P. Skill Critical Damage (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.PHYSICAL_SKILL_CRITICAL_DAMAGE_ADD));

        // M. Skill Critical Damage (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.MAGIC_CRITICAL_DAMAGE) * 100);

        // M. Skill Critical Damage (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.MAGIC_CRITICAL_DAMAGE_ADD));

        // Received Basic Critical Damage (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.DEFENCE_CRITICAL_DAMAGE) * 100);

        // Received Basic Critical Damage (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(0);

        // Received P. Skill Critical Damage (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_DAMAGE) * 100);

        // Received P. Skill Critical Damage (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_DAMAGE_ADD));

        // Received M. Skill Critical Damage (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.DEFENCE_MAGIC_CRITICAL_DAMAGE) * 100);

        // Received M. Skill Critical Damage (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.DEFENCE_MAGIC_CRITICAL_DAMAGE_ADD));

        // ################################## RECOVERY ##############################
        // HP ReCovery Potions' Effect (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.ADDITIONAL_POTION_HP) * 100);

        // HP Recovery Potions' Effect (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.ADDITIONAL_POTION_HP) * 100);

        // MP Recovery Potions' Effect (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.ADDITIONAL_POTION_MP) * 100);

        // MP Recovery Potions' Effect (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.ADDITIONAL_POTION_MP) * 100);

        // HP Recovery Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.REGENERATE_HP_RATE) * 100);

        // HP Recovery Rate (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getStat().getHpRegen());

        // HP Recovery Rate while standing (%)
        writer.WriteInt16(index++);
        writer.WriteInt32(!player.isMoving() ? player.getStat().getHpRegen() : 0);

        // HP Recovery Rate while standing (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(!player.isMoving() ? player.getStat().getHpRegen() : 0);

        // HP Recovery Rate while sitting (%)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.isSitting() ? player.getStat().getHpRegen() : 0);

        // HP Recovery Rate while sitting (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.isSitting() ? player.getStat().getHpRegen() : 0);

        // HP Recovery Rate while walking (%)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.isMoving() && !player.isRunning() ? player.getStat().getHpRegen() : 0);

        // HP Recovery Rate while walking (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.isMoving() && !player.isRunning() ? player.getStat().getHpRegen() : 0);

        // HP Recovery Rate while running (%)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.isRunning() ? player.getStat().getHpRegen() : 0);

        // HP Recovery Rate while running (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.isRunning() ? player.getStat().getHpRegen() : 0);

        // MP Recovery Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.REGENERATE_MP_RATE) * 100);

        // MP Recovery Rate (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getStat().getMpRegen());

        // MP Recovery Rate while standing (%)
        writer.WriteInt16(index++);
        writer.WriteInt32(!player.isMoving() ? player.getStat().getMpRegen() : 0);

        // MP Recovery Rate while standing (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(!player.isMoving() ? player.getStat().getMpRegen() : 0);

        // MP Recovery Rate while sitting (%)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.isSitting() ? player.getStat().getMpRegen() : 0);

        // MP Recovery Rate while sitting (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.isSitting() ? player.getStat().getMpRegen() : 0);

        // MP Recovery Rate while walking (%)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.isMoving() && !player.isRunning() ? player.getStat().getMpRegen() : 0);

        // MP Recovery Rate while walking (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.isMoving() && !player.isRunning() ? player.getStat().getMpRegen() : 0);

        // MP Recovery Rate while running (%)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.isRunning() ? player.getStat().getMpRegen() : 0);

        // MP Recovery Rate while running (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.isRunning() ? player.getStat().getMpRegen() : 0);

        // CP Recovery Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.REGENERATE_CP_RATE) * 100);

        // CP Recovery Rate (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.getStat().getCpRegen());

        // CP Recovery Rate while standing (%)
        writer.WriteInt16(index++);
        writer.WriteInt32(!player.isMoving() ? player.getStat().getCpRegen() : 0);

        // CP Recovery Rate while standing (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(!player.isMoving() ? player.getStat().getCpRegen() : 0);

        // CP Recovery Rate while sitting (%)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.isSitting() ? player.getStat().getCpRegen() : 0);

        // CP Recovery Rate while sitting (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.isSitting() ? player.getStat().getCpRegen() : 0);

        // CP Recovery Rate while walking (%)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.isMoving() && !player.isRunning() ? player.getStat().getCpRegen() : 0);

        // CP Recovery Rate while walking (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.isMoving() && !player.isRunning() ? player.getStat().getCpRegen() : 0);

        // CP Recovery Rate while running (%)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.isRunning() ? player.getStat().getCpRegen() : 0);

        // CP Recovery Rate while running (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32(player.isRunning() ? player.getStat().getCpRegen() : 0);

        // ################################## SKILL COOLDOWN ##############################
        // P. Skill Cooldown (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getReuseTypeValue(1) * 100);

        // M. Skill Cooldown (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getReuseTypeValue(2) * 100);

        // Song/ Dance Cooldown (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getReuseTypeValue(3) * 100);

        // ################################## MP CONSUMPTION ##############################
        // P. Skill MP Consumption Decrease (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getMpConsumeTypeValue(1) * 100);

        // M. Skill MP Consumption Decrease (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getMpConsumeTypeValue(2) * 100);

        // Song/ Dance MP Consumption Decrease (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getMpConsumeTypeValue(3) * 100);

        // P. Skill MP Consumption Decrease (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getMpConsumeTypeValue(1) * 100);

        // M. Skill MP Consumption Decrease (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getMpConsumeTypeValue(2) * 100);

        // Song/ Dance MP Consumption Decrease (num.)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getMpConsumeTypeValue(3) * 100);

        // ################################## ANOMALIES ##############################
        // Buff Cancel Resistance Bonus (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.RESIST_DISPEL_BUFF) * 100);

        // Debuff/ Anomaly Resistance Bonus (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getValue(Stat.ABNORMAL_RESIST_MAGICAL) * 100);

        // Unequip Resistance (%)
        writer.WriteInt16(index++);
        writer.WriteInt32(4600); // 46%

        // Paralysis Atk. Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getAttackTrait(TraitType.PARALYZE) * 100);

        // Shock Atk. Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getAttackTrait(TraitType.SHOCK) * 100);

        // Knockback Atk. Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getAttackTrait(TraitType.KNOCKBACK) * 100);

        // Sleep Atk. Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getAttackTrait(TraitType.SLEEP) * 100);

        // Imprisonment Atk. Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getAttackTrait(TraitType.IMPRISON) * 100);

        // Pull Atk. Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getAttackTrait(TraitType.PULL) * 100);

        // Fear Atk. Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getAttackTrait(TraitType.FEAR) * 100);

        // Silence Atk. Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getAttackTrait(TraitType.SILENCE) * 100);

        // Hold Atk. Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getAttackTrait(TraitType.HOLD) * 100);

        // Suppression Atk. Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getAttackTrait(TraitType.SUPPRESSION) * 100);

        // Infection Atk. Rate (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getAttackTrait(TraitType.INFECTION) * 100);

        // Paralysis Resistance (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.PARALYZE) * 100);

        // Shock Resistance (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.SHOCK) * 100);

        // Knockback Resistance (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.KNOCKBACK) * 100);

        // Sleep Resistance (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.SLEEP) * 100);

        // Imprisonment Resistance (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.IMPRISON) * 100);

        // Pull Resistance (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.PULL) * 100);

        // Fear Resistance (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.FEAR) * 100);

        // Silence Resistance (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.SLEEP) * 100);

        // Hold Resistance (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.HOLD) * 100);

        // Suppresion Resistance (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.SUPPRESSION) * 100);

        // Infection Resistance (%)
        writer.WriteInt16(index++);
        writer.WriteInt32((int)player.getStat().getDefenceTrait(TraitType.INFECTION) * 100);
    }
}