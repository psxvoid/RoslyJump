using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.Queries;
using dngrep.core.Queries.Specifiers;
using dngrep.core.Queries.SyntaxWalkers;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.Local;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    public class MethodDeclarationState : LocalContextState
    {
        public MethodDeclarationState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            ClassDeclarationSyntax containingClass =
                ((CombinedSyntaxNode)this.ContextNode).Node
                .GetFirstParentOfType<ClassDeclarationSyntax>();

            SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(
                new SyntaxTreeQueryDescriptor { Target = QueryTarget.Method });

            var walker = new SyntaxTreeQueryWalker(query);

            walker.Visit(containingClass);

            return walker.Results.Select(x => new CombinedSyntaxNode(x)).ToArray();
        }
    }
}
