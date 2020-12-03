using System;
using dngrep.core.VirtualNodes;
using RoslyJump.Core.Contexts.Local;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    public sealed class InactiveState : LocalContextState
    {
        public InactiveState(LocalContext context) :
            base(context, null)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            return Array.Empty<CombinedSyntaxNode>();
        }

        public override void TransitionTo(CombinedSyntaxNode? syntaxNode, LocalContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            if (syntaxNode == null)
            {
                // the state should remain inactive
                return;
            }

            base.TransitionTo(syntaxNode, context);
        }
    }
}
