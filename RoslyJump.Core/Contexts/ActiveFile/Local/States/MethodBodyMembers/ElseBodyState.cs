using System;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Syntax;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyMembers
{
    public class ElseBodyState : IfMemberStateBase<BlockSyntax>
    {
        private readonly CombinedSyntaxNode[] targets;

        public ElseBodyState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
            if (contextNode.MixedNode.GetType() != typeof(ElseBodySyntax))
            {
                throw new ArgumentException(
                    $"Unsupported context node for {nameof(ElseBodyState)}. " +
                    $"Actual node type: {contextNode.MixedNode.GetType()}.");
            }

            targets = new[] { contextNode };
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            return targets;
        }
    }
}
