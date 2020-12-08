using System;
using System.Linq;
using dngrep.core.Queries;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslyJump.Core.Infrastructure
{
    // TODO: move to dngrep
    public class ClassMemberSyntaxNodeMatcher : ISyntaxNodeMatcher
    {
        private readonly static Type[] ClassMemberSiblings = new[]
        {
            typeof(PropertyDeclarationSyntax),
            typeof(EventDeclarationSyntax),
            typeof(FieldDeclarationSyntax),
            typeof(MethodDeclarationSyntax)
        };

        private ClassMemberSyntaxNodeMatcher()
        {

        }

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return ClassMemberSiblings.Contains(node.GetType());
        }

        public static ClassMemberSyntaxNodeMatcher Instance = new ClassMemberSyntaxNodeMatcher();
    }
}
