namespace L2Dn.GameServer.Model.Geo.Internal;

internal static class Constants
{
    /// <summary>
    /// Cells in a block on the x axis.
    /// </summary>
    public const int CellsInBlockX = 8;

    /// <summary>
    /// Cells in a block on the y axis.
    /// </summary>
    public const int CellsInBlockY = 8;

    /// <summary>
    /// Cells in a block
    /// </summary>
    public const int CellsInBlock = CellsInBlockX * CellsInBlockY;
 
    /// <summary>
    /// Blocks in a region on the x axis.
    /// </summary>
    public const int BlocksInRegionX = 256;
    
    /// <summary>
    /// Blocks in a region on the y axis.
    /// </summary>
    public const int BlocksInRegionY = 256;

    /// <summary>
    /// Blocks in a region.
    /// </summary>
    public const int BlocksInRegion = BlocksInRegionX * BlocksInRegionY;

    /// <summary>
    /// Cells in a region on the x axis.
    /// </summary>
    public const int CellsInRegionX = BlocksInRegionX * CellsInBlockX;

    /// <summary>
    /// Cells in a region on the y axis.
    /// </summary>
    public const int CellsInRegionY = BlocksInRegionY * CellsInBlockY;

    /// <summary>
    /// Cells in a region.
    /// </summary>
    public const int CellsInRegion = CellsInRegionX * CellsInRegionY;
    
    /// <summary>
    /// Regions in the world on the x axis
    /// </summary>
    public const int RegionsInWorldX = 32;
    
    /// <summary>
    /// Regions in the world on the y axis
    /// </summary>
    public const int RegionsInWorldY = 32;
    
    /// <summary>
    /// Regions in the world
    /// </summary>
    public const int RegionsInWorld = RegionsInWorldX * RegionsInWorldY; 
    
    /// <summary>
    /// Blocks in the world on the x axis
    /// </summary>
    public const int BlocksInWorldX = RegionsInWorldX * BlocksInRegionX;
    
    /// <summary>
    /// Blocks in the world on the y axis
    /// </summary>
    public const int BlocksInWorldY = RegionsInWorldY * BlocksInRegionY;
    
    /// <summary>
    /// Blocks in the world
    /// </summary>
    public const int BlocksInWorld = BlocksInWorldX * BlocksInWorldY; 

    /// <summary>
    /// Cells in the world on the x axis
    /// </summary>
    public const int CellsInWorldX = BlocksInWorldX * CellsInBlockX;
    
    /// <summary>
    /// Cells in the world on the y axis
    /// </summary>
    public const int CellsInWorldY = BlocksInWorldY * CellsInBlockY;
    
    /// <summary>
    /// Cells in the world
    /// </summary>
    public const long CellsInWorld = (long)CellsInWorldX * CellsInWorldY; 
}