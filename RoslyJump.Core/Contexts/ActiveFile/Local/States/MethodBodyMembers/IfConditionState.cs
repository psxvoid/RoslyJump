using System;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Syntax;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyMembers
{
    public class IfConditionState : IfMemberStateBase<ExpressionSyntax>
    {
        private readonly CombinedSyntaxNode[] targets;

        public IfConditionState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
            if (contextNode.MixedNode.GetType() != typeof(IfConditionSyntax))
            {
                throw new ArgumentException(
                    $"Unsupported context node for {nameof(IfConditionState)}. " +
                    $"Actual node type: {contextNode.MixedNode.GetType()}.");
            }

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
