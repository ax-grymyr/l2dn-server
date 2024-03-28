namespace L2Dn.GameServer.Model.Events;

public enum SubscriptionType
{
    NpcTemplate, // npc ids or levels
    ZoneType, // zone ids
    ItemTemplate, // item template ids
    Castle, // castle ids
    Fortress, // fortress ids
    Olympiad, // singleton container
    InstanceTemplate, // instance template ids
    Global, // singleton container 
    GlobalNpcs, // singleton container
    GlobalMonsters, // singleton container
    GlobalPlayers // singleton container
}