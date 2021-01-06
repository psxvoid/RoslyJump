using System;
using dngrep.core.Queries.SyntaxNodeMatchers.Targets;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.ClassMembers
{
    // TODO: Also support properties lambdas and expressions.
    public class MethodBodyState : MethodMemberStateBase<SyntaxNode>
    {
        public MethodBodyState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
            if (!MethodBodySyntaxNodeMatcher.Instance.Match(contextNode.BaseNode))
            {
                throw new ArgumentException("Unsupported node type.", nameof(contextNode));
            }
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            if (this.ActiveNode == null)
            {
                throw new NullReferenceException(
                    "Active node should be set before querying the parent.");
            }

            // any parent can only have a single body
            return new[] { this.ActiveNode.Value };
        }
    }
}
