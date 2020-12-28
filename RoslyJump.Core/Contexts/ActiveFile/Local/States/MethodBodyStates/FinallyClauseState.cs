using System;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyStates
{
    public class FinallyClauseState : TryMemberStateBase<FinallyClauseSyntax>
    {
        private readonly CombinedSyntaxNode[] targetNodes;

        protected override int JumpDownCount => 2;

        public FinallyClauseState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        
        {
            if (contextNode.MixedNode.GetType() != typeof(FinallyClauseSyntax))
            {
                throw new ArgumentException(
                    $"Unsupported context node for {nameof(FinallyClauseState)}.");
            }

            this.targetNodes = new[] { contextNode };
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            return this.targetNodes;
        }

        protected override CombinedSyntaxNode? QueryChildContextNode()
        {
            return this.ActiveBaseNode.Block.QueryVirtualAndCombine(
                NestedBlockVirtualQuery.Instance);
        }
    }
}
