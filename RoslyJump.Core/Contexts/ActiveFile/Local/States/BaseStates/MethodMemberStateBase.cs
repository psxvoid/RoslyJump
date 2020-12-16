using System;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis;
using RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates.States;
using RoslyJump.Core.Contexts.Local;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates
{
    public abstract class MethodMemberStateBase<TNode>
        : LocalContextState<TNode, MethodMemberSiblingState>
        where TNode : SyntaxNode
    {
        protected MethodMemberStateBase(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
            if (!MethodMemberSyntaxNodeMatcher.Instance.Match(contextNode.BaseNode))
            {
                throw new ArgumentException(
                    "The provided context node is not a member of a method declaration.",
                    nameof(contextNode));
            }
        }

        protected override MethodMemberSiblingState InitSiblingState()
        {
            SyntaxNode? siblingParent = this.BaseNode.GetContainingParent();

            _ = siblingParent ?? throw new InvalidOperationException(
                "Unable to get the parent class or struct node.");

            if (!MethodMemberParentSyntaxNodeMatcher.Instance.Match(siblingParent))
            {
                throw new InvalidOperationException(
                    "Unable to query the method member parent " +
                    $"for {nameof(MethodMemberSiblingState)}");
            }

            return new MethodMemberSiblingState(new CombinedSyntaxNode(siblingParent));
        }
    }
}
