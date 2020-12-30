using System;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Syntax;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyStates
{
    public class IfBodyState : IfMemberStateBase<BlockSyntax>
    {
        private CombinedSyntaxNode[] targets;

        public IfBodyState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
            if (contextNode.MixedNode.GetType() != typeof(IfBodySyntax))
            {
                throw new ArgumentException(
                    $"Unsupported context node for {nameof(IfBodyState)}. " +
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
