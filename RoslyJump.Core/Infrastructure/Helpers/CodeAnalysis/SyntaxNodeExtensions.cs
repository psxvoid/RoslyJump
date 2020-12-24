using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Infrastructure.Helpers.Generics;

namespace RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis
{
    public static class SyntaxNodeExtensions
    {
        /// <summary>
        /// Find the length to the first parent of the specified type.
        /// By "length" a one should think as of length of the parent inheritance chain
        /// from the target node to the node of the specified type. For example,
        /// if the target is <see cref="ClassDeclarationSyntax"/> and the generic
        /// constraint is <see cref="NamespaceDeclarationSyntax"/> and the target
        /// has non-empty parent of type <see cref="NamespaceDeclarationSyntax"/>
        /// then it will return 0, meaning it is a direct parent of the target node.
        /// If the parent won't be found then it will throw an exception.
        /// </summary>
        /// <typeparam name="T">The type of the parent node to look for.</typeparam>
        /// <param name="target">
        /// The node for which it is required to calculate a length to the parent of the specified type.
        /// </param>
        /// <returns>
        /// The length to the specified parent. It will be 0 for a direct parent.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Is thrown when the parent of the specified type isn't found in the
        /// parent inheritance change of <see cref="SyntaxNode"/>.
        /// </exception>
        public static int GetLengthToFirstParentOfType<T>(this SyntaxNode target)
            where T : SyntaxNode
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));

            SyntaxNode? parent = target.Parent;

            int length = 0;
            bool isParentFound = false;

            while (parent != null)
            {
                if (parent.GetType() == typeof(T))
                {
                    isParentFound = true;
                    break;
                }

                length++;
                parent = parent.Parent;
            }

            if (!isParentFound)
            {
                throw new InvalidOperationException($"No parent of type {typeof(T)} is found.");
            }

            return length;
        }

        /// <summary>
        /// Find the length to the specified parent.
        /// By "length" a one should think as of length of the parent inheritance chain
        /// from the target node to the specified parent node. For example,
        /// if the target is <see cref="ClassDeclarationSyntax"/> and the generic
        /// the target has non-empty parent of type <see cref="NamespaceDeclarationSyntax"/>
        /// then it will return 0, meaning it is a direct parent of the target node.
        /// If the parent won't be found then it will throw an exception.
        /// </summary>
        /// <param name="target">
        /// The node for which it is required to calculate a length to the parent of the specified type.
        /// </param>
        /// <returns>
        /// The length to the specified parent. It will be 0 for a direct parent.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Is thrown when the parent of the specified type isn't found in the
        /// parent inheritance change of <see cref="SyntaxNode"/>.
        /// </exception>
        public static int GetLengthToParent(this SyntaxNode target, SyntaxNode parentNode)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));
            _ = parentNode ?? throw new ArgumentNullException(nameof(parentNode));

            SyntaxNode? parent = target.Parent;

            int length = 0;
            bool isParentFound = false;

            while (parent != null)
            {
                if (parent == parentNode)
                {
                    isParentFound = true;
                    break;
                }

                length++;
                parent = parent.Parent;
            }

            if (!isParentFound)
            {
                throw new InvalidOperationException(
                    "The provided parent node is not parent of the target node.");
            }

            return length;
        }

        public static SyntaxNode GetContainerNode(this SyntaxNode target)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));

            SyntaxNode? current = target.Parent;

            while (current != null && !current.IsContainer())
            {
                current = current.Parent;
            }

            return current ?? throw new InvalidOperationException(
                "The target node does not have a container.");
        }

        /// <summary>
        /// Retrieves the first containing parent node that contains the specified node.
        /// The containing parent node can be a class, namespace, file
        /// (see <see cref="CompilationUnitSyntax"/>), etc. The method searches only for
        /// unique parents that have definitive state. For example, a method body
        /// does not have a definitive state because it can be represented as
        /// <see cref="BlockSyntax"/> or <see cref="ExpressionSyntax"/> (but not as both).
        /// </summary>
        /// <param name="target">
        /// The node for which it is requested to find the parent node.
        /// </param>
        /// <returns>
        /// The parent node that contains the target node.
        /// The containing parent node can be a class, namespace, or file
        /// (see <see cref="CompilationUnitSyntax"/>).
        /// </returns>
        public static SyntaxNode GetContainingParent(this SyntaxNode target)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));

            const string ErrorMessage = "Unable to get the parent for the target";

            CompilationUnitSyntax? fileParent = target
                .GetFirstParentOfType<CompilationUnitSyntax>();

            NamespaceDeclarationSyntax? namespaceParent = target
                .GetFirstParentOfType<NamespaceDeclarationSyntax>();

            ClassDeclarationSyntax? classParent = target
                .GetFirstParentOfType<ClassDeclarationSyntax>();

            StructDeclarationSyntax? structParent = target
                .GetFirstParentOfType<StructDeclarationSyntax>();

            MethodDeclarationSyntax? methodParent = target
                .GetFirstParentOfType<MethodDeclarationSyntax>();

            ConstructorDeclarationSyntax? ctorParent = target
                .GetFirstParentOfType<ConstructorDeclarationSyntax>();

            OperatorDeclarationSyntax? operatorParent = target
                .GetFirstParentOfType<OperatorDeclarationSyntax>();

            InterfaceDeclarationSyntax? interfaceParent = target
                .GetFirstParentOfType<InterfaceDeclarationSyntax>();

            LocalFunctionStatementSyntax? localFunctionParent = target
                .GetFirstParentOfType<LocalFunctionStatementSyntax>();

            PropertyDeclarationSyntax? propParent = target
                .GetFirstParentOfType<PropertyDeclarationSyntax>();

            EventDeclarationSyntax? eventParent = target
                .GetFirstParentOfType<EventDeclarationSyntax>();

            int nonNullableParentCount =
                (fileParent == null ? 0 : 1) +
                (namespaceParent == null ? 0 : 1) +
                (classParent == null ? 0 : 1) +
                (structParent == null ? 0 : 1) +
                (methodParent == null ? 0 : 1) +
                (ctorParent == null ? 0 : 1) +
                (operatorParent == null ? 0 : 1) +
                (interfaceParent == null ? 0 : 1) +
                (localFunctionParent == null ? 0 : 1) +
                (propParent == null ? 0 : 1);

            SyntaxNode? parent;

            if (nonNullableParentCount == 0)
            {
                throw new InvalidOperationException(ErrorMessage);
            }
            else if (nonNullableParentCount == 1)
            {
                parent =
                    (SyntaxNode?)fileParent ??
                    (SyntaxNode?)namespaceParent ??
                    (SyntaxNode?)classParent ??
                    (SyntaxNode?)structParent ??
                    (SyntaxNode?)methodParent ??
                    (SyntaxNode?)ctorParent ??
                    (SyntaxNode?)operatorParent ??
                    (SyntaxNode?)interfaceParent ??
                    (SyntaxNode?)localFunctionParent ??
                    (SyntaxNode?)propParent ??
                    (SyntaxNode?)eventParent;
            }
            else
            {
                List<SyntaxNode> nonNullableParents = new List<SyntaxNode>();

#pragma warning disable CS8604 // Possible null reference argument.
                nonNullableParents.AddIfNotNull(fileParent);
                nonNullableParents.AddIfNotNull(namespaceParent);
                nonNullableParents.AddIfNotNull(classParent);
                nonNullableParents.AddIfNotNull(structParent);
                nonNullableParents.AddIfNotNull(methodParent);
                nonNullableParents.AddIfNotNull(ctorParent);
                nonNullableParents.AddIfNotNull(operatorParent);
                nonNullableParents.AddIfNotNull(interfaceParent);
                nonNullableParents.AddIfNotNull(localFunctionParent);
                nonNullableParents.AddIfNotNull(propParent);
                nonNullableParents.AddIfNotNull(eventParent);
#pragma warning restore CS8604 // Possible null reference argument.

                parent = nonNullableParents
                    .Select(x => new KeyValuePair<SyntaxNode, int>(x, target.GetLengthToParent(x)))
                    .OrderBy(x => x.Value)
                    .First().Key;
            }

            if (parent == null)
            {
                throw new InvalidOperationException(ErrorMessage);
            }

            return parent;
        }

        /// <summary>
        /// Checks whether the specified node is an event.
        /// </summary>
        /// <param name="node">
        /// The node that should be verified whether it is
        /// an event or not.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the specified <see cref="SyntaxNode"/>
        /// is an event, else <see langword="false"/>.
        /// </returns>
        public static bool IsEvent(this SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return node is EventDeclarationSyntax || node is EventFieldDeclarationSyntax;
        }

        /// <summary>
        /// Verifies that the target class has a specified name.
        /// </summary>
        /// <param name="class">
        /// The <see cref="SyntaxNode"/> the for which the name should be verified.
        /// </param>
        /// <param name="name">
        /// The name that the target class should have.
        /// </param>
        /// <returns>
        /// <see cref="true"/> when the target has the specified name,
        /// else <see cref="false"/>.
        /// </returns>
        public static bool HasName(this ClassDeclarationSyntax @class, string name)
        {
            _ = @class ?? throw new ArgumentNullException(nameof(@class));
            _ = name ?? throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The name cannot be empty.", nameof(name));
            }

            return @class.Identifier.ValueText == name;
        }

        /// <summary>
        /// Verifies that the target method has a specified name.
        /// </summary>
        /// <param name="method">
        /// The <see cref="SyntaxNode"/> the for which the name should be verified.
        /// </param>
        /// <param name="name">
        /// The name that the target method should have.
        /// </param>
        /// <returns>
        /// <see cref="true"/> when the target has the specified name,
        /// else <see cref="false"/>.
        /// </returns>
        public static bool HasName(this MethodDeclarationSyntax method, string name)
        {
            _ = method ?? throw new ArgumentNullException(nameof(method));
            _ = name ?? throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The name cannot be empty.", nameof(name));
            }

            return method.Identifier.ValueText == name;
        }

        /// <summary>
        /// Verifies that the target field has a specified name.
        /// </summary>
        /// <param name="field">
        /// The <see cref="SyntaxNode"/> the for which the name should be verified.
        /// </param>
        /// <param name="name">
        /// The name that the target field should have.
        /// </param>
        /// <returns>
        /// <see cref="true"/> when the target has the specified name,
        /// else <see cref="false"/>.
        /// </returns>
        public static bool HasName(this FieldDeclarationSyntax field, string name)
        {
            _ = field ?? throw new ArgumentNullException(nameof(field));
            _ = name ?? throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The name cannot be empty.", nameof(name));
            }

            return field.GetIdentifierName() == name;
        }

        /// <summary>
        /// Verifies that the target property has a specified name.
        /// </summary>
        /// <param name="prop">
        /// The <see cref="SyntaxNode"/> the for which the name should be verified.
        /// </param>
        /// <param name="name">
        /// The name that the target property should have.
        /// </param>
        /// <returns>
        /// <see cref="true"/> when the target has the specified name,
        /// else <see cref="false"/>.
        /// </returns>
        public static bool HasName(this PropertyDeclarationSyntax prop, string name)
        {
            _ = prop ?? throw new ArgumentNullException(nameof(prop));
            _ = name ?? throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The name cannot be empty.", nameof(name));
            }

            return prop.GetIdentifierName() == name;
        }

        /// <summary>
        /// Retrieves the parent node of the specified node
        /// and casts it to the specified type. Throws an exception
        /// when the parent node cannot be retrieved or casted.
        /// </summary>
        /// <typeparam name="T">
        /// The target type to which the parent node should be casted.
        /// </typeparam>
        /// <param name="node">
        /// The target node for which the parent should be retrieved.
        /// </param>
        /// <returns>
        /// The parent node of the specified node casted to
        /// the specified type.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the parent node of the target cannot be
        /// obtained.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// Thrown when the parent node cannot be casted to the
        /// target type.
        /// </exception>
        public static T ParentAs<T>(this SyntaxNode node)
            where T : SyntaxNode
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            if (node.Parent == null)
            {
                throw new InvalidOperationException(
                    "The parent doesn't exist on the target node.");
            }

            if (!(node.Parent is T notNullableParent))
            {
                throw new InvalidCastException(
                    "The parent exists on the target node " +
                    "but it doesn't match the expected type. " +
                    $"The actual type: {node.Parent.GetType()}. " +
                    $"The expected type: {typeof(T)}.");
            }

            return notNullableParent;
        }

        /// <summary>
        /// Retrieves the parent node of the specified node
        /// and casts it to the specified type. In contrast to
        /// <see cref="ParentAs{T}(SyntaxNode)"/> it does not throw
        /// an exception when the parent node cannot be casted to
        /// the specified type but instead returns <see cref="null"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The target type to which the parent node should be casted.
        /// </typeparam>
        /// <param name="node">
        /// The target node for which the parent should be retrieved.
        /// </param>
        /// <returns>
        /// The parent node of the specified node casted to
        /// the specified type or <see cref="null"/> in case it
        /// cannot be casted.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the parent node of the target cannot be
        /// obtained.
        /// </exception>
        public static T? ParentAsOrNull<T>(this SyntaxNode node)
            where T : SyntaxNode
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            if (node.Parent == null)
            {
                throw new InvalidOperationException(
                    "The parent doesn't exist on the target node.");
            }

            return node.Parent as T;
        }

        /// <summary>
        /// Determines whether the provided <see cref="IfStatementSyntax"/>
        /// condition equals to the specified string representation of the
        /// same condition.
        /// </summary>
        /// <param name="if">
        /// The syntax node for which the condition should be verified on
        /// the equality with it's string representation.
        /// </param>
        /// <param name="condition">
        /// The string representation of the condition.
        /// </param>
        /// <returns>
        /// <see cref="true"/> when the condition of the specified
        /// <see cref="IfStatementSyntax"/> matches it's string
        /// representation, else <see cref="false"/>.
        /// </returns>
        public static bool HasCondition(this IfStatementSyntax @if, string condition)
        {
            _ = @if ?? throw new ArgumentNullException(nameof(@if));
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            return @if.Condition.ToString() == condition;
        }
    }
}
