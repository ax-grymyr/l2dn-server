using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class ReflectMagic(StatSet @params): AbstractStatAddEffect(@params, Stat.VENGEANCE_SKILL_MAGIC_DAMAGE);