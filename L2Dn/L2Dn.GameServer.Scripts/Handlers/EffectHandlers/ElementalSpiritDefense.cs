using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("ElementalSpiritDefense")]
public sealed class ElementalSpiritDefense(EffectParameterSet parameters)
    : AbstractStatEffect(parameters, parameters.GetEnum<ElementalType>(XmlSkillEffectParameterType.Type).getDefenseStat());