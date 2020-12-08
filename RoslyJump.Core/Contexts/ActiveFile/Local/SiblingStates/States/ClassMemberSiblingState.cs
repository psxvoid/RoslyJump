using System;
using System.Linq;
using dngrep.core.Queries;
using dngrep.core.Queries.Specifiers;
using dngrep.core.Queries.SyntaxWalkers;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Infrastructure;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates.States
{
    public class ClassMemberSiblingState : SiblingStateBase
    {

        public ClassMemberSiblingState(CombinedSyntaxNode baseNode) : base(baseNode)
        {
            if(baseNode.BaseNode.GetType() != typeof(ClassDeclarationSyntax))
            {
                throw new ArgumentException(
                    "Only base node of type ClassDeclarationSyntax is supported for this state.",
                    nameof(baseNode));
            }
        }

        protected override CombinedSyntaxNode[] QueryTargetsProtected(CombinedSyntaxNode baseNode)
        {
            SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(
                new SyntaxTreeQueryDescriptor
                {
                    Scope = QueryTargetScope.Class
                });


            var walker = new SyntaxTreeQueryWalker(query);

            walker.Visit(baseNode.BaseNode);

            CombinedSyntaxNode[] members = walker.Results
                .Where(ClassMemberSyntaxNodeMatcher.Instance.Match)
                .GroupBy(x => x.Kind())
                .Select(x => new CombinedSyntaxNode(x.First()))
                .ToArray();

            return members;
        }
    }
}
