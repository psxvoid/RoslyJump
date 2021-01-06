using System;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Syntax;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyMembers
{
    public class TryBodyState : TryMemberStateBase<BlockSyntax>
    {
        private readonly CombinedSyntaxNode[] targets;

        protected override int JumpDownCount => 2;

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
            targets = new[] { contextNode };
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            return targets;
        }

        protected override CombinedSyntaxNode? QueryChildContextNode()
        {
            return ActiveBaseNode.QueryVirtualAndCombine(
                NestedBlockVirtualQuery.Instance);
        }
    }
}
