using System;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    public class ConstructorDeclarationState : ClassMemberStateBase
    {
        public ConstructorDeclarationState(LocalContext context, CombinedSyntaxNode? contextNode) : base(context, contextNode)
        {
            if (contextNode == null ||
                contextNode.Value.BaseNode.GetType() != typeof(ConstructorDeclarationSyntax))
            {
                throw new ArgumentException("Invalid context node for the constructor's state.");
            }
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            CombinedSyntaxNode[]? nodes = this.ContextNode
                ?.BaseNode.GetFirstParentOfType<ClassDeclarationSyntax>()
                ?.ChildNodes()
                .Where(x => x.GetType() == typeof(ConstructorDeclarationSyntax))
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray();

            return nodes ?? Array.Empty<CombinedSyntaxNode>();
        }
    }
}
