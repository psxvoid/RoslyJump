using System;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates.States;
using RoslyJump.Core.Contexts.Local;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates
{
    public abstract class MixedNestableMemberStateBase<TNode>
        : LocalContextState<TNode, MixedMemberSiblingState>
        where TNode : SyntaxNode
    {
        protected MixedNestableMemberStateBase(
            LocalContext context,
            CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override MixedMemberSiblingState InitSiblingState()
        {
            _ = this.ContextNode ?? throw new InvalidOperationException(
                "The context node should be initialized before initializing the sibling state.");

            SyntaxNode siblingParent;

            if (this.BaseNode is CompilationUnitSyntax)
            {
                // CompilationUnitSyntax is the last possible parent in a file
                // we don't want to change it (to preserve the same sibling context).
                siblingParent = this.BaseNode;
            }
            else
            {
                siblingParent = this.BaseNode.GetContainingParent();
            }

            return new MixedMemberSiblingState(new CombinedSyntaxNode(siblingParent));
        }
    }
}
