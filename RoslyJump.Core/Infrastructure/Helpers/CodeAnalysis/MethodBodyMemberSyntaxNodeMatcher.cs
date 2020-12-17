using System;
using System.Linq;
using dngrep.core.Queries;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslyJump.Core.Infrastructure
{
    // TODO: move to dngrep
    public class MethodBodyMemberSyntaxNodeMatcher : ISyntaxNodeMatcher
    {
        private static readonly MethodBodyMemberSyntaxNodeMatcher instance =
            new MethodBodyMemberSyntaxNodeMatcher();

        private readonly static Type[] MethodBodyMemberSiblings = new[]
        {
            typeof(IfStatementSyntax),
            typeof(LocalDeclarationStatementSyntax),
            typeof(ReturnStatementSyntax),
            typeof(ForStatementSyntax),
            typeof(ForEachStatementSyntax),
        };

        private MethodBodyMemberSyntaxNodeMatcher()
        {
        }

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return MethodBodyMemberSiblings.Contains(node.GetType());
        }

        public static MethodBodyMemberSyntaxNodeMatcher Instance => instance;
    }
}
