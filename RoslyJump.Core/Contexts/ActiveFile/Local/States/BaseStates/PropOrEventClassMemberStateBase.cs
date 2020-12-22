using System;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates
{
    public abstract class PropOrEventClassMemberStateBase<T> : ClassMemberStateBase<T>
        where T : SyntaxNode
    {
        protected PropOrEventClassMemberStateBase(
            LocalContext context,
            CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
            Type nodeType = contextNode.BaseNode.GetType();

            if (nodeType != typeof(PropertyDeclarationSyntax)
                && nodeType != typeof(EventDeclarationSyntax)
                && nodeType != typeof(EventFieldDeclarationSyntax))
            {
                throw new ArgumentException(
                    "The provided node is not a base node " +
                    $"for {nameof(PropOrEventClassMemberStateBase<T>)}");
            }
        }

        protected override CombinedSyntaxNode? QueryChildContextNode()
        {
            AccessorListSyntax? accessorList;
            ArrowExpressionClauseSyntax? arrowExpression = null;

            SyntaxNode baseNode = this.ActiveNode?.BaseNode ?? throw new NullReferenceException(
                $"The active node isn't set for {nameof(PropOrEventClassMemberStateBase<T>)}");

            if (baseNode is PropertyDeclarationSyntax prop && prop != null)
            {
                accessorList = prop.AccessorList;
                arrowExpression = prop.ExpressionBody;
            }
            else if (baseNode is EventDeclarationSyntax @event && @event != null)
            {
                accessorList = @event.AccessorList;
            }
            else if (baseNode is EventFieldDeclarationSyntax eventField && eventField != null)
            {
                return null;
            }
            else
            {
                throw new InvalidOperationException(
                    "Unable to find a base node for the child context node");
            }

            if (accessorList != null)
            {
                return accessorList.Accessors.First().QueryVirtualAndCombine(
                    MethodBodyVirtualQuery.Instance);
            }

            return arrowExpression != null
                ? arrowExpression.QueryVirtualAndCombine(MethodBodyVirtualQuery.Instance)
                : (CombinedSyntaxNode?)null;
        }
    }
}
