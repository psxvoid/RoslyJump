using System;
using System.Linq;
using dngrep.core.Extensions.EnumerableExtensions;
using dngrep.core.Queries;
using dngrep.core.Queries.Specifiers;
using dngrep.core.Queries.SyntaxWalkers;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    public class MethodDeclarationState : ClassMemberStateBase<MethodDeclarationSyntax>
    {
        public MethodDeclarationState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            _ = this.ContextNode ?? throw new NullReferenceException(
                $"The context node is not set for {nameof(MethodDeclarationState)}.");

            SyntaxNode? containingClasOrStruct = this.BaseNode.Parent;
            
            _ = containingClasOrStruct ?? throw new InvalidOperationException(
                $"Unable to query the parent for {nameof(MethodDeclarationState)}.");

            SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(
                new SyntaxTreeQueryDescriptor { Target = QueryTarget.Method });

            var walker = new SyntaxTreeQueryWalker(query);

            walker.Visit(containingClasOrStruct);

            if (walker.Results.IsNullOrEmpty())
            {
                throw new InvalidOperationException(
                    $"Unable to query target nodes for {nameof(MethodDeclarationState)}.");
            }

            return walker.Results.Select(x => new CombinedSyntaxNode(x)).ToArray();
        }
    }
}
