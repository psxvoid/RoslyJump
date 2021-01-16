using System;
using System.Linq;
using dngrep.core.Queries;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslyJump.Core.Infrastructure
{
    // TODO: move to dngrep
    public class NamespaceMemberSyntaxNodeMatcher : ISyntaxNodeMatcher
    {
        private static readonly NamespaceMemberSyntaxNodeMatcher LocalInstance =
            new NamespaceMemberSyntaxNodeMatcher();

        private static readonly Type[] NamespaceMemberSiblings = new[]
        {
            typeof(NamespaceDeclarationSyntax),
            typeof(UsingDirectiveSyntax),
            typeof(ClassDeclarationSyntax),
            typeof(EnumDeclarationSyntax),
            typeof(StructDeclarationSyntax),
            typeof(InterfaceDeclarationSyntax),
        };

        private NamespaceMemberSyntaxNodeMatcher()
        {
        }

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return NamespaceMemberSiblings.Contains(node.GetType());
        }

        public static NamespaceMemberSyntaxNodeMatcher Instance => LocalInstance;
    }
}
