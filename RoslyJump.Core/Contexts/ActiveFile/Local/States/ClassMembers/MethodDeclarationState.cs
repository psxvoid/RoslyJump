using System;
using System.Linq;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.ClassMembers
{
    public class MethodDeclarationState : ClassMemberStateBase<MethodDeclarationSyntax>
    {
        public MethodDeclarationState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            return this.BaseNode.Parent?.ChildNodes()
                .Where(x => x.GetType() == typeof(MethodDeclarationSyntax))
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray()
                ?? throw new InvalidOperationException(
                    $"Unable to query target nodes for {nameof(MethodDeclarationState)}.");
        }

        protected override CombinedSyntaxNode? QueryChildContextNode()
        {
            return new CombinedSyntaxNode(this.BaseNode.ParameterList);
        }
    }
}
