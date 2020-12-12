using System;
using System.Linq;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Infrastructure;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates.States
{
    public class FileNamespaceClassMemberSiblingState : SiblingStateBase
    {
        public FileNamespaceClassMemberSiblingState(CombinedSyntaxNode baseNode) : base(baseNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetsProtected(CombinedSyntaxNode baseNode)
        {
            Func<SyntaxNode, bool> match;

            if (baseNode.BaseNode is CompilationUnitSyntax)
            {
                match = FileMemberSyntaxNodeMatcher.Instance.Match;
            }
            else if (baseNode.BaseNode is NamespaceDeclarationSyntax)
            {
                match = NamespaceMemberSyntaxNodeMatcher.Instance.Match;
            }
            else if (baseNode.BaseNode is ClassDeclarationSyntax)
            {
                match = ClassMemberSyntaxNodeMatcher.Instance.Match;
            }
            else
            {
                throw new ArgumentException("Unsupported node type", nameof(baseNode));
            }

            CombinedSyntaxNode[] members = baseNode.BaseNode.ChildNodes()
                .Where(match)
                .GroupBy(x => x.Kind())
                .Select(x => new CombinedSyntaxNode(x.First()))
                .ToArray();

            return members;
        }
    }
}
