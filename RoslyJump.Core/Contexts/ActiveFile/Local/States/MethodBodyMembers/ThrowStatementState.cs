using System;
using System.Linq;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyMembers
{
    public class ThrowStatementState : MethodBodyMemberStateBase<ThrowStatementSyntax>
    {
        public ThrowStatementState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            return ActiveBaseNode.Parent?.ChildNodes()
                ?.Where(x => x.GetType() == typeof(ThrowStatementSyntax))
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray()
                ?? throw new InvalidOperationException(
                    $"Unable to query target nodes for {nameof(ThrowStatementState)}.");
        }
    }
}
