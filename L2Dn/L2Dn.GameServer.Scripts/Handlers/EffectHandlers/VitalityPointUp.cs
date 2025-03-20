using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Vitality Point Up effect implementation.
/// </summary>
[HandlerName("VitalityPointUp")]
public sealed class VitalityPointUp: AbstractEffect
{
    private readonly int _value;

    public VitalityPointUp(EffectParameterSet parameters)
    {
        _value = parameters.GetInt32(XmlSkillEffectParameterType.Value, 0);
    }

    public override EffectTypes EffectTypes => EffectTypes.VITALITY_POINT_UP;

    public override bool CanStart(Creature effector, Creature effected, Skill skill)
    {
        return effected != null && effected.isPlayer();
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (player == null)
            return;

        player.updateVitalityPoints(_value, false, false);

        if (!player.isSubclassLocked())
        {
            UserInfoPacket ui = new UserInfoPacket(player);
            ui.AddComponentType(UserInfoType.VITA_FAME);
            player.sendPacket(ui);
        }

        // Send item list to update vitality items with red icons in inventory.
        ThreadPool.schedule(() =>
        {
            List<Item> items = [];
            foreach (Item i in player.getInventory().getItems())
            {
                if (i.getTemplate().hasSkills())
                {
                    foreach (ItemSkillHolder s in i.getTemplate().getAllSkills())
                    {
                        if (s.getSkill().HasEffectType(EffectTypes.VITALITY_POINT_UP))
                        {
                            items.Add(i);
                            break;
                        }
                    }
                }
            }

            if (items.Count != 0)
            {
                InventoryUpdatePacket iu = new InventoryUpdatePacket(items);
                player.sendInventoryUpdate(iu);
            }
        }, 1000);
    }

    public override int GetHashCode() => _value;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._value);
}