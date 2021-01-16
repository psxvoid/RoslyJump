using System;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates.States;
using RoslyJump.Core.Contexts.Local;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates
{
    public abstract class MethodBodyMemberStateBase<TNode>
        : LocalContextState<TNode, MethodBodyMemberSiblingState>
        where TNode : SyntaxNode
    {
        protected override int JumpUpCount =>
            (this.ContextNode?.BaseNode.Parent is BlockSyntax block
                && block.Parent is AccessorDeclarationSyntax)
            || (this.ContextNode?.BaseNode.Parent is ArrowExpressionClauseSyntax arrowExpression
                && arrowExpression.Parent is PropertyDeclarationSyntax)
            ? 2
            : base.JumpDownCount;

        protected MethodBodyMemberStateBase(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode? QueryParentContextNode()
        {
            SyntaxNode? parent = this.ContextNode?.BaseNode.GetContainerNode();

            return parent?.QueryVirtualAndCombine(
                VirtualQueryExtensions.GetAllSupportedQueries())
                ?? throw new InvalidOperationException(
                    $"Unable to query the parent context for {this.GetType()}");
        }

        protected override MethodBodyMemberSiblingState InitSiblingState()
        {
            SyntaxNode? siblingParent = this.BaseNode.GetContainerNode();

            _ = siblingParent ?? throw new InvalidOperationException(
                "Unable to get the parent class or struct node.");

            CombinedSyntaxNode combinedSiblingParent = siblingParent.QueryVirtualAndCombine(
                MethodBodyVirtualQuery.Instance,
                NestedBlockVirtualQuery.Instance);

            return new MethodBodyMemberSiblingState(combinedSiblingParent);
        }
    }
}
