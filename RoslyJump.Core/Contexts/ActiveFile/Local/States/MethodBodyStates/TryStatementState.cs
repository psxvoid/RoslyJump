using System;
using System.Linq;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyStates
{
    public class TryStatementState : MethodBodyMemberStateBase<TryStatementSyntax>
    {
        public TryStatementState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            return this.BaseNode.Parent?.ChildNodes()
                .Where(x => x.GetType() == typeof(TryStatementSyntax))
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray()
                ?? throw new InvalidOperationException(
                    $"Unable to query target nodes for {nameof(TryStatementState)}.");
        }

        protected override CombinedSyntaxNode? QueryChildContextNode()
        {
            return this.ActiveBaseNode.Block.QueryVirtualAndCombine(
                TryBodyVirtualQuery.Instance);
        }
    }
}
