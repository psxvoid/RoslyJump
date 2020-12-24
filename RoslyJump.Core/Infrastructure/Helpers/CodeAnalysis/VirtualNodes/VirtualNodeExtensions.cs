using System;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Syntax;
using Microsoft.CodeAnalysis;
using static dngrep.core.VirtualNodes.Syntax.MethodBodyDeclarationSyntax;

namespace RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis.VirtualNodes
{
    public static class VirtualNodeExtensions
    {
        public static T BaseNodeAs<T>(this IVirtualSyntaxNode node)
            where T : SyntaxNode
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            if (!(node.BaseNode is T castedBaseNode))
            {
                throw new InvalidCastException(
                    "The provided base node cannot be casted to the specified type. " +
                    $"The actual type {node.BaseNode.GetType()}." +
                    $"The expected type {typeof(T)}");
            }

            return castedBaseNode;
        }

        public static bool HasExpression(this MethodBodyDeclarationSyntax body, string expression)
        {
            _ = body ?? throw new ArgumentNullException(nameof(body));
            _ = expression ?? throw new ArgumentNullException(nameof(expression));

            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new ArgumentException(
                    "The expressing string cannot be empty.",
                    nameof(expression));
            }

            if (body.ActiveBodyType != BodyType.ExpressionBody)
            {
                throw new InvalidOperationException(
                    $"The provided method body type is not {BodyType.ExpressionBody}. " +
                    $"The actual body type is {body.ActiveBodyType}.");
            }

            if (body.ExpressionBody == null)
            {
                throw new InvalidOperationException(
                    "The provided body has ExpressionBody type but it's not initialized.");
            }

            return body.ExpressionBody.ToString() == expression;
        }
    }
}
