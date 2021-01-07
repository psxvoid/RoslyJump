using System;
using System.Linq;
using dngrep.core.Queries.SyntaxNodeMatchers.Targets;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyMembers
{
    public class StatementState : MethodBodyMemberStateBase<StatementSyntax>
    {
        static StatementState()
        {
        }

        public StatementState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            SyntaxNode? parent = this.BaseNode.Parent;

            parent = parent is LabeledStatementSyntax ? parent.Parent : parent;

            return parent?.ChildNodes()
                .Select(x => x is LabeledStatementSyntax label ? label.Statement : x)
                .QueryVirtualAndCombine(VirtualQueryExtensions.GetAllSupportedQueries())
                .Where(x =>
                    // all known types should have more narrowed states
                    // (child of StatementSyntax) that is why we have to
                    // exclude them. All not implemented statements will
                    // use this state instead
                    !LocalContext.IsKnownNodeType(x)
                    && MethodBodyMemberSyntaxNodeMatcher.Instance.Match(x))
                .ToArray()
                ?? throw new InvalidOperationException(
                    $"Unable to query the target nodes for {nameof(StatementState)}");
        }
    }
}
