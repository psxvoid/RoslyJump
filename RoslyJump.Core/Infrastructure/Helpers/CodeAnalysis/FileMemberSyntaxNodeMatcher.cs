using System;
using System.Linq;
using dngrep.core.Queries;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslyJump.Core.Infrastructure
{
    // TODO: move to dngrep
    public class FileMemberSyntaxNodeMatcher : ISyntaxNodeMatcher
    {
        private static readonly FileMemberSyntaxNodeMatcher LocalInstance =
            new FileMemberSyntaxNodeMatcher();

        private static readonly Type[] FileMemberSiblings = new[]
        {
            typeof(NamespaceDeclarationSyntax),
            typeof(UsingDirectiveSyntax),
            typeof(ClassDeclarationSyntax),
            typeof(EnumDeclarationSyntax),
            typeof(StructDeclarationSyntax),
            typeof(InterfaceDeclarationSyntax),
        };

        private FileMemberSyntaxNodeMatcher()
        {
        }

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return FileMemberSiblings.Contains(node.GetType());
        }

        public static FileMemberSyntaxNodeMatcher Instance => LocalInstance;
    }
}
