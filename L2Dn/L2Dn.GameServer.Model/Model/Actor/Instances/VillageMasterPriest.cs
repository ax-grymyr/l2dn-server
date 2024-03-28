using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.Model;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class VillageMasterPriest: VillageMaster
{
    /**
     * Creates a village master.
     * @param template the village master NPC template
     */
    public VillageMasterPriest(NpcTemplate template): base(template)
    {
    }

    protected sealed override bool checkVillageMasterRace(CharacterClass pClass)
    {
        return (pClass.GetRace() == Race.HUMAN) || (pClass.GetRace() == Race.ELF);
    }

    protected sealed override bool checkVillageMasterTeachType(CharacterClass pClass)
    {
        return CategoryData.getInstance().isInCategory(CategoryType.CLERIC_GROUP, pClass);
    }
}