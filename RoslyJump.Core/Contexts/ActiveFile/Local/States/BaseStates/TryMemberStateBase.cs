using System;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates.States;
using RoslyJump.Core.Contexts.Local;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates
{
    public abstract class TryMemberStateBase<TNode>
        : LocalContextState<TNode, TryMemberSiblingState> where TNode : SyntaxNode
    {
        public TryMemberStateBase(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
            Type mixedType = contextNode.MixedNode.GetType();

            if (mixedType != typeof(TryBodySyntax)
                && mixedType != typeof(CatchClauseSyntax)
                && mixedType != typeof(FinallyClauseSyntax))
            {
                throw new ArgumentException(
                    $"Unsupported context node for {this.GetType()}.");
            }
        }

        protected override TryMemberSiblingState InitSiblingState()
        {
            return new TryMemberSiblingState(
                new CombinedSyntaxNode(
                    this.ContextNode?.BaseNode.ParentAs<TryStatementSyntax>()
                    ?? throw new InvalidOperationException(
                        "Unable to get the parent of a sibling state " +
                        $"for {nameof(TryMemberSiblingState)}.")
                    ));
        }
    }
}
