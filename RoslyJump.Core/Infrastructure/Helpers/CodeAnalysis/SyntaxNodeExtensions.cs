using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        /// <summary>
        /// Retrieves the first containing parent node that contains the specified node.
        /// The containing parent node can be a class, namespace, or file
        /// (see <see cref="CompilationUnitSyntax"/>).
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
            const string ErrorMessage = "Unable to get the parent for the target";

            CompilationUnitSyntax? fileParent = target
                .GetFirstParentOfType<CompilationUnitSyntax>();

            NamespaceDeclarationSyntax? namespaceParent = target
                .GetFirstParentOfType<NamespaceDeclarationSyntax>();

            ClassDeclarationSyntax? classParent = target
                .GetFirstParentOfType<ClassDeclarationSyntax>();
            
            StructDeclarationSyntax? structParent = target
                .GetFirstParentOfType<StructDeclarationSyntax>();

            int nonNullableParentCount =
                (fileParent == null ? 0 : 1) +
                (namespaceParent == null ? 0 : 1) +
                (classParent == null ? 0 : 1) +
                (structParent == null ? 0 : 1);

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
                    (SyntaxNode?)classParent;
            }
            else
            {
                List<SyntaxNode> nonNullableParents = new List<SyntaxNode>();
                if (fileParent != null) nonNullableParents.Add(fileParent);
                if (namespaceParent != null) nonNullableParents.Add(namespaceParent);
                if (classParent != null) nonNullableParents.Add(classParent);
                if (structParent != null) nonNullableParents.Add(structParent);

                parent = nonNullableParents
                    .Select(x =>
                        new KeyValuePair<SyntaxNode, int>(x, target.GetLengthToParent(x)))
                    .OrderBy(x => x.Value)
                    .First().Key;
            }

            if (parent == null)
            {
                throw new InvalidOperationException(ErrorMessage);
            }

            return parent;
        }
    }
}
