using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class VillageMasterOrc: VillageMaster
{
    /**
     * Creates a village master.
     * @param template the village master NPC template
     */
    public VillageMasterOrc(NpcTemplate template): base(template)
    {
    }

    protected sealed override bool checkVillageMasterRace(CharacterClass pClass)
    {
        return pClass.GetRace() == Race.ORC;
    }
}