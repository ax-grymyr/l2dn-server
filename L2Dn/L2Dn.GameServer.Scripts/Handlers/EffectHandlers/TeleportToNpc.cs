using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Teleport player/party to summoned npc effect implementation.
/// </summary>
[HandlerStringKey("TeleportToNpc")]
public sealed class TeleportToNpc: AbstractEffect
{
    private readonly int _npcId;
    private readonly bool _party;

    public TeleportToNpc(EffectParameterSet parameters)
    {
        _npcId = parameters.GetInt32(XmlSkillEffectParameterType.NpcId);
        _party = parameters.GetBoolean(XmlSkillEffectParameterType.Party, false);
    }

    public override EffectTypes EffectTypes => EffectTypes.TELEPORT_TO_TARGET;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Location? teleLocation = null;
        foreach (Npc npc in effector.getSummonedNpcs())
        {
            if (npc.Id == _npcId)
                teleLocation = npc.Location;
        }

        if (teleLocation != null)
        {
            Party? party = effected.getParty();
            if (_party && party != null)
            {
                foreach (Player member in party.getMembers())
                    Teleport(member, teleLocation.Value);
            }
            else
            {
                Teleport(effected, teleLocation.Value);
            }
        }
    }

    private static void Teleport(Creature effected, Location location)
    {
        if (effected.IsInsideRadius2D(location.Location2D, 900))
        {
            effected.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
            effected.broadcastPacket(new FlyToLocationPacket(effected, location.Location3D, FlyType.DUMMY));
            effected.abortAttack();
            effected.abortCast();
            effected.setXYZ(location.Location3D);
            effected.broadcastPacket(new ValidateLocationPacket(effected));
            effected.revalidateZone(true);
        }
        else
        {
            effected.teleToLocation(location);
        }
    }

    public override int GetHashCode() => HashCode.Combine(_npcId, _party);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._npcId, x._party));
}