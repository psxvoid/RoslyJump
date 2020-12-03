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
            ParameterListSyntax paramList =
                ((CombinedSyntaxNode)this.ContextNode).Node
                .GetFirstParentOfType<ParameterListSyntax>();

            return paramList.Parameters.Select(x => new CombinedSyntaxNode(x)).ToArray();
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
    }
}
