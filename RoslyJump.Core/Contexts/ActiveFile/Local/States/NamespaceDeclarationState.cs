using System;
using System.Linq;
using dngrep.core.Extensions.EnumerableExtensions;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    public class NamespaceDeclarationState
        : MixedNestableMemberStateBase<NamespaceDeclarationSyntax>
    {
        public NamespaceDeclarationState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            CombinedSyntaxNode[]? nodes = this.BaseNode.Parent?.ChildNodes()
                .Where(x => x.GetType() == typeof(NamespaceDeclarationSyntax))
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray();

            if (nodes == null || nodes.IsNullOrEmpty())
            {
                throw new InvalidOperationException(
                    $"Unable to find target nodes for {nameof(NamespaceDeclarationState)}.");
            }

            return nodes;
        }

        protected override CombinedSyntaxNode? QueryChildContextNode()
        {
            SyntaxNode? firstKnownChild = this.BaseNode.Members
                .Cast<SyntaxNode>()
                .FirstOrDefault(x => LocalContext.IsKnownNodeType(x)
                    || x is ExpressionSyntax);

            return firstKnownChild == null
                ? (CombinedSyntaxNode?)null
                : firstKnownChild.QueryVirtualAndCombine(NestedBlockVirtualQuery.Instance);
        }
    }
}
