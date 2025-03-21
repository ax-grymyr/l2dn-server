using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Punishment;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Block Action effect implementation.
/// </summary>
public sealed class BlockAction: AbstractEffect
{
    private readonly FrozenSet<int> _blockedActions;

    public BlockAction(StatSet @params)
    {
        string blockedActions = @params.getString("blockedActions");
        _blockedActions = ParseUtil.ParseSet<int>(blockedActions, ',');
    }

    public override bool canStart(Creature effector, Creature effected, Skill skill)
    {
        return effected != null && effected.isPlayer();
    }

    public override bool checkCondition(int id) => !_blockedActions.Contains(id);

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (_blockedActions.Contains(BotReportTable.PARTY_ACTION_BLOCK_ID))
        {
            PunishmentManager.getInstance().startPunishment(new PunishmentTask(0, effected.ObjectId.ToString(),
                PunishmentAffect.CHARACTER, PunishmentType.PARTY_BAN, null, "block action debuff", "system", true));
        }

        if (_blockedActions.Contains(BotReportTable.CHAT_BLOCK_ID))
        {
            PunishmentManager.getInstance().startPunishment(new PunishmentTask(0, effected.ObjectId.ToString(),
                PunishmentAffect.CHARACTER, PunishmentType.CHAT_BAN, null, "block action debuff", "system", true));
        }
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        if (_blockedActions.Contains(BotReportTable.PARTY_ACTION_BLOCK_ID))
        {
            PunishmentManager.getInstance().stopPunishment(effected.ObjectId.ToString(),
                PunishmentAffect.CHARACTER, PunishmentType.PARTY_BAN);
        }

        if (_blockedActions.Contains(BotReportTable.CHAT_BLOCK_ID))
        {
            PunishmentManager.getInstance().stopPunishment(effected.ObjectId.ToString(),
                PunishmentAffect.CHARACTER, PunishmentType.CHAT_BAN);
        }
    }

    public override int GetHashCode() => _blockedActions.GetSetHashCode();

    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._blockedActions.GetSetComparable());
}