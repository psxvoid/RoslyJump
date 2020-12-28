using System;
using System.Linq;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyStates
{
    public class CatchClauseState : TryMemberStateBase<CatchClauseSyntax>
    {
        protected override int JumpDownCount => 2;

        public CatchClauseState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        
        {
            if (contextNode.MixedNode.GetType() != typeof(CatchClauseSyntax))
            {
                throw new ArgumentException(
                    $"Unsupported context node for {nameof(CatchClauseState)}.");
            }
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            return this.ActiveBaseNode.ParentAs<TryStatementSyntax>().Catches
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray();
        }

        protected override CombinedSyntaxNode? QueryChildContextNode()
        {
            return this.ActiveBaseNode.Block.QueryVirtualAndCombine(
                NestedBlockVirtualQuery.Instance);
        }
    }
}
