namespace RoslyJump.Package
{
    /// <summary>
    /// Contains command ids for the package. Those values should match values in .vsct file.
    /// </summary>
    public static class CommandIds
    {
#pragma warning disable format
        internal const uint ContextJumpNext         = 0x100;
        internal const uint ContextJumpPrev         = 0x200;
        internal const uint ContextJumpUp           = 0x300;
        internal const uint ContextJumpDown         = 0x400;
        internal const uint ContextJumpNextSibling  = 0x500;
        internal const uint ContextJumpPrevSibling  = 0x600;
#pragma warning restore format
    }
}
