namespace L2Dn.GameServer.Model.Geo;

[Flags]
public enum Direction
{
    None = 0,
    
    East = 0b0001,
    West = 0b0010,
    South = 0b0100,
    North = 0b1000,
    
    NorthEast = North | East,
    NorthWest = North | West,
    SouthEast = South | East,
    SouthWest = South | West,
    
    All = East | West | South | North 
}