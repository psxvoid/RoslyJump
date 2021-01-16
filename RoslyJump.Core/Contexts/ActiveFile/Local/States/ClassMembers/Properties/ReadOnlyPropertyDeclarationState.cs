using System;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Syntax;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.ClassMembers.Properties
{
    public class ReadOnlyPropertyDeclarationState :
        PropertyClassMemberStateBase<PropertyDeclarationSyntax>
    {
        protected override int JumpDownCount => 2;

        public ReadOnlyPropertyDeclarationState(
            LocalContext context,
            CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
            if (contextNode.MixedNode.GetType() != typeof(ReadOnlyPropertyDeclarationSyntax))
            {
                throw new ArgumentException(
                    $"Unsupported base node type for {nameof(ReadOnlyPropertyDeclarationState)}");
            }
        }

        protected override CombinedSyntaxNode? QueryChildContextNode()
        {
            ArrowExpressionClauseSyntax? expressionBody = this.BaseNode.ExpressionBody;

            if (expressionBody == null)
            {
                throw new ArgumentException(
                    "Unable to query a child node for " + 
                    $"{nameof(ReadOnlyPropertyDeclarationState)}");
            }

            return expressionBody.QueryVirtualAndCombine(NestedBlockVirtualQuery.Instance);
        }
    }
}
