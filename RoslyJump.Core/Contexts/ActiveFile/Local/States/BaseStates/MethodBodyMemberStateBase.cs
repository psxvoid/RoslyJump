using System;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis;
using RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates.States;
using RoslyJump.Core.Contexts.Local;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates
{
    public abstract class MethodBodyMemberStateBase<TNode>
        : LocalContextState<TNode, MethodBodyMemberSiblingState>
        where TNode : SyntaxNode
    {
        protected MethodBodyMemberStateBase(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode? QueryParentContextNode()
        {
            SyntaxNode parent = this.BaseNode.GetContainerNode();

            return parent.QueryVirtualAndCombine(
                MethodBodyVirtualQuery.Instance,
                NestedBlockVirtualQuery.Instance);
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
