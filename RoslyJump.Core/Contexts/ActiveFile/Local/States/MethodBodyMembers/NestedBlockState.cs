using System;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.Queries;
using dngrep.core.Queries.SyntaxNodeMatchers.Targets;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Syntax;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates.States;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyMembers
{
    public class NestedBlockState : MethodBodyMemberStateBase<SyntaxNode>
    {
        private static readonly IVirtualNodeQuery[] AllSupportedExceptIfCondition =
            VirtualQueryExtensions.GetAllSupportedQueries()
                .Except(new IVirtualNodeQuery[] { IfConditionVirtualQuery.Instance })
                .ToArray();

        public NestedBlockState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
            if (contextNode.MixedNode.GetType() != typeof(NestedBlockSyntax))
            {
                throw new ArgumentException(
                    $"The provided node is not of type {nameof(NestedBlockSyntax)}.",
                    nameof(contextNode));
            }

            if (!NestedBlockSyntaxNodeMatcher.Instance.Match(contextNode.BaseNode))
            {
                throw new ArgumentException(
                    "The provided base node is not a nested block",
                    nameof(contextNode));
            }
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            SyntaxNode? parent = this.BaseNode.GetContainerNode();

            if (parent == null)
            {
                throw new InvalidOperationException(
                    $"Unable to get the parent to query targets for {nameof(NestedBlockState)}");
            }

            return parent.ChildNodes()
                .Where(x => x.IsContainer())
                .QueryVirtualAndCombine(NestedBlockVirtualQuery.Instance)
                .ToArray();
        }

        protected override CombinedSyntaxNode? QueryParentContextNode()
        {
            SyntaxNode? parent = this.ActiveBaseNode.Parent;

            if (parent is IfStatementSyntax @if && @if.Condition == ActiveBaseNode)
            {
                return @if.Condition.QueryVirtualAndCombine(IfConditionVirtualQuery.Instance);
            }
            else if (parent is ExpressionSyntax e && e.Parent is IfStatementSyntax)
            {
                return e.QueryVirtualAndCombine(AllSupportedExceptIfCondition);
            }

            return base.QueryParentContextNode();
        }

        protected override MethodBodyMemberSiblingState InitSiblingState()
        {
            SyntaxNode? parent = this.ActiveBaseNode.Parent;

            if (parent is IfStatementSyntax @if && @if.Condition == this.ActiveBaseNode)
            {
                return new MethodBodyMemberSiblingState(
                    @if.Condition.QueryVirtualAndCombine(IfConditionVirtualQuery.Instance));
            }

            if (parent is AccessorDeclarationSyntax)
            {
                return new MethodBodyMemberSiblingState(
                    this.ActiveBaseNode.QueryVirtualAndCombine(NestedBlockVirtualQuery.Instance));
            }

            if (parent is PropertyDeclarationSyntax prop && prop.ExpressionBody != null)
            {
                return new MethodBodyMemberSiblingState(
                    this.ActiveBaseNode.QueryVirtualAndCombine(NestedBlockVirtualQuery.Instance));
            }

            return base.InitSiblingState();
        }
    }
}
