using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("ClassChange")]
public sealed class ClassChange: AbstractEffect
{
    private const int _identityCrisisSkillId = 1570;

    private readonly int _index;

    public ClassChange(EffectParameterSet parameters)
    {
        _index = parameters.GetInt32(XmlSkillEffectParameterType.Index, 0);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (!effected.isPlayer() || player == null)
            return;

        // Executing later otherwise interrupted exception during storeCharBase.
        ThreadPool.schedule(() =>
        {
            if (player.isTransformed() || player.isSubclassLocked() || player.isAffectedBySkill(_identityCrisisSkillId))
            {
                player.sendMessage("You cannot switch your class right now!");
                return;
            }

            Skill? identityCrisis = SkillData.Instance.GetSkill(_identityCrisisSkillId, 1);
            if (identityCrisis != null)
            {
                identityCrisis.ApplyEffects(player, player);
            }

            if (OlympiadManager.getInstance().isRegisteredInComp(player))
            {
                OlympiadManager.getInstance().unRegisterNoble(player);
            }

            CharacterClass activeClass = player.getClassId();
            player.setActiveClass(_index);

            SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_SUCCESSFULLY_SWITCHED_S1_TO_S2);
            msg.Params.addClassId(activeClass);
            msg.Params.addClassId(player.getClassId());
            player.sendPacket(msg);

            player.broadcastUserInfo();
            player.sendStorageMaxCount();
            player.sendPacket(new AcquireSkillListPacket(player));
            player.sendPacket(new ExSubJobInfoPacket(player, SubclassInfoType.CLASS_CHANGED));

            Party? party = player.getParty();
            if (player.isInParty() && party != null)
            {
                // Delete party window for other party members
                party.broadcastToPartyMembers(player, PartySmallWindowDeleteAllPacket.STATIC_PACKET);
                foreach (Player member in party.getMembers())
                {
                    // And re-add
                    if (member != player)
                    {
                        member.sendPacket(new PartySmallWindowAllPacket(member, party));
                    }
                }
            }

            // Stop auto use.
            foreach (Shortcut shortcut in player.getAllShortCuts())
            {
                if (!shortcut.isAutoUse())
                {
                    continue;
                }

                player.removeAutoShortcut(shortcut.getSlot(), shortcut.getPage());

                if (player.getAutoUseSettings().isAutoSkill(shortcut.getId()))
                {
                    Skill? knownSkill = player.getKnownSkill(shortcut.getId());
                    if (knownSkill != null)
                    {
                        if (knownSkill.IsBad)
                        {
                            AutoUseTaskManager.getInstance().removeAutoSkill(player, shortcut.getId());
                        }
                        else
                        {
                            AutoUseTaskManager.getInstance().removeAutoBuff(player, shortcut.getId());
                        }
                    }
                }
                else
                {
                    Item? knownItem = player.getInventory().getItemByObjectId(shortcut.getId());
                    if (knownItem != null)
                    {
                        if (knownItem.isPotion())
                        {
                            AutoUseTaskManager.getInstance().removeAutoPotionItem(player);
                        }
                        else
                        {
                            AutoUseTaskManager.getInstance().removeAutoSupplyItem(player, knownItem.Id);
                        }
                    }
                }
            }

            // Fix Death Knight model animation.
            if (player.isDeathKnight())
            {
                player.transform(101, false);
                ThreadPool.schedule(() => player.stopTransformation(false), 50);
            }
        }, 500);
    }

    public override int GetHashCode() => _index;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._index);
}