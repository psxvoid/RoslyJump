using System;
using System.Linq;
using dngrep.core.Extensions.EnumerableExtensions;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    public class ClassDeclarationState : ClassMemberStateBase<ClassDeclarationSyntax>
    {
        public ClassDeclarationState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            _ = this.ContextNode ?? throw new NullReferenceException(
                $"The context node is not set for {nameof(ClassDeclarationState)}.");

            ClassDeclarationSyntax? parent = this.BaseNode
                .GetFirstParentOfType<ClassDeclarationSyntax>();

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
