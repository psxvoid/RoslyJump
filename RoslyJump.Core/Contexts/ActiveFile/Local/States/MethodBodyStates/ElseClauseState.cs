using System;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyStates
{
    public class ElseClauseState : IfMemberStateBase<ElseClauseSyntax>
    {
        private readonly CombinedSyntaxNode[] targets;

        public ElseClauseState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
            if (contextNode.MixedNode.GetType() != typeof(ElseClauseSyntax))
            {
                throw new ArgumentException(
                    $"The provided context node is not supported by {nameof(ElseClauseState)}.\"" +
                    $"Actual: {contextNode.MixedNode.GetType()}.",
                    nameof(contextNode));
            }

            this.targets = new[] { contextNode };
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            return this.targets;
        }
    }
}
