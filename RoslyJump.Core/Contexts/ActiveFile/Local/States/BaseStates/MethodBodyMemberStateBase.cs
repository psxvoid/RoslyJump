using System;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.VirtualQueries;
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
            SyntaxNode parent = this.BaseNode.GetContainingParent();

            SyntaxNode body = parent.GetBody();

            if (!MethodBodyVirtualQuery.Instance.CanQuery(body))
            {
                throw new InvalidOperationException(
                    $"Unable to query parent node for {nameof(MethodBodyMemberSiblingState)}");
            }

            return new CombinedSyntaxNode(MethodBodyVirtualQuery.Instance.Query(body));
        }

        protected override MethodBodyMemberSiblingState InitSiblingState()
        {
            SyntaxNode? siblingParent = this.BaseNode.GetContainingParent().GetBody();

            _ = siblingParent ?? throw new InvalidOperationException(
                "Unable to get the parent class or struct node.");

            return new MethodBodyMemberSiblingState(new CombinedSyntaxNode(siblingParent));
        }
    }
}
