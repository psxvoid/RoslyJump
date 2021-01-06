using System;
using System.Linq;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyMembers
{
    public class IfStatementState : MethodBodyMemberStateBase<IfStatementSyntax>
    {
        public IfStatementState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            return BaseNode.GetContainerNode()
                ?.ChildNodes()
                .Where(x => x.GetType() == typeof(IfStatementSyntax))
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray()
                ?? throw new InvalidOperationException(
                    $"Unable to query target nodes for {nameof(IfStatementState)}");
        }

        protected override CombinedSyntaxNode? QueryChildContextNode()
        {
            return BaseNode.Condition.QueryVirtualAndCombine(
                IfConditionVirtualQuery.Instance);
        }
    }
}
