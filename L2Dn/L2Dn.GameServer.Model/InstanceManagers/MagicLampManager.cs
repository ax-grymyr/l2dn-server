using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.OutgoingPackets.MagicLamp;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.InstanceManagers;

public sealed class MagicLampManager
{
    private const int REWARD_COUNT = 1;
    private readonly int FLAT_BONUS = 100000;

    public MagicLampManager()
    {
    }

    public void UseMagicLamp(Player player)
    {
        ImmutableArray<MagicLampDataHolder> lamps = MagicLampData.Instance.Lamps;
        if (lamps.IsDefaultOrEmpty)
            return;

        Map<LampType, MagicLampHolder> rewards = new();
        int count = 0;
        while (count == 0) // There should be at least one Magic Lamp reward.
        {
            foreach (MagicLampDataHolder lamp in lamps)
            {
                if (lamp.FromLevel <= player.getLevel() && player.getLevel() <= lamp.ToLevel &&
                    Rnd.get(100d) < lamp.Chance)
                {
                    rewards.GetOrAdd(lamp.Type, _ => new MagicLampHolder(lamp)).inc();
                    if (++count >= REWARD_COUNT)
                    {
                        break;
                    }
                }
            }
        }

        rewards.Values.ForEach(lamp =>
        {
            int exp = (int)lamp.getExp();
            int sp = (int)lamp.getSp();
            player.addExpAndSp(exp, sp);

            LampType lampType = lamp.getType();
            player.sendPacket(new ExMagicLampResultPacket(exp, lampType));
            player.sendPacket(new ExMagicLampInfoPacket(player));
            manageSkill(player, lampType);
        });
    }

    public void addLampExp(Player player, double exp, bool rateModifiers)
    {
        if (Config.MagicLamp.ENABLE_MAGIC_LAMP)
        {
            int lampExp = (int)(exp * player.getStat().getExpBonusMultiplier() * (rateModifiers
                ? Config.MagicLamp.MAGIC_LAMP_CHARGE_RATE * player.getStat().getMul(Stat.MAGIC_LAMP_EXP_RATE, 1)
                : 1));

            int calc = lampExp + player.getLampExp();
            if (player.getLevel() < 64)
            {
                calc += FLAT_BONUS;
            }

            if (calc > Config.MagicLamp.MAGIC_LAMP_MAX_LEVEL_EXP)
            {
                calc %= Config.MagicLamp.MAGIC_LAMP_MAX_LEVEL_EXP;
                UseMagicLamp(player);
            }

            player.setLampExp(calc);
            player.sendPacket(new ExMagicLampInfoPacket(player));
        }
    }

    private void manageSkill(Player player, LampType lampType)
    {
        Skill? lampSkill;

        switch (lampType)
        {
            case LampType.RED:
            {
                lampSkill = CommonSkill.RED_LAMP.getSkill();
                break;
            }
            case LampType.PURPLE:
            {
                lampSkill = CommonSkill.PURPLE_LAMP.getSkill();
                break;
            }
            case LampType.BLUE:
            {
                lampSkill = CommonSkill.BLUE_LAMP.getSkill();
                break;
            }
            case LampType.GREEN:
            {
                lampSkill = CommonSkill.GREEN_LAMP.getSkill();
                break;
            }
            default:
            {
                lampSkill = null;
                break;
            }
        }

        if (lampSkill != null)
        {
            player.breakAttack(); // *TODO Stop Autohunt only for cast a skill?, nope.
            player.breakCast();

            player.doCast(lampSkill);
        }
    }

    public static MagicLampManager getInstance()
    {
        return SingletonHolder.INSTANCE;
    }

    private static class SingletonHolder
    {
        public static readonly MagicLampManager INSTANCE = new MagicLampManager();
    }
}