using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslyJump.Core.Contexts.Local.States
{
    public sealed class MethodParameterState : LocalContextState
    {
        public MethodParameterState(LocalContext context, SyntaxNode contextNode) :
            base(context, contextNode)
        {
        }

        protected override SyntaxNode[] QueryTargetNodesFunc()
        {
            ParameterListSyntax paramList =
                this.ContextNode.GetFirstParentOfType<ParameterListSyntax>();

            return paramList.Parameters.ToArray();
        }

        public override void TransitionTo(SyntaxNode syntaxNode, LocalContext context)
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
    }
}
