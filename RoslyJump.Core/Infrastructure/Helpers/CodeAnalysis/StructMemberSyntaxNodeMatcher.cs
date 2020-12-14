using System;
using System.Linq;
using dngrep.core.Queries;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslyJump.Core.Infrastructure
{
    // TODO: move to dngrep
    public class StructMemberSyntaxNodeMatcher : ISyntaxNodeMatcher
    {
        private static readonly StructMemberSyntaxNodeMatcher instance =
            new StructMemberSyntaxNodeMatcher();

        private readonly static Type[] StructMemberSiblings = new[]
        {
            typeof(ConstructorDeclarationSyntax),
            typeof(PropertyDeclarationSyntax),
            typeof(EventDeclarationSyntax),
            typeof(FieldDeclarationSyntax),
            typeof(MethodDeclarationSyntax),
            typeof(OperatorDeclarationSyntax),
        };

        private StructMemberSyntaxNodeMatcher()
        {
        }

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return StructMemberSiblings.Contains(node.GetType());
        }

        public static StructMemberSyntaxNodeMatcher Instance => instance;
    }
}
