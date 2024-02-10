using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class VillageMasterDwarf: VillageMaster
{
    /**
     * Creates a village master.
     * @param template the village master NPC template
     */
    public VillageMasterDwarf(NpcTemplate template): base(template)
    {
    }

    protected sealed override bool checkVillageMasterRace(ClassId pClass)
    {
        return pClass.getRace() == Race.DWARF;
    }
}