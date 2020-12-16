using System;
using System.Linq;
using dngrep.core.Queries;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis
{
    // TODO: move to dngrep
    public class MethodMemberParentSyntaxNodeMatcher : ISyntaxNodeMatcher
    {
        private static readonly MethodMemberParentSyntaxNodeMatcher instance =
            new MethodMemberParentSyntaxNodeMatcher();

        private readonly static Type[] MethodMembers = new[]
        {
            typeof(MethodDeclarationSyntax),
            typeof(ConstructorDeclarationSyntax),
        };

        private MethodMemberParentSyntaxNodeMatcher()
        {
        }

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return MethodMembers.Contains(node.GetType());
        }

        public static MethodMemberParentSyntaxNodeMatcher Instance => instance;
    }
}
