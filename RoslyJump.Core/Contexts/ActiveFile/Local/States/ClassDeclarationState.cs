using System;
using System.Linq;
using dngrep.core.Extensions.EnumerableExtensions;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    public class ClassDeclarationState : FileNamespaceClassMemberStateBase<ClassDeclarationSyntax>
    {
        public ClassDeclarationState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            _ = this.ContextNode ?? throw new NullReferenceException(
                $"The context node is not set for {nameof(ClassDeclarationState)}.");

            SyntaxNode? parent = this.BaseNode.Parent;
            _ = parent ?? throw new InvalidOperationException(
                    $"Unable to find the parent for {nameof(ClassDeclarationState)}.");

            CombinedSyntaxNode[] results = parent.ChildNodes()
                .Where(x => x.GetType() == typeof(ClassDeclarationSyntax))
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray();

            if (results.IsNullOrEmpty())
            {
                throw new InvalidOperationException(
                    $"Unable to find target nodes for {nameof(ClassDeclarationState)}.");
            }

            return results;
        }
    }
}
