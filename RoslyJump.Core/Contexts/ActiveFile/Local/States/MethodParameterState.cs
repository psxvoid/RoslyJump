using System;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslyJump.Core.Contexts.Local.States
{
    public sealed class MethodParameterState : LocalContextState
    {
        public MethodParameterState(LocalContext context, CombinedSyntaxNode contextNode) :
            base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            _ = this.ContextNode ?? throw new NullReferenceException(
                "The context node is not set for MethodParameterState");

            ParameterListSyntax? paramList =
                this.ContextNode?.Node
                ?.GetFirstParentOfType<ParameterListSyntax>();

            return paramList?.Parameters.Select(x => new CombinedSyntaxNode(x)).ToArray()
                ?? throw new NullReferenceException(
                    "ParameterList node isn't found for the MethodParameterState.");
        }

        public override void TransitionTo(CombinedSyntaxNode? syntaxNode, LocalContext context)
        {
            if (
                syntaxNode != null
                && syntaxNode.GetType() == typeof(ParameterSyntax)
                && syntaxNode == this.ContextNode)
            {
                // the correct state is already set
                return;
            }

            base.TransitionTo(syntaxNode, context);
        }

        protected override CombinedSyntaxNode? QueryParentContextNode()
        {
            return new CombinedSyntaxNode(
                this.ContextNode?.BaseNode.GetFirstParentOfType<MethodDeclarationSyntax>()
                ?? throw new NullReferenceException(
                    "Unable to query parent context node for the MethodParameterState."));
        }
    }
}
