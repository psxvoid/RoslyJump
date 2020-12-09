using System;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.Queries;
using dngrep.core.Queries.Specifiers;
using dngrep.core.Queries.SyntaxWalkers;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    public class MethodDeclarationState : ClassMemberStateBase
    {
        public MethodDeclarationState(LocalContext context, CombinedSyntaxNode? contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            _ = this.ContextNode ?? throw new NullReferenceException(
                "The context node is not set for MethodDeclarationState");

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.
            ClassDeclarationSyntax containingClass =
                ((CombinedSyntaxNode)this.ContextNode).Node
                    .GetFirstParentOfType<ClassDeclarationSyntax>();
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(
                new SyntaxTreeQueryDescriptor { Target = QueryTarget.Method });

            var walker = new SyntaxTreeQueryWalker(query);

            walker.Visit(containingClass);

            return walker.Results.Select(x => new CombinedSyntaxNode(x)).ToArray();
        }
    }
}
