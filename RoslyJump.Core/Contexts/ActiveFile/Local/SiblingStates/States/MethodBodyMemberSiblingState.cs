using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.Queries.SyntaxNodeMatchers.Targets;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Syntax;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates.States
{
    public class MethodBodyMemberSiblingState : SiblingStateBase
    {
        private class MethodBodyMemberComparer : IEqualityComparer<CombinedSyntaxNode?>
        {
            private readonly IEqualityComparer<CombinedSyntaxNode?> baseComparer;

            public MethodBodyMemberComparer(IEqualityComparer<CombinedSyntaxNode?> baseComparer)
            {
                this.baseComparer = baseComparer;
            }

            public bool Equals(CombinedSyntaxNode? x, CombinedSyntaxNode? y)
            {
                if (x?.MixedNode is LabeledStatementSyntax label)
                {
                    return x == y || label.Statement == y?.MixedNode;
                }
                else
                {
                    return this.baseComparer.Equals(x, y);
                }
            }

            public int GetHashCode(CombinedSyntaxNode? obj)
            {
                return obj.GetHashCode();
            }
        }

        private static new readonly IEqualityComparer<CombinedSyntaxNode?> ComparerInstance =
            new MethodBodyMemberComparer(SiblingStateBase.ComparerInstance);

        protected override IEqualityComparer<CombinedSyntaxNode?> Comparer => ComparerInstance;

        public MethodBodyMemberSiblingState(CombinedSyntaxNode baseNode) : base(baseNode)
        {
            Type mixedNodeType = baseNode.MixedNode.GetType();

            if (mixedNodeType != typeof(MethodBodyDeclarationSyntax)
                && mixedNodeType != typeof(NestedBlockSyntax)
                && mixedNodeType != typeof(IfConditionSyntax))
            {
                throw new ArgumentException(
                    "The provided node is not a method body nor a nested block. " +
                    $"Actual node type is {mixedNodeType}.");
            }

            if (!MethodBodySyntaxNodeMatcher.Instance.Match(baseNode.BaseNode)
                && !baseNode.BaseNode.IsContainer())
            {
                throw new ArgumentException(
                    "The provided base node is not a method body.",
                    nameof(baseNode));
            }
        }

        protected override CombinedSyntaxNode[] QueryTargetsProtected(CombinedSyntaxNode root)
        {
            if (root.MixedNode is IfConditionSyntax condition)
            {
                return new CombinedSyntaxNode[]
                {
                    condition.Expression.QueryVirtualAndCombine(
                        NestedBlockVirtualQuery.Instance)
                };
            }

            return root.BaseNode
                ?.ChildNodes()
                .QueryVirtualAndCombine(VirtualQueryExtensions.GetAllSupportedQueries())
                .Where(MethodBodyMemberSyntaxNodeMatcher.Instance.Match)
                .GroupBy(x => x.MixedNode.GetType())
                .Select(x => x.First())
                .ToArray()
                ?? Array.Empty<CombinedSyntaxNode>();
        }
    }
}
