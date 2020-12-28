using System;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Syntax;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyStates
{
    public class TryBodyState : TryMemberStateBase<BlockSyntax>
    {
        private readonly CombinedSyntaxNode[] targets;

        public TryBodyState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
            if (contextNode.MixedNode.GetType() != typeof(TryBodySyntax))
            {
                throw new ArgumentException(
                    $"The provided context node is not supported by {nameof(TryBodyState)}.",
                    nameof(contextNode));
            }

            // try statement can only have a single try block
            this.targets = new[] { contextNode };
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            return this.targets;
        }
    }
}
