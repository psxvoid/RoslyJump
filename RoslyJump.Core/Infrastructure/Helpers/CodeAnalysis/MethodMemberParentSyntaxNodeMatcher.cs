using System;
using System.Linq;
using dngrep.core.Queries;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis
{
    // TODO: move to dngrep
    public class MethodMemberParentSyntaxNodeMatcher
        : ISyntaxNodeMatcher, ICombinedSyntaxNodeMatcher
    {
        private static readonly MethodMemberParentSyntaxNodeMatcher LocalInstance =
            new MethodMemberParentSyntaxNodeMatcher();

        private static readonly Type[] MethodMembers = new[]
        {
            typeof(MethodDeclarationSyntax),
            typeof(ConstructorDeclarationSyntax),
            typeof(DestructorDeclarationSyntax),
            typeof(LocalFunctionStatementSyntax),
        };

        private static readonly Type[] MaybeMethodMembers = new[]
        {
            typeof(PropertyDeclarationSyntax),
            typeof(EventDeclarationSyntax),
        };

        private static readonly Type[] VirtualMethodMembers = new[]
        {
            typeof(ReadOnlyPropertyDeclarationSyntax)
        };

        private MethodMemberParentSyntaxNodeMatcher()
        {
        }

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            Type nodeType = node.GetType();

            bool isMethodMemberParent = MethodMembers.Contains(nodeType);

            if (isMethodMemberParent)
            {
                return true;
            }

            bool isPotentialMethodMember = MaybeMethodMembers.Contains(nodeType);

            if (!isPotentialMethodMember)
            {
                return false;
            }

            if (node is PropertyDeclarationSyntax prop
                && ((prop.AccessorList == null && prop.ExpressionBody != null)
                    || (prop.AccessorList != null && prop.AccessorList.Accessors.Count > 0
                        && prop.AccessorList.Accessors.Any(
                            x => x.Body != null || x.ExpressionBody != null))))
            {
                return true;
            }

            if (node is EventDeclarationSyntax @event
                    && @event.AccessorList != null && @event.AccessorList.Accessors.Count > 0
                        && @event.AccessorList.Accessors.Any(
                            x => x.Body != null || x.ExpressionBody != null))
            {
                return true;
            }

            return false;
        }

        public bool Match(CombinedSyntaxNode node)
        {
            if (node.IsVirtual)
            {
                return VirtualMethodMembers.Contains(node.MixedNode.GetType());
            }

            return this.Match(node.BaseNode);
        }

        public static MethodMemberParentSyntaxNodeMatcher Instance => LocalInstance;
    }
}
