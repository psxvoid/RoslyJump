using System;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyStates
{
    public class FinallyClauseState : TryMemberStateBase<FinallyClauseSyntax>
    {
        private readonly CombinedSyntaxNode[] targetNodes;

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
    }
}
