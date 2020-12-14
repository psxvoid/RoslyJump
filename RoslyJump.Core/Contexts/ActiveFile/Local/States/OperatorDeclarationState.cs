using System;
using System.Linq;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    public class OperatorDeclarationState : ClassMemberStateBase<OperatorDeclarationSyntax>
    {
        public OperatorDeclarationState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            _ = this.ContextNode ?? throw new NullReferenceException(
                $"The context node is not set for {nameof(OperatorDeclarationState)}.");

            SyntaxNode? containingClasOrStruct = this.BaseNode.Parent;
            
            _ = containingClasOrStruct ?? throw new InvalidOperationException(
                $"Unable to query the parent for {nameof(OperatorDeclarationState)}.");

            CombinedSyntaxNode[] nodes = this.BaseNode.GetContainingParent()
                .ChildNodes()
                .Where(x => x.GetType() == typeof(OperatorDeclarationSyntax))
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray();

            return nodes;
        }
    }
}
