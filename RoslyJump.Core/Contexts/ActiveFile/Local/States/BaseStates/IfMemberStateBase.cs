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
    public abstract class IfMemberStateBase<TNode>
        : LocalContextState<TNode, IfMemberSiblingState> where TNode : SyntaxNode
    {
        public IfMemberStateBase(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
            Type mixedType = contextNode.MixedNode.GetType();

            if (mixedType != typeof(IfConditionSyntax)
                && mixedType != typeof(IfBodySyntax)
                && mixedType != typeof(ElseClauseSyntax)
                && mixedType != typeof(ElseBodySyntax))
            {
                throw new ArgumentException(
                    $"Unsupported context node for {this.GetType()}.");
            }
        }

        protected override IfMemberSiblingState InitSiblingState()
        {
            return new IfMemberSiblingState(
                new CombinedSyntaxNode(
                    (this.ContextNode?.MixedNode.GetType() == typeof(ElseBodySyntax)
                        ? this.ContextNode?.BaseNode
                            .ParentAs<ElseClauseSyntax>()
                            .ParentAs<IfStatementSyntax>()
                        : this.ContextNode?.BaseNode.ParentAs<IfStatementSyntax>())
                    ?? throw new InvalidOperationException(
                        "Unable to get the parent of a sibling state " +
                        $"for {nameof(IfMemberSiblingState)}.")
                    ));
        }
    }
}
