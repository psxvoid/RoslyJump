using System;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    public class DestructorSyntaxState : ClassMemberStateBase<DestructorDeclarationSyntax>
    {
        public DestructorSyntaxState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            // a class can only have one constructor
            return new CombinedSyntaxNode[] {
                this.ActiveNode?? throw new InvalidOperationException(
                $"Unable to query the target node for {nameof(DestructorSyntaxState)}")
            };
        }
    }
}
