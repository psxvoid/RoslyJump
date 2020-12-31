using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyStates
{
    public class ElseClauseState : IfMemberStateBase<ElseClauseSyntax>
    {
        private readonly CombinedSyntaxNode[] targets;

        public ElseClauseState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
            if (contextNode.MixedNode.GetType() != typeof(ElseClauseSyntax))
            {
                throw new ArgumentException(
                    $"The provided context node is not supported by {nameof(ElseClauseState)}.\"" +
                    $"Actual: {contextNode.MixedNode.GetType()}.",
                    nameof(contextNode));
            }

            this.targets = new[] { contextNode };
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            var elseClauses = new List<ElseClauseSyntax>();

            // get parent else clauses (including active)

            IfStatementSyntax? current = this.BaseNode.ParentAs<IfStatementSyntax>();

            while (
                current != null
                && current.GetType() == typeof(IfStatementSyntax)
                && current.Else != null)
            {
                elseClauses.Add(current.Else);
                if (current.Parent is IfStatementSyntax ifStatement)
                {
                    current = ifStatement;
                }
                else
                {
                    current = null;
                }
            }

            // get child else clauses

            current = this.BaseNode.Statement as IfStatementSyntax;

            while (
                current != null
                && current.GetType() == typeof(IfStatementSyntax)
                && current.Else != null)
            {
                elseClauses.Add(current.Else);

                current = current.Else?.Statement as IfStatementSyntax;
            }

            return elseClauses.Select(x => new CombinedSyntaxNode(x))
                .ToArray();
        }

        protected override CombinedSyntaxNode? QueryParentContextNode()
        {
            IfStatementSyntax? ifRoot = this.BaseNode.ParentAs<IfStatementSyntax>();

            while (ifRoot?.Parent is ElseClauseSyntax @else && @else != null)
            {
                ifRoot = @else.ParentAs<IfStatementSyntax>();
            }

            return new CombinedSyntaxNode(ifRoot ?? throw new InvalidOperationException(
                $"Unable to find the parent node for {nameof(ElseClauseState)}."));
        }
    }
}
