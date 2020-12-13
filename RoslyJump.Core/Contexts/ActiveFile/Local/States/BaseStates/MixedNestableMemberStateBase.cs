using System;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis;
using RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates.States;
using RoslyJump.Core.Contexts.Local;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates
{
    public abstract class MixedNestableMemberStateBase<TNode>
        : LocalContextState<TNode, FileNamespaceClassMemberSiblingState>
        where TNode : SyntaxNode
    {
        protected MixedNestableMemberStateBase(
            LocalContext context,
            CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override FileNamespaceClassMemberSiblingState InitSiblingState()
        {
            _ = this.ContextNode ?? throw new InvalidOperationException(
                "The context node should be initialized before initializing the sibling state.");

            SyntaxNode siblingParent = this.BaseNode.GetContainingParent();

            return new FileNamespaceClassMemberSiblingState(new CombinedSyntaxNode(siblingParent));
        }
    }
}
