using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.Queries;
using dngrep.core.Queries.Specifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.Local;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    public class MethodDeclarationState : LocalContextState
    {
        public MethodDeclarationState(LocalContext context, SyntaxNode contextNode) : base(context, contextNode)
        {
        }

        protected override SyntaxNode[] QueryTargetNodesFunc()
        {
            ClassDeclarationSyntax containingClass =
                this.ContextNode.GetFirstParentOfType<ClassDeclarationSyntax>();

            var query = SyntaxTreeQueryBuilder.From(
                new SyntaxTreeQueryDescriptor { Target = QueryTarget.Method });

            var walker = new SyntaxTreeQueryWalker(query);

            walker.Visit(containingClass);

            return walker.Results.ToArray();
        }
    }
}
