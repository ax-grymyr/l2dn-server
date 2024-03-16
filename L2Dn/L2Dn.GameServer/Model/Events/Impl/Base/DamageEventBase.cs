namespace L2Dn.GameServer.Model.Events.Impl.Base;

public abstract class DamageEventBase: TerminateEventBase
{
    public bool OverrideDamage { get; set; }
    public double OverridenDamage { get; set; }    
}