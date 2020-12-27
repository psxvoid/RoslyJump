using System;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Syntax;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.Local;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyStates
{
    public class TryBodyState : LocalContextState<BlockSyntax>
    {
        private readonly CombinedSyntaxNode[] targetNodes;

        public TryBodyState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
            if (contextNode.MixedNode.GetType() != typeof(TryBodySyntax))
            {
                throw new ArgumentException(
                    $"The provided context node is not supported by {nameof(TryBodyState)}.",
                    nameof(contextNode));
            }

            // try block can have only a single body
            this.targetNodes = new[] { contextNode };
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            return targetNodes;
        }
    }
}
