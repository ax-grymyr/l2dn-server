using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("ElementalSpiritAttack")]
public sealed class ElementalSpiritAttack(EffectParameterSet parameters)
    : AbstractStatEffect(parameters, parameters.GetEnum<ElementalType>(XmlSkillEffectParameterType.Type).getAttackStat());