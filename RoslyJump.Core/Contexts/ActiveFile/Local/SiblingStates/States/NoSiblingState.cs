using System;
using dngrep.core.VirtualNodes;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates.States
{
    public class NoSiblingState : SiblingStateBase
    {
        public NoSiblingState() : base(CombinedSyntaxNode.Empty)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetsProtected(CombinedSyntaxNode root)
        {
            return Array.Empty<CombinedSyntaxNode>();
        }

        public override void Next()
        {
        }

        public override void Prev()
        {
        }
    }
}
