using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Give Recommendation effect implementation.
/// </summary>
[AbstractEffectName("GiveRecommendation")]
public sealed class GiveRecommendation: AbstractEffect
{
    private readonly int _amount;

    public GiveRecommendation(EffectParameterSet parameters)
    {
        _amount = parameters.GetInt32(XmlSkillEffectParameterType.Amount, 0);
        if (_amount == 0)
            throw new ArgumentException("amount parameter is missing or set to 0.");
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? target = effected != null && effected.isPlayer() ? (Player)effected : null;
        if (target != null)
        {
            int recommendationsGiven = _amount;
            if (target.getRecomHave() + _amount >= 255)
                recommendationsGiven = 255 - target.getRecomHave();

            if (recommendationsGiven > 0)
            {
                target.setRecomHave(target.getRecomHave() + recommendationsGiven);

                SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_OBTAINED_S1_RECOMMENDATION_S);
                sm.Params.addInt(recommendationsGiven);
                target.sendPacket(sm);
                target.updateUserInfo();
                target.sendPacket(new ExVoteSystemInfoPacket(target));
            }
            else
            {
                Player? player = effector != null && effector.isPlayer() ? (Player)effector : null;
                if (player != null)
                    player.sendPacket(SystemMessageId.NOTHING_HAPPENED);
            }
        }
    }

    public override int GetHashCode() => _amount;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._amount);
}