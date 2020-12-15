using System;
using dngrep.core.VirtualNodes;
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
        
        protected override MethodBodyMemberSiblingState InitSiblingState()
        {
            SyntaxNode? siblingParent = this.BaseNode.GetContainingParent().GetBody();

            _ = siblingParent ?? throw new InvalidOperationException(
                "Unable to get the parent class or struct node.");

            return new MethodBodyMemberSiblingState(new CombinedSyntaxNode(siblingParent));
        }
    }
}
