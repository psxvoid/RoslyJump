using System;
using Microsoft.CodeAnalysis;
using RoslyJump.Core.Contexts.Local;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    public sealed class InactiveState : LocalContextState
    {
        public InactiveState(LocalContext context) :
            base(context, null)
        {
        }

        protected override SyntaxNode[] QueryTargetNodesFunc()
        {
            return Array.Empty<SyntaxNode>();
        }

        public override void TransitionTo(SyntaxNode syntaxNode, LocalContext context)
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
