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
        private static readonly ClassMemberSyntaxNodeMatcher instance =
            new ClassMemberSyntaxNodeMatcher();

        private readonly static Type[] ClassMemberSiblings = new[]
        {
            typeof(ConstructorDeclarationSyntax),
            typeof(InterfaceDeclarationSyntax),
            typeof(StructDeclarationSyntax),
            typeof(ClassDeclarationSyntax),
            typeof(PropertyDeclarationSyntax),
            typeof(EventDeclarationSyntax),
            typeof(EventFieldDeclarationSyntax),
            typeof(FieldDeclarationSyntax),
            typeof(MethodDeclarationSyntax),
            typeof(OperatorDeclarationSyntax),
            typeof(DestructorDeclarationSyntax),
            typeof(IndexerDeclarationSyntax),
        };

        private ClassMemberSyntaxNodeMatcher()
        {
        }

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return ClassMemberSiblings.Contains(node.GetType());
        }

        public static ClassMemberSyntaxNodeMatcher Instance => instance;
    }
}
