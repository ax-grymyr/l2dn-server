namespace L2Dn.Packages.Unreal;

[Flags]
public enum UObjectFlags: uint
{
    None = 0,

    /// <summary>
    /// Object is transactional.
    /// </summary>
    Transactional = 0x00000001,

    /// <summary>
    /// Object is not reachable on the object graph.
    /// </summary>
    Unreachable = 0x00000002,

    /// <summary>
    /// Object is visible outside its package.
    /// </summary>
    Public = 0x00000004,

    /// <summary>
    /// Temporary import tag in load/save.
    /// </summary>
    TagImp = 0x00000008,

    /// <summary>
    /// Temporary export tag in load/save.
    /// </summary>
    TagExp = 0x00000010,

    /// <summary>
    /// Modified relative to source files.
    /// </summary>
    SourceModified = 0x00000020,

    /// <summary>
    /// Check during garbage collection.
    /// </summary>
    TagGarbage = 0x00000040,

    /// <summary>
    /// During load, indicates object needs loading.
    /// </summary>
    NeedLoad = 0x00000200,

    /// <summary>
    /// A hardcoded name which should be syntax-highlighted.
    /// </summary>
    RF_HighlightedName = 0x00000400, // or RF_EliminateObject(?)

    /// <summary>
    /// In a singular function.
    /// </summary>
    InSingularFunc = 0x00000800, // or RF_RemappedName(?)

    /// <summary>
    /// Suppressed log name.
    /// </summary>
    Suppress = 0x00001000, // or RF_StateChanged(?)

    /// <summary>
    /// Within an EndState call.
    /// </summary>
    InEndState = 0x00002000,

    /// <summary>
    /// Don't save object.
    /// </summary>
    Transient = 0x00004000,

    /// <summary>
    /// Data is being preloaded from file.
    /// </summary>
    PreLoading = 0x00008000,

    /// <summary>
    /// In-file load for client.
    /// </summary>
    LoadForClient = 0x00010000,

    /// <summary>
    /// In-file load for client.
    /// </summary>
    LoadForServer = 0x00020000,

    /// <summary>
    /// In-file load for client.
    /// </summary>
    LoadForEdit = 0x00040000,

    /// <summary>
    /// Keep object around for editing even if unreferenced.
    /// </summary>
    Standalone = 0x00080000,

    /// <summary>
    /// Don't load this object for the game client.
    /// </summary>
    NotForClient = 0x00100000,

    /// <summary>
    /// Don't load this object for the game server.
    /// </summary>
    NotForServer = 0x00200000,

    /// <summary>
    /// Don't load this object for the editor.
    /// </summary>
    NotForEdit = 0x00400000,

    /// <summary>
    /// Object Destroy has already been called.
    /// </summary>
    Destroyed = 0x00800000,

    /// <summary>
    /// Object needs to be postloaded.
    /// </summary>
    NeedPostLoad = 0x01000000,

    /// <summary>
    /// Has execution stack.
    /// </summary>
    HasStack = 0x02000000,

    /// <summary>
    /// Native (UClass only).
    /// </summary>
    Native = 0x04000000,

    /// <summary>
    /// Marked (for debugging).
    /// </summary>
    Marked = 0x08000000,

    /// <summary>
    /// ShutdownAfterError called.
    /// </summary>
    ErrorShutdown = 0x10000000,

    /// <summary>
    /// For debugging Serialize calls.
    /// </summary>
    DebugPostLoad = 0x20000000,

    /// <summary>
    /// For debugging Serialize calls.
    /// </summary>
    DebugSerialize = 0x40000000,

    /// <summary>
    /// For debugging Destroy calls.
    /// </summary>
    DebugDestroy = 0x80000000
}
