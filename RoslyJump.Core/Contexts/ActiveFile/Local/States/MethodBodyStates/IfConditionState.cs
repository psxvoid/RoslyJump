using System;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Syntax;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.Local;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyStates
{
    public class IfConditionState : LocalContextState<ExpressionSyntax>
    {
        private CombinedSyntaxNode[] targets;

        public IfConditionState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
            if (contextNode.MixedNode.GetType() != typeof(IfConditionSyntax))
            {
                throw new ArgumentException(
                    $"Unsupported context node for {nameof(IfConditionState)}. " +
                    $"Actual node type: {contextNode.MixedNode.GetType()}.");
            }

            this.targets = new[] { contextNode };
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            return this.targets;
        }
    }
}
