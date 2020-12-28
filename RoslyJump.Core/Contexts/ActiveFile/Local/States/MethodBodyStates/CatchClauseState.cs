using System;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyStates
{
    public class CatchClauseState : TryMemberStateBase<CatchClauseSyntax>
    {
        private readonly CombinedSyntaxNode[] targetNodes;

        public CatchClauseState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        
        {
            if (contextNode.MixedNode.GetType() != typeof(CatchClauseSyntax))
            {
                throw new ArgumentException(
                    $"Unsupported context node for {nameof(CatchClauseState)}.");
            }

            this.targetNodes = new[] { contextNode };
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            return this.targetNodes;
        }
    }
}
