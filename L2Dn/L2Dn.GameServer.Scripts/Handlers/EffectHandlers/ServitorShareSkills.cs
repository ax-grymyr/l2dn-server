using System.Collections.Immutable;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.Network.OutgoingPackets.Pets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class ServitorShareSkills: AbstractEffect
{
    private const int _servitorShareSkillId = 1557;
    private const int _powerfulServitorShareSkillId = 45054;

    // For Powerful servitor share (45054).
    private static readonly ImmutableArray<int> _servitorSharePassiveSkills =
    [
        50189, 50468, 50190, 50353, 50446, 50444, 50555, 50445, 50449, 50448, 50447, 50450,
    ];

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (effected.isPlayer() && player != null)
        {
            if (player.getClient()?.State != GameSessionState.InGame)
            {
                // ThreadPool.schedule(() => onStart(effector, effected, skill, item), 1000);
                return;
            }

            if (!effected.hasServitors())
                return;

            ICollection<L2Dn.GameServer.Model.Actor.Summon> summons = player.getServitors().Values;
            for (int i = 0; i < _servitorSharePassiveSkills.Length; i++)
            {
                int passiveSkillId = _servitorSharePassiveSkills[i];
                BuffInfo? passiveSkillEffect = player.getEffectList().getBuffInfoBySkillId(passiveSkillId);
                if (passiveSkillEffect != null)
                {
                    foreach (L2Dn.GameServer.Model.Actor.Summon s in summons)
                    {
                        s.addSkill(passiveSkillEffect.getSkill());
                        s.broadcastInfo();
                        if (s.isPet())
                        {
                            player.sendPacket(new ExPetSkillListPacket(true, (Pet)s));
                        }
                    }
                }
            }
        }
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        Player? player = effected.getActingPlayer();
        if (!effected.isPlayer() || player == null)
            return;

        if (!effected.hasServitors())
            return;

        ICollection<L2Dn.GameServer.Model.Actor.Summon> summons = player.getServitors().Values;
        for (int i = 0; i < _servitorSharePassiveSkills.Length; i++)
        {
            int passiveSkillId = _servitorSharePassiveSkills[i];
            foreach (L2Dn.GameServer.Model.Actor.Summon s in summons)
            {
                BuffInfo? passiveSkillEffect = s.getEffectList().getBuffInfoBySkillId(passiveSkillId);
                if (passiveSkillEffect != null)
                {
                    s.removeSkill(passiveSkillEffect.getSkill(), true);
                    s.broadcastInfo();
                    if (s.isPet())
                        player.sendPacket(new ExPetSkillListPacket(true, (Pet)s));
                }
            }
        }
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}